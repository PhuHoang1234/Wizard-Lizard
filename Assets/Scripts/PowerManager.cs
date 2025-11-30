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

    [Header("Unlocks")]
    public bool hasLightning = false;         // starts locked

    void Start()
    {
        // hide line at start
        if (lightningLine != null)
            lightningLine.enabled = false;
    }

    void Update()
    {
        if (lightningTimer > 0f)
            lightningTimer -= Time.deltaTime;
    }

    // Can we cast right now?
    public bool CanCastLightning()
    {
        // must be unlocked AND off cooldown
        return hasLightning && lightningTimer <= 0f;
    }

    // Called by PlayerController3D when you press castKey (F)
    public bool TryCastLightning(Transform caster)
    {
        if (!CanCastLightning())
            return false;

        lightningTimer = lightningCooldown;
        CastLightning(caster);
        return true;
    }

    // 🔓 called from pickup trigger
    public void UnlockLightning()
    {
        hasLightning = true;
        Debug.Log("Lightning unlocked!");
    }

    // ---------- main lightning logic ----------
    void CastLightning(Transform caster)
    {
        Vector3 origin = caster.position;
        Vector3 direction = caster.forward;

        float bestAngle = aimAssistAngle;
        Transform bestTarget = null;

        // 1) search for enemies in a sphere around the player
        Collider[] hits = Physics.OverlapSphere(origin, lightningRange);
        foreach (var col in hits)
        {
            Transform root = col.transform.root;
            if (!col.CompareTag("Enemy") && !root.CompareTag("Enemy"))
                continue;

            Vector3 dirToEnemy = (root.position - origin);
            float angle = Vector3.Angle(caster.forward, dirToEnemy);

            if (angle < bestAngle)
            {
                if (Physics.Raycast(origin, dirToEnemy.normalized, out RaycastHit rh, lightningRange))
                {
                    if (rh.collider.transform.root == root)
                    {
                        bestAngle = angle;
                        bestTarget = root;
                    }
                }
            }
        }

        // 2) if we found an enemy, aim at it and KILL it
        if (bestTarget != null)
        {
            Vector3 toTarget = bestTarget.position - origin;
            direction = toTarget.normalized;

            Vector3 endPos = bestTarget.position;
            if (Physics.Raycast(origin, direction, out RaycastHit finalHit, lightningRange))
            {
                endPos = finalHit.point;
            }

            DrawLightning(origin, endPos);

            // NEW:
            HandleEnemyKilled(bestTarget);
            return;
        }

        // 3) no enemy in cone: just shoot forward and draw line
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, lightningRange))
        {
            DrawLightning(origin, hitInfo.point);
        }
        else
        {
            DrawLightning(origin, origin + direction * lightningRange);
        }
    }

    void DrawLightning(Vector3 start, Vector3 end)
    {
        if (!lightningLine)
        {
            Debug.LogWarning("DrawLightning: lightningLine is not assigned!");
            return;
        }

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
    void HandleEnemyKilled(Transform enemyRoot)
    {
        if (enemyRoot == null) return;

        Debug.Log("HandleEnemyKilled on " + enemyRoot.name);

        // 1) stop their AI / movement
        var patrol = enemyRoot.GetComponentInChildren<EnemyPatrol>();
        if (patrol) patrol.enabled = false;

        var rb = enemyRoot.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // ❗ 2) disable goblin colliders so he can no longer kill the player
        Collider[] cols = enemyRoot.GetComponentsInChildren<Collider>();
        foreach (var c in cols)
        {
            c.enabled = false;
        }
        // (optional extra safety)
        enemyRoot.tag = "Untagged";

        // 3) play death animation on their Animator
        Animator enemyAnim = enemyRoot.GetComponentInChildren<Animator>();
        if (enemyAnim != null)
        {
            enemyAnim.SetTrigger("Die");   // trigger must be called "Die"
        }
        else
        {
            Debug.LogWarning("No Animator found on enemy " + enemyRoot.name);
        }

        // 4) delete goblin after delay so anim can finish
        Destroy(enemyRoot.gameObject, 2.5f);
    }


}
