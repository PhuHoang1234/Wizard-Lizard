using UnityEngine;

public class Patrol : MonoBehaviour
{
    

    public Transform[] points;
    public float speed = 2.0f;
    public float reachDist = 0.1f;
    public float turnSpeedDegPerSec = 360f;   // how fast to rotate

    int i = 0, dir = 1;
    float pendingTurn = 0f;                   // degrees left to rotate

    void Update()
    {
        if (points == null || points.Length == 0) return;

        // --- turning phase (runs for a few frames until 180° is done) ---
        if (pendingTurn > 0f)
        {
            float step = Mathf.Min(turnSpeedDegPerSec * Time.deltaTime, pendingTurn);
            transform.Rotate(Vector3.up, step);    // for 2D use Vector3.forward
            pendingTurn -= step;
            return;                                 // pause movement while turning
        }

        // --- movement (your original logic) ---
        var target = points[i].position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= reachDist)
        {
            if (i == 0) dir = 1;
            else if (i == points.Length - 1) dir = -1;
            i += dir;                               // ping-pong

            pendingTurn = 180f;                     // queue a 180° spin at the apex
        }
    }
}
