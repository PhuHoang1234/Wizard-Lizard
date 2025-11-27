using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWyvern : MonoBehaviour
{
    [Header("Arena Patrol Bounds")]
    public float patrolXMin = -20f, patrolXMax = 20f;
    public float patrolZMin = -15f, patrolZMax = 15f;
    public float patrolSpeed = 3f;

    [Header("Chase/Aerial")]
    public Transform player;
    public float aggroRange = 30f;
    public float groundChaseSpeed = 6f;
    public float flySpeed = 8f;
    public float diveSpeed = 15f;
    public float flyHeight = 15f;
    public float rotationSpeed = 5f;

    [Header("Dive Crush Attack")]
    public float diveCooldown = 8f;
    public float crushDamage = 250f;  // Reduced: ~25% of 1000 HP - NOT instant kill!
    public float crushRadius = 6f;
    public LayerMask playerLayer = -1;

    [Header("Head Focus")]
    public Transform headBone;
    public float headLookSpeed = 3f;
    public float headLookWeight = 1f;

    [Header("Boss Phases")]
    public float maxHealth = 1000f;
    [SerializeField] private float health;

    public GameObject fireballPrefab;
    public Transform fireballSpawn;

    private Animator anim;
    private int phase = 1;
    private Vector3 patrolTarget;
    private bool isFlying = false;
    private float patrolTime = 0f;
    private bool diving = false;
    private Vector3 diveTarget;
    private float nextDiveTime;
    private Transform currentLookTarget;
    private PlayerHealth playerHealth;  // Reference to player's health

    void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        playerHealth = player ? player.GetComponent<PlayerHealth>() : null;

        if (headBone == null)
        {
            headBone = FindHeadBone(transform);
            Debug.Log(headBone ? $"Head bone auto-found: {headBone.name}" : "WARNING: Assign Head Bone!");
        }

        nextDiveTime = Time.time + Random.Range(5f, 10f);
        SetRandomPatrolTarget();
    }

    void Update()
    {
        // Check if player is alive
        bool playerAlive = playerHealth && playerHealth.IsAlive();

        float distToPlayer = playerAlive ? Vector3.Distance(transform.position, player.position) : 999f;
        patrolTime += Time.deltaTime;
        currentLookTarget = playerAlive ? player : null;

        if (playerAlive && distToPlayer < aggroRange)
        {
            // Aggro only if player alive
            if (phase == 1 && health > maxHealth * 0.75f)
            {
                GroundChase();
            }
            else if (phase == 2 || health <= maxHealth * 0.75f)
            {
                AerialPhase();
                if (Time.time > nextDiveTime)
                {
                    DiveAttack();
                    nextDiveTime = Time.time + (phase == 3 ? Random.Range(4f, 6f) : Random.Range(7f, 12f));
                }
            }
            else
            {
                EnragePhase();
            }
        }
        else
        {
            // Patrol or idle if player dead/out of range
            PatrolArena();
        }

        // Phase transitions (only if player alive)
        if (playerAlive && health <= maxHealth * 0.75f && phase < 2)
        {
            phase = 2;
            TakeOff();
        }
        if (playerAlive && health <= maxHealth * 0.25f) phase = 3;
    }

    void LateUpdate()
    {
        if (headBone && currentLookTarget)
        {
            Vector3 lookDir = (currentLookTarget.position - headBone.position).normalized;
            Quaternion targetHeadRot = Quaternion.LookRotation(lookDir);
            headBone.rotation = Quaternion.Slerp(headBone.rotation, targetHeadRot, headLookSpeed * Time.deltaTime * headLookWeight);
        }
    }

    void GroundChase()
    {
        anim.SetFloat("Speed", groundChaseSpeed);
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * groundChaseSpeed * Time.deltaTime, Space.Self);
    }

    void AerialPhase()
    {
        anim.SetBool("IsFlying", true);
        anim.SetFloat("FlySpeed", diving ? diveSpeed : flySpeed);

        Vector3 target = diving ? diveTarget : (player.position + Vector3.up * flyHeight);
        Vector3 dir = (target - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * (diving ? diveSpeed : flySpeed) * Time.deltaTime, Space.Self);

        if (diving && Vector3.Distance(transform.position, diveTarget) < 4f)
        {
            diving = false;
            CrushImpact(diveTarget);
            patrolTarget = transform.position + Vector3.up * 25f + (Vector3.right + Vector3.forward) * Random.Range(10f, 20f);
        }
    }

    void EnragePhase()
    {
        DiveAttack();
    }

    void PatrolArena()
    {
        anim.SetBool("IsFlying", false);
        anim.SetFloat("Speed", patrolSpeed);
        anim.SetFloat("FlySpeed", 0);

        if (Vector3.Distance(transform.position, patrolTarget) < 3f || patrolTime > 12f)
        {
            SetRandomPatrolTarget();
        }

        Vector3 dir = (patrolTarget - transform.position).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime, Space.Self);
    }

    void SetRandomPatrolTarget()
    {
        float x = Random.Range(patrolXMin, patrolXMax);
        float z = Random.Range(patrolZMin, patrolZMax);
        patrolTarget = new Vector3(x, 0f, z);
        patrolTime = 0f;
    }

    void DiveAttack()
    {
        diving = true;
        diveTarget = player.position;
        diveTarget.y = 0f;
        anim.SetFloat("FlySpeed", diveSpeed);
    }

    void CrushImpact(Vector3 impactPos)
    {
        // Only damage if player alive & in radius
        if (playerHealth && playerHealth.IsAlive())
        {
            Collider[] hits = Physics.OverlapSphere(impactPos, crushRadius, playerLayer);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    playerHealth.TakeDamage(crushDamage);
                    Debug.Log($"Wyvern CRUSH HIT! Player HP: {playerHealth.currentHealth}/{playerHealth.maxHealth}");
                }
            }
        }
        // Effects: Particles/shake/roar
    }

    Transform FindHeadBone(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains("head") || child.name.ToLower().Contains("neck")) return child;
            Transform found = FindHeadBone(child);
            if (found) return found;
        }
        return null;
    }

    public void TakeOff() { anim.SetTrigger("TakeOff"); isFlying = true; }
    public void SpawnFireball()
    {
        if (fireballPrefab && fireballSpawn)
        {
            GameObject fb = Instantiate(fireballPrefab, fireballSpawn.position, fireballSpawn.rotation);
            fb.GetComponent<Rigidbody>().AddForce(transform.forward * 25f + Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0) Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");
        // Boss defeated logic
    }
}