using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [Header("Lightning")]
    public float lightningRange = 20f;        // how far the ray can go
    public float lightningCooldown = 3f;      // seconds between casts
    public float aimAssistAngle = 30f;        // degrees for auto-aim cone
    public LineRenderer lightningLine;        // drag your Line Renderer here
    public float lineShowTime = 0.08f;        // how long the line is visible
    public float damage = 0f;                 // optional, not used yet

    // internal timer for cooldown
    float lightningTimer = 0f;

    void Update()
    {
        // count down the cooldown
        if (lightningTimer > 0f)
            lightningTimer -= Time.deltaTime;
    }

    // Can we cast right now?
    public bool CanCastLightning()
    {
        return lightningTimer <= 0f;
    }

    // Called by the player – returns true if the cast actually happened
    public bool TryCastLightning(Transform caster)
    {
        if (!CanCastLightning())
            return false;

        lightningTimer = lightningCooldown;
        CastLightning(caster);
        return true;
    }

    // ---------- main lightning logic ----------
    void CastLightning(Transform caster)
    {
        Vector3 origin = caster.position;
        Vector3 direction = caster.forward;

        float bestAngle = aimAssistAngle;
        Transform bestTarget = null;

        // 1) search for enemies in a sphere
        Collider[] hits = Physics.OverlapSphere(origin, lightningRange);
        foreach (var col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;

            Vector3 dirToEnemy = (col.transform.position - origin).normalized;
            float angle = Vector3.Angle(caster.forward, dirToEnemy);

            if (angle < bestAngle)
            {
                // simple line-of-sight check
                if (Physics.Raycast(origin, dirToEnemy, out RaycastHit rh, lightningRange))
                {
                    if (rh.collider.transform == col.transform)
                    {
                        bestAngle = angle;
                        bestTarget = col.transform;
                    }
                }
            }
        }

        // 2) if we found something, aim at it
        if (bestTarget != null)
        {
            direction = (bestTarget.position - origin).normalized;
        }

        // 3) final raycast and draw the line
        if (Physics.Raycast(origin, direction, out RaycastHit finalHit, lightningRange))
        {
            DrawLightning(origin, finalHit.point);

            // later you can damage an enemy here if you want:
            // var enemy = finalHit.collider.GetComponent<EnemyHealth>();
            // if (enemy) enemy.TakeDamage(damage);
        }
        else
        {
            DrawLightning(origin, origin + direction * lightningRange);
        }
    }

    void DrawLightning(Vector3 start, Vector3 end)
    {
        if (!lightningLine) return;

        lightningLine.enabled = true;
        lightningLine.positionCount = 2;
        lightningLine.SetPosition(0, start);
        lightningLine.SetPosition(1, end);

        CancelInvoke(nameof(HideLightning));
        Invoke(nameof(HideLightning), lineShowTime);
    }

    void HideLightning()
    {
        if (lightningLine)
            lightningLine.enabled = false;
    }
}
