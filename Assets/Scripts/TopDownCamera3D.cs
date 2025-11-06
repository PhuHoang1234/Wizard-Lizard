using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCamera3D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Rigidbody targetRb; // optional, for look-ahead

    [Header("Framing")]
    [Tooltip("Dead zone width (X) and depth (Z) in world units.")]
    public Vector2 deadZone = new Vector2(1.5f, 1.0f);

    [Tooltip("Time to smooth camera movement.")]
    public float smoothTime = 0.18f;

    [Tooltip("Max look-ahead distance along target velocity.")]
    public float lookAheadDistance = 0.8f;

    [Tooltip("Scales look-ahead by target speed.")]
    public float lookAheadSpeedFactor = 0.25f;

    [Header("Height/Projection")]
    [Tooltip("Fixed camera height above the ground (Y).")]
    public float cameraHeight = 20f;

    Vector3 velocity;

    void Reset()
    {
        var cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        Vector3 p = transform.position;
        p.y = cameraHeight;
        transform.position = p;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 camPos = transform.position;
        Vector3 targetXZ = new Vector3(target.position.x, camPos.y, target.position.z);

        // Compute offset needed to push the target out of the dead zone (in XZ)
        Vector2 deltaXZ = new Vector2(target.position.x - camPos.x, target.position.z - camPos.z);
        Vector2 offsetXZ = Vector2.zero;

        if (Mathf.Abs(deltaXZ.x) > deadZone.x)
            offsetXZ.x = deltaXZ.x - Mathf.Sign(deltaXZ.x) * deadZone.x;

        if (Mathf.Abs(deltaXZ.y) > deadZone.y)
            offsetXZ.y = deltaXZ.y - Mathf.Sign(deltaXZ.y) * deadZone.y;

        Vector3 desired = new Vector3(
            camPos.x + offsetXZ.x,
            cameraHeight,
            camPos.z + offsetXZ.y
        );

        // Velocity-based look-ahead (XZ)
        if (targetRb)
        {
            Vector3 v = targetRb.linearVelocity;
            Vector2 vXZ = new Vector2(v.x, v.z);
            if (vXZ.sqrMagnitude > 0.0001f)
            {
                Vector2 la = vXZ.normalized * Mathf.Min(vXZ.magnitude * lookAheadSpeedFactor, lookAheadDistance);
                desired += new Vector3(la.x, 0f, la.y);
            }
        }

        transform.position = Vector3.SmoothDamp(camPos, desired, ref velocity, smoothTime);
    }
}

