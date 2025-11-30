using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Path (waypoints)")]
    public Transform[] patrolPoints;              // Assign in Inspector
    public float patrolSpeed = 2f;
    public float patrolPointTolerance = 0.2f;     // how close is "reached"

    [Header("Vision Cone (Light-based)")]
    public Light visionLight;                     // drag your red spotlight here
    public float visionDistance = 15f;            // if 0, uses light.range
    public float visionAngle = 40f;               // if 0, uses light.spotAngle * 0.5f
    public LayerMask obstacleMask = ~0;           // what blocks line of sight

    [Header("Chase")]
    public Transform player;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 8f;
    public float stopDistance = 1.2f;

    [Header("Chase Timing")]
    public float loseSightDelay = 0.6f;           // delay before he stops chasing

    [Header("Collision")]
    public float collisionRadius = 0.4f;          // how “fat” the goblin is for checking walls
    public float collisionHeight = 1.0f;          // ray height above the floor
    public float collisionSkin = 0.02f;           // tiny gap so he doesn’t clip inside walls

    [Header("Head Look IK")]
    public Transform lookTarget;
    [Range(0, 1)] public float lookWeight = 1f;

    [Header("Animation")]
    public Animator anim;                         // GoblinModel’s Animator
    public string speedParam = "Speed";
    public string isChasingParam = "IsChasing";

    // --- private ---
    Rigidbody rb;
    float baseY;
    int currentPatrolIndex = 0;

    bool HasPatrolPath => patrolPoints != null && patrolPoints.Length > 0;

    // chase state
    bool isChasing = false;
    bool wasChasing = false;
    float timeSinceLastSeen = 999f;               // start as "not seeing"

    // anti-stuck while patrolling
    Vector3 lastPatrolPos;
    float stuckTimer = 0f;
    public float stuckMoveThreshold = 0.02f;      // how far he must move to count as "moving"
    public float stuckTeleportTime = 2.0f;        // seconds stuck before teleport

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezePositionY;

        if (!anim)
            anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!lookTarget && player)
            lookTarget = player;

        baseY = transform.position.y;
        lastPatrolPos = transform.position;

        if (!visionLight)
            Debug.LogWarning("EnemyPatrol: no visionLight assigned, enemy will always see player.");
    }

    void Update()
    {
        // 1) SEE / NOT SEE logic with small delay
        bool canSeeNow = PlayerInsideLightCone();

        if (canSeeNow)
            timeSinceLastSeen = 0f;
        else
            timeSinceLastSeen += Time.deltaTime;

        wasChasing = isChasing;
        isChasing = timeSinceLastSeen < loseSightDelay;

        // 2) State behaviour
        if (isChasing)
        {
            if (anim)
            {
                anim.SetBool(isChasingParam, true);
                anim.SetFloat(speedParam, chaseSpeed);
            }
            ChasePlayer();
        }
        else
        {
            if (anim)
            {
                anim.SetBool(isChasingParam, false);
                anim.SetFloat(speedParam, patrolSpeed);
            }
            Patrol();
        }

        // 3) Just stopped chasing? snap to closest patrol point (for direction)
        if (wasChasing && !isChasing)
        {
            SetNearestPatrolPoint();
            lastPatrolPos = transform.position;
            stuckTimer = 0f;
        }
    }

    void LateUpdate()
    {
        if (!anim || !lookTarget) return;

        if (isChasing)
        {
            anim.SetLookAtWeight(lookWeight, 0.3f, 1f, 0f);
            anim.SetLookAtPosition(lookTarget.position);
        }
        else
        {
            anim.SetLookAtWeight(0f);
        }
    }

    // ---------------------- DETECTION ----------------------
    bool PlayerInsideLightCone()
    {
        if (!player) return false;

        // no light = simple distance check
        if (!visionLight)
        {
            Vector3 flat = player.position - transform.position;
            flat.y = 0f;
            return flat.magnitude < visionDistance;
        }

        Vector3 origin = visionLight.transform.position;
        Vector3 forward = visionLight.transform.forward;
        Vector3 toPlayer = player.position - origin;

        float dist = toPlayer.magnitude;
        float maxDist = visionDistance > 0f ? visionDistance : visionLight.range;
        if (dist > maxDist) return false;

        float halfAngle = (visionAngle > 0f ? visionAngle : visionLight.spotAngle) * 0.5f;
        float angle = Vector3.Angle(forward, toPlayer);
        if (angle > halfAngle) return false;

        // Raycast to check walls
        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Player"))
                return false;
        }

        return true;
    }

    // Move toward targetPos, but stop if a wall is in the way
    void MoveWithCollision(Vector3 targetPos, float speed)
    {
        Vector3 currentPos = transform.position;
        Vector3 toTarget = targetPos - currentPos;
        float distanceThisFrame = speed * Time.deltaTime;

        if (toTarget.sqrMagnitude <= 0.0001f)
            return;

        Vector3 dir = toTarget.normalized;

        // start the cast a bit above the ground
        Vector3 origin = currentPos + Vector3.up * collisionHeight;

        // check if we'll hit a wall this frame
        bool hitWall = Physics.SphereCast(
            origin,
            collisionRadius,
            dir,
            out RaycastHit hit,
            distanceThisFrame + collisionSkin,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitWall)
        {
            // free space, move normally
            transform.position = currentPos + dir * distanceThisFrame;
        }
        else
        {
            // hit a wall, move up to the wall but not through it
            float moveDist = Mathf.Max(0f, hit.distance - collisionSkin);
            transform.position = currentPos + dir * moveDist;
        }
    }

    // ---------------------- CHASE ----------------------
    void ChasePlayer()
    {
        if (!player) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(player.position.x, currentPos.y, player.position.z);

        Vector3 toTarget = targetPos - currentPos;
        float distance = toTarget.magnitude;

        if (distance <= stopDistance)
        {
            if (anim)
            {
                anim.SetBool(isChasingParam, false);
                anim.SetFloat(speedParam, 0f);
            }
            return;
        }

        Vector3 dir = toTarget / Mathf.Max(distance, 0.001f);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move toward the player, but stop at walls
        MoveWithCollision(targetPos, chaseSpeed);
    }

    // ---------------------- PATROL ----------------------
    void Patrol()
    {
        if (!HasPatrolPath)
        {
            // no path: just idle
            if (anim)
            {
                anim.SetBool(isChasingParam, false);
                anim.SetFloat(speedParam, 0f);
            }
            return;
        }

        Transform patrolTarget = patrolPoints[currentPatrolIndex];

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(patrolTarget.position.x, baseY, patrolTarget.position.z);

        Vector3 toTarget = targetPos - currentPos;
        float sqrDist = toTarget.sqrMagnitude;

        // reached this point → switch to next
        if (sqrDist < patrolPointTolerance * patrolPointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            return;
        }

        Vector3 dir = toTarget.normalized;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move toward the waypoint, but stop at walls
        MoveWithCollision(targetPos, patrolSpeed);

        // check if we've become stuck while patrolling
        CheckStuckOnPatrol();
    }

    // pick nearest patrol point when we stop chasing / teleport
    void SetNearestPatrolPoint()
    {
        if (!HasPatrolPath) return;

        Vector3 currentPos = transform.position;
        Vector3 origin = currentPos + Vector3.up * collisionHeight;

        float bestSqr = float.MaxValue;
        int bestIndex = currentPatrolIndex;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Vector3 p = patrolPoints[i].position;
            p.y = currentPos.y;

            Vector3 toPoint = p - currentPos;
            float dist = toPoint.magnitude;
            if (dist <= 0.01f)
                continue;

            // optional: skip points that are clearly behind a wall
            bool blocked = Physics.SphereCast(
                origin,
                collisionRadius,
                toPoint.normalized,
                out RaycastHit hit,
                dist,
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );

            if (blocked)
                continue;

            float sqr = toPoint.sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                bestIndex = i;
            }
        }

        currentPatrolIndex = bestIndex;
    }

    // if stuck too long while patrolling, teleport to patrol point
    void CheckStuckOnPatrol()
    {
        float sqrMoved = (transform.position - lastPatrolPos).sqrMagnitude;

        if (sqrMoved < stuckMoveThreshold * stuckMoveThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTeleportTime && HasPatrolPath)
            {
                SetNearestPatrolPoint();

                Vector3 tp = patrolPoints[currentPatrolIndex].position;
                tp.y = baseY;
                transform.position = tp;

                Debug.Log("EnemyPatrol: was stuck, teleported to patrol point " + currentPatrolIndex);

                stuckTimer = 0f;
                lastPatrolPos = transform.position;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPatrolPos = transform.position;
        }
    }
}
