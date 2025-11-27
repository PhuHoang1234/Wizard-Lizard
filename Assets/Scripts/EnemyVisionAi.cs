using System.Collections;
using UnityEngine;

public class EnemyVisionAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // drag Player here
    public LayerMask obstacleMask;    // barrels, walls, pillar etc.

    [Header("Vision")]
    public float viewDistance = 12f;
    [Range(0f, 180f)]
    public float viewAngle = 45f;     // cone angle (total)

    [Header("Detection Times")]
    public float alertTime = 0.5f;    // see player this long -> ALERT
    public float timeToCatch = 1.2f;  // see player this long -> CAUGHT
    public float loseAlertAfter = 1.5f; // hidden this long -> calm down

    [Header("Scanning")]
    public float rotateSpeed = 90f;   // normal scan speed (deg/sec)
    public float minPause = 0.3f;
    public float maxPause = 1.2f;

    [Header("Alert Behaviour")]
    public float alertTurnSpeed = 45f; // how fast to slowly track the player

    bool isChasing = false;   // final “caught” state
    bool isAlerted = false;   // currently tracking you

    float visibleTimer = 0f;  // how long we’ve continuously seen the player
    float hiddenTimer = 0f;   // how long we’ve continuously NOT seen the player

    void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    void Update()
    {
        if (!player) return;

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

        if (!isChasing)
        {
            // 1) Enter alert if we’ve seen the player long enough
            if (!isAlerted && visibleTimer >= alertTime)
            {
                isAlerted = true;
            }

            // 2) If we stay in view even longer -> caught
            if (isAlerted && visibleTimer >= timeToCatch)
            {
                isChasing = true;
                OnPlayerCaught();
            }

            // 3) If we were alert but lost sight for a while -> calm down
            if (isAlerted && hiddenTimer >= loseAlertAfter)
            {
                isAlerted = false;
            }
        }

        // While alert (but not fully caught) slowly track towards the player
        if (isAlerted && !isChasing)
        {
            RotateSlowlyTowardPlayer();
        }
    }

    IEnumerator ScanRoutine()
    {
        while (!isChasing)
        {
            bool doFastSweep = Random.value < 0.25f; // 25% chance

            if (doFastSweep)
            {
                float sweepSign = Random.value < 0.5f ? -1f : 1f;
                float sweepAngle = sweepSign * 90f;
                float originalYaw = transform.eulerAngles.y;
                float targetYaw = originalYaw + sweepAngle;
                Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

                float fastSpeed = rotateSpeed * 2.5f;

                while (!isChasing && !isAlerted &&
                       Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        fastSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                float targetYaw = Random.Range(0f, 360f);
                Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

                while (!isChasing && !isAlerted &&
                       Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        rotateSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                float pauseTime = Random.Range(minPause, maxPause);
                float t = 0f;
                while (!isChasing && !isAlerted && t < pauseTime)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }

            // loop continues while !isChasing
        }
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

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;
        if (distance > viewDistance) return false;

        Vector3 dir = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                            dir,
                            distance,
                            obstacleMask))
        {
            return false;
        }

        return true;
    }

    void OnPlayerCaught()
    {
        Debug.Log("Player spotted – you are cooked!");
        // TODO: show lose panel / reload scene here.
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position;

        Vector3 leftDir = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawLine(origin, origin + leftDir * viewDistance);
        Gizmos.DrawLine(origin, origin + rightDir * viewDistance);
    }
}
