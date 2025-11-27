using UnityEngine;

public class DetectManager : MonoBehaviour
{
    public static bool DetectTarget(GameObject target, Transform self, float visionDistance, float visionAngle, bool isTargetHidding)
    {
        if (target == null) return false;

        Vector3 dirToPlayer = target.transform.position - self.position;
        float distance = dirToPlayer.magnitude;

        // Check distance and angle
        if (distance < visionDistance)
        {
            float angle = Vector3.Angle(self.forward, dirToPlayer);

            if (angle < visionAngle / 2f)
            {
                // Raycast to confirm visibility
                if (Physics.Raycast(self.position + Vector3.up * 0.5f, dirToPlayer.normalized, out RaycastHit hit, visionDistance))
                {
                    if (hit.collider.CompareTag("Player") && !isTargetHidding)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
