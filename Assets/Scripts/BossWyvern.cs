using System.Collections;
using UnityEngine;

public class BossWyvernSimpleAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // drag Wizard here (or auto-find by tag)
    public LayerMask obstacleMask = ~0;   // walls / pillars that block vision
    public Animator anim;                 // optional

    [Header("Audio")]
    public AudioSource roarSource;        // optional roar sound
    public bool roarOnChaseStart = true;  // play roar when he starts chasing

    [Header("Vision")]
    public float viewDistance = 20f;      // how far the boss can see
    [Range(0f, 180f)]
    public float viewAngle = 60f;         // cone angle in degrees
    public float alertTime = 0.4f;        // see this long -> ALERT
    public float timeToChase = 0.8f;      // see this long -> CHASE
    public float loseAlertAfter = 1.5f;   // lose sight this long -> calm down

    [Header("Scanning (idle spin)")]
    public float scanRotateSpeed = 90f;   // deg/sec during idle scanning
    public float minPause = 0.3f;
    public float maxPause = 1.2f;

    [Header("Chase Movement")]
    public float moveSpeed = 7f;          // run speed towards player
    public float chaseTurnSpeed = 360f;   // turning speed while chasing
    public float alertTurnSpeed = 120f;   // turning speed while just alert

    [Header("Be controlled")]
    public float beControlledTime = 2.0f;
    float beControlledTimer = 0.0f;
    bool isControlled = false;

    // internal state
    bool isAlerted = false;
    bool isChasing = false;
    float visibleTimer = 0f;
    float hiddenTimer = 0f;
    Coroutine scanRoutine;
    PlayerController3D playerController;
    Transform chaseTarget;


    void Awake()
    {
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!anim)
            anim = GetComponent<Animator>();

        playerController = player.GetComponent<PlayerController3D>();

    }

    // This script is disabled at start.
    // BossIntroTrigger turns it ON after the cutscene.
    void OnEnable()
    {
        isAlerted = false;
        isChasing = false;
        visibleTimer = 0f;
        hiddenTimer = 0f;

        if (scanRoutine != null) StopCoroutine(scanRoutine);
        scanRoutine = StartCoroutine(ScanRoutine());

        if (anim)
        {
            anim.SetBool("IsChasing", false);
            anim.SetFloat("Speed", 0f);
        }
    }

    void OnDisable()
    {
        if (scanRoutine != null) StopCoroutine(scanRoutine);
    }

    void Update()
    {
        if (!player) return;

        if (isControlled)
        {
            beControlledTimer -= Time.deltaTime;

            if (beControlledTimer <= 0f)
            {
                EndControlled();
            }
            return;
        }

        if (isChasing)
        {
            DecideTarget();
            ChasePlayer();
            return;
        }

        bool canSee = CanSeePlayer();

        if (canSee)
        {
            visibleTimer += Time.deltaTime;
            hiddenTimer = 0f;
        }
        else
        {
            hiddenTimer += Time.deltaTime;
            visibleTimer = 0f;
        }

        // enter alert state
        if (!isAlerted && visibleTimer >= alertTime)
            isAlerted = true;

        // after being seen long enough → chase
        if (isAlerted && visibleTimer >= timeToChase)
        {
            isChasing = true;
            if (scanRoutine != null) StopCoroutine(scanRoutine);
            OnChaseStart();
        }

        // if we were alert but lost sight long enough → calm down
        if (isAlerted && hiddenTimer >= loseAlertAfter)
            isAlerted = false;

        // while alert (but not yet chasing), slowly track towards player
        if (isAlerted && !isChasing)
            RotateSlowlyTowardPlayer();
    }

    // ───────────────────── Scanning / idle spin ─────────────────────
    IEnumerator ScanRoutine()
    {
        // Just spins in random directions while not chasing.
        while (!isChasing)
        {
            float targetYaw = Random.Range(0f, 360f);
            Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

            while (!isChasing &&
                   Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    scanRotateSpeed * Time.deltaTime
                );
                yield return null;
            }

            float pauseTime = Random.Range(minPause, maxPause);
            float t = 0f;
            while (!isChasing && t < pauseTime)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

    // ───────────────────── Chase ─────────────────────
    void OnChaseStart()
    {
        if (anim)
        {
            anim.SetBool("IsChasing", true);  // optional
            anim.SetFloat("Speed", moveSpeed);
        }

        // Optional extra roar when he aggroes
        if (roarOnChaseStart && roarSource != null)
        {
            // use PlayOneShot or just Play depending on how you set it up
            if (roarSource.clip != null)
                roarSource.Play();
        }
    }

    void ChasePlayer()
    {
        Vector3 toTarget = chaseTarget.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        Vector3 dir = toTarget.normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            chaseTurnSpeed * Time.deltaTime
        );

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }

    void RotateSlowlyTowardPlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            alertTurnSpeed * Time.deltaTime
        );
    }

    void DecideTarget()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Tail");

        foreach (var obj in objs)
        {
            chaseTarget = obj.transform;
            return;
        }

        chaseTarget = player;
    }

    // ───────────────────── Vision ─────────────────────
    bool CanSeePlayer()
    {
        if (playerController.powerManager.isInvisible) return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f; // eye height
        Vector3 toPlayer = player.position - origin;
        float distance = toPlayer.magnitude;
        if (distance > viewDistance) return false;

        Vector3 dir = toPlayer.normalized;
        dir.y = 0f;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return false;

        // Raycast to see if a wall is blocking
        if (Physics.Raycast(origin, dir, distance, obstacleMask))
            return false;

        return true;
    }

    // ───────────────────── Kill player on touch ─────────────────────
    void KillPlayer(GameObject playerObj)
    {
        // Your player already has CharacterDeath, so this is safe.
        CharacterDeath cd = playerObj.GetComponent<CharacterDeath>();
        if (cd != null)
        {
            cd.Die();
            return;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isChasing) return;
        if (other.gameObject.CompareTag("Player"))
            KillPlayer(other.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isChasing) return;
        if (other.CompareTag("Player"))
            KillPlayer(other.gameObject);
    }

    // (optional) vision gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 leftDir = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawLine(origin, origin + leftDir * viewDistance);
        Gizmos.DrawLine(origin, origin + rightDir * viewDistance);
    }

    // ───────────────────── Take lightning cast and be controlled ─────────────────────
    public void BeControlled()
    {
        beControlledTimer = beControlledTime;
        isControlled = true;
    }

    public void EndControlled()
    {
        isControlled = false;
    }
}
