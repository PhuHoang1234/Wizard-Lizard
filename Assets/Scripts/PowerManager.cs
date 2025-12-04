using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [Header("Lightning")]
    public float lightningRange = 20f;        
    public float lightningCooldown = 3f;      
    public float aimAssistAngle = 30f;       
    public LineRenderer lightningLine;        
    public float lineShowTime = 0.08f;        
    public float damage = 0f;

    [Header("Invisible")]
    public float invisibleCooldown = 5f;
    public float invisibleDuration = 15.0f;
    public bool isInvisible = false;
    Transform invisibilityCaster;

    [Header("Distraction")]
    public float distractionCooldown = 10f;
    public float distractionDuration = 20f;
    public GameObject tailPrefab;
    public Follow_player mainCamera;
    bool isControllingTail = false;
    GameObject tail;
    Transform distractionCaster;


    float lightningTimer = 0f;
    float invisibleTimer = 0f;
    float distractionTimer = 0f;
    float invisibleDurationTimer = 0f;
    float distractionDurationTimer = 0f;

    [Header("Unlocks")]
    public bool hasLightning = false;
    public bool hasInvisible = false;
    public bool hasDistraction = true;

    void Start()
    {
        if (lightningLine != null)
            lightningLine.enabled = false;
    }

    void Update()
    {
        // lightning timers
        if (lightningTimer > 0f)
            lightningTimer -= Time.deltaTime;

        // invisible timers
        if (isInvisible)
        {
            invisibleDurationTimer -= Time.deltaTime;
            if (invisibleDurationTimer <= 0)
            {
                EndInvisibility();
            }
        }

        if (invisibleTimer > 0f)
            invisibleTimer -= Time.deltaTime;

        // distraction timers
        if(tail != null)
        {
            distractionDurationTimer -= Time.deltaTime;

            if (distractionDurationTimer <= 0)
            {
                EndDistraction(false);
            }
        }

        if (distractionTimer > 0f)
            distractionTimer -= Time.deltaTime;
    }

    // ------------ Lightning -------------
    public bool CanCastLightning()
    {
        return hasLightning && lightningTimer <= 0f;
    }

    public bool TryCastLightning(Transform caster)
    {
        if (!CanCastLightning())
            return false;

        lightningTimer = lightningCooldown;
        CastLightning(caster);
        return true;
    }

    public void UnlockLightning()
    {
        hasLightning = true;
        Debug.Log("Lightning unlocked!");
    }

    void CastLightning(Transform caster)
    {
        Vector3 origin = caster.position;
        Vector3 direction = caster.forward;

        float bestAngle = aimAssistAngle;
        Transform bestTarget = null;

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

            HandleEnemyKilled(bestTarget);
            return;
        }

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


        var patrol = enemyRoot.GetComponentInChildren<EnemyPatrol>();
        if (patrol) patrol.enabled = false;

        var rb = enemyRoot.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }


        Collider[] cols = enemyRoot.GetComponentsInChildren<Collider>();
        foreach (var c in cols)
        {
            c.enabled = false;
        }

        enemyRoot.tag = "Untagged";


        Animator enemyAnim = enemyRoot.GetComponentInChildren<Animator>();
        if (enemyAnim != null)
        {
            enemyAnim.SetTrigger("Die");   
        }
        else
        {
            Debug.LogWarning("No Animator found on enemy " + enemyRoot.name);
        }

        Destroy(enemyRoot.gameObject, 2.5f);
    }


    // ------------ Invisible -------------

    public bool CanCastInvisible()
    {
        return hasInvisible && !isInvisible && invisibleTimer <= 0f;
    }

    public bool TryCastInvisible(Transform caster)
    {
        if (!CanCastInvisible())
            return false;

        invisibleDurationTimer = invisibleDuration;
        CastInvisible(caster);
        return true;
    }

    public void UnlockInvisible()
    {
        hasInvisible = true;
        Debug.Log("Invisible unlocked!");
    }

    void CastInvisible(Transform caster)
    {
        if (isInvisible) return;

        isInvisible = true;
        invisibleDurationTimer = invisibleDuration;
        this.invisibilityCaster = caster;

        InvisibilityUtility.SetInvisible(caster, 0.0f);

        Debug.Log("Become invisible (transparent)");
    }

    void EndInvisibility()
    {
        if (invisibilityCaster == null) return;
        isInvisible = false;

        InvisibilityUtility.SetVisible(invisibilityCaster);

        invisibilityCaster = null;
        InvisibleStartCooldown();
    }

    void InvisibleStartCooldown()
    {
        invisibleTimer = invisibleCooldown;
    }


    // ------------ Distraction -------------

    public bool CanCastDistraction()
    {
        return hasDistraction && tail == null && !isControllingTail && distractionTimer <= 0f;
    }

    public bool TryCastDistraction(Transform caster, KeyCode releaseKey)
    {
        if (!CanCastDistraction())
            return false;

        distractionDurationTimer = distractionDuration;
        CastDistraction(caster, releaseKey);
        return true;
    }

    public void UnlockDistraction()
    {
        hasDistraction = true;
        Debug.Log("Distraction unlocked!");
    }

    void CastDistraction(Transform caster, KeyCode releaseKey)
    {
        if (tail != null) return;

        Vector3 spawnPos = caster.position + caster.forward * 3f;
        tail = Instantiate(tailPrefab, spawnPos, Quaternion.identity);
        tail.SetActive(true);
        TailObject to = tail.GetComponent<TailObject>();
        to.powerManager = caster.GetComponent<PlayerController3D>().powerManager;
        to.releaseKey = releaseKey;

        PlayerController3D player = caster.GetComponent<PlayerController3D>();
        player.canControl = false;
        to.canControl = true;
        mainCamera.SetTarget(tail.transform);

        distractionCaster = caster;

        Debug.Log("Distraction");
    }

    public void TailReleased()
    {
        PlayerCanBeControlledAgain();
    }

    public void TailPicked(bool isPlayerPickTail)
    {
        if (tail == null) return;

        EndDistraction(isPlayerPickTail);
    }

    void EndDistraction(bool isPlayerPickTail)
    {
        PlayerCanBeControlledAgain();
        tail = null;
        distractionCaster = null;

        DistractionStartCooldown(isPlayerPickTail);
    }

    void PlayerCanBeControlledAgain()
    {
        PlayerController3D player = distractionCaster.GetComponent<PlayerController3D>();
        player.canControl = true;

        if(tail != null) tail.GetComponent<TailObject>().canControl = false;

        mainCamera.SetTarget();
    }

    void DistractionStartCooldown(bool isPlayerPickTail)
    {
        distractionTimer = isPlayerPickTail? 0 : distractionCooldown;
    }

}
