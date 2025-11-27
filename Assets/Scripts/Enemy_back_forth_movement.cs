using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2.0f;
    public float reachDist = 0.1f;
    public float turnSpeedDegPerSec = 360f;   // how fast to rotate

    int i = 0;      // current target index
    int dir = 1;    // 1 = forward, -1 = backward

    void Update()
    {
        if (points == null || points.Length == 0) return;

        // Current target point
        Vector3 targetPos = points[i].position;

        // --------- ROTATE TOWARDS TARGET ---------
        // Direction to target (ignore vertical for Y-rotation only)
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeedDegPerSec * Time.deltaTime
            );
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
        if (Vector3.Distance(transform.position, targetPos) <= reachDist)
        {
            if (i == 0) dir = 1;
            else if (i == points.Length - 1) dir = -1;

            i += dir;
        }
    }
}
