using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public abstract class EnemyBase : MonoBehaviour
{
    // === Basic Settings ===
    public Transform[] patrolPoints;            // Patrol route points
    public float visionRange = 10.0f;           // Vision range (unused)
    public float visionAngle = 60.0f;           // Field of view angle
    public float visionDistance = 8.0f;         // Max visible distance
    public float loseDistance = 12.0f;          // Distance to lose target
    public float moveSpeed = 2.0f;              // Normal movement speed
    public float chaseSpeed = 4.0f;             // Speed while chasing
    public float loseSightTime = 2.0f;          // Time before giving up after losing sight
    public float maxChaseTime = 2.0f;           // Maximum chase duration
    public float hearingDistance = 0.0f;        // Hearing radius for sound detection
    public float waitTime = 2.0f;               // Wait time at patrol points

    // === Health ===
    public float maxHealth = 10.0f;
    private float currentHealth = 0.0f;

    // === Components ===
    protected NavMeshAgent agent;
    public GameManager manager;
    protected Animator animator;                   // Animator reference

    // === Patrol and State Control ===
    protected int currentPoint = 0;
    protected bool isBack = false;
    protected bool isChasing = false;
    protected bool CanSeePlayer = false;
    protected float timeSinceLastSeen = 0f;
    protected float chaseTimer = 0f;
    protected GameObject target;

    // === Investigation Logic ===
    protected bool isGoingToInvestigating = false;
    protected bool isInvestigating = false;
    protected bool isTurningToInvestigate = false;
    protected float investigateDelay = 2.0f;
    protected float investigateTimer = 0f;
    protected float lookDelay = 1.0f;
    protected float lookTimer = 0f;
    public float turnSpeed = 5f;

    // === Misc ===
    protected Vector3 targetPos;
    protected bool canBeInterrupt = true;
    protected float idleTimer = 0.0f;
    private bool isDead = false;

    // === States ===
    protected enum State
    {
        Patrol,
        Chase,
        Investigate,
        Idle
    }

    protected enum IdleType
    {
        IdleP,
        Tired,
        BeControlled
    }

    protected State state = State.Patrol;
    protected IdleType idle = IdleType.IdleP;

    // === Animation ===
    protected List<string> animationTypes = new List<string>();
    protected string WALKING = "isWalking";
    protected string RUNNING = "isRunning";
    protected string IDLING = "isIdling";
    protected string INVESTIGATING = "isInvestigating";
    protected string TIRED = "isTired";
    protected string BECONTROLLED = "isCONTROLLED";
    protected string TAKEDAMAGE = "isTakeDamaged";



    // === Initialization ===
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // Get the Animator from the enemy

        SetUpAnimationTypes();

        GoToNextPatrolPoint();
    }


    // === Update Loop ===
    protected virtual void Update()
    {
        if (manager.isGamePaused) return;

        canBeInterrupt = true;

        if (isDead) return;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Investigate:
                Investigate();
                break;
            case State.Idle:
                Idle();
                break;
        }

        DetectPlayer();

    }


    // ===================================
    //           PLAYER DETECTION
    // ===================================
    protected virtual void DetectPlayer()
    {
        if (!canBeInterrupt) return;

        if (DetectManager.DetectTarget(manager.tail, transform, visionDistance, visionAngle, false))
        {
            CanSeePlayer = true;
            agent.isStopped = false;
            target = manager.tail;
            timeSinceLastSeen = 0;

            StartChase();
        }
        else if (DetectManager.DetectTarget(manager.player, transform, visionDistance, visionAngle, manager.powerManager.isPlayerHidding()))
        {
            CanSeePlayer = true;
            agent.isStopped = false;
            target = manager.player;
            timeSinceLastSeen = 0;

            StartChase();
        }

        // Player lost
        timeSinceLastSeen += Time.deltaTime;
        CanSeePlayer = false;
    }


    // === Debug Vision Cone ===
    private void OnDrawGizmosSelected()
    {
        if (agent == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, visionDistance);

        Vector3 forward = transform.forward;
        float halfAngle = visionAngle / 2f;
        Vector3 leftBoundary = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, halfAngle, 0) * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 1f + leftBoundary * visionDistance);
        Gizmos.DrawLine(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 1f + rightBoundary * visionDistance);
    }



    // ===================================
    //              PATROL
    // ===================================
    protected virtual void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            // Reverse direction at end points
            if (currentPoint == 0 || currentPoint == patrolPoints.Length - 1)
            {
                isBack = !isBack;
                Idle(waitTime, IdleType.IdleP);
            }
            else
            {
                GoToNextPatrolPoint();
            }
        }
    }

    private void BackToPatrol()
    {
        agent.isStopped = false;
        state = State.Patrol;
        GoToNextPatrolPoint();
    }

    protected virtual void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        if (isBack && currentPoint != 0)
            SetTargetPos(patrolPoints[--currentPoint].position);
        else if (!isBack && currentPoint != patrolPoints.Length - 1)
            SetTargetPos(patrolPoints[++currentPoint].position);

        PlayAnimation(WALKING);

        agent.SetDestination(targetPos);
    }

    protected void SetTargetPos(Vector3 nextTarget)
    {
        targetPos = nextTarget;
    }


    // ===================================
    //              CHASE
    // ===================================
    protected virtual void StartChase()
    {
        if (!isChasing)
        {
            chaseTimer = maxChaseTime;
            state = State.Chase;
            isInvestigating = false;
            isChasing = true;
            agent.speed = chaseSpeed;
        }
    }

    protected virtual void GiveUpChasing()
    {
        canBeInterrupt = false;
        agent.isStopped = false;
        isChasing = false;
        agent.speed = moveSpeed;
        target = null;
        BackToPatrol();
    }

    protected virtual void Chase()
    {
        if (target.transform == null) return;

        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;
            PlayAnimation(RUNNING);
            agent.SetDestination(target.transform.position);

            if (!CanSeePlayer && timeSinceLastSeen > loseSightTime)
                GiveUpChasing();
            else if (chaseTimer <= 0.0f)
                Idle(2.0f, IdleType.Tired);
        }
    }

    // ===================================
    //            INVESTIGATION
    // ===================================
    protected virtual void Investigate()
    {
        // Rotate toward target before moving
        if (isTurningToInvestigate)
        {
            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(LookAtDir(targetPos, turnSpeed)));
            if (angle < 5f)
            {
                lookTimer += Time.deltaTime;
                if (lookTimer >= lookDelay)
                {
                    isTurningToInvestigate = false;
                    agent.isStopped = false;
                    PlayAnimation(WALKING);
                    agent.SetDestination(targetPos);
                }
            }
            return;
        }

        // Start investigating when reached destination
        if (!agent.pathPending && agent.remainingDistance < 0.2f && !isInvestigating)
            StartInvestigating();

        // Countdown investigation timer
        if (isInvestigating)
        {
            investigateTimer -= Time.deltaTime;
            if (investigateTimer <= 0.0f)
                GiveUpInvestigating();
        }
    }

    private Vector3 LookAtDir(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, speed * 100f * Time.deltaTime);
        }

        return dir;
    }

    public virtual void HearSound(Vector3 soundPos)
    {
        if (!isChasing)
        {
            state = State.Investigate;
            SetTargetPos(soundPos);
            agent.isStopped = true;
            PlayAnimation(IDLING);
            isTurningToInvestigate = true;
            isInvestigating = false;
            lookTimer = 0f;
        }
    }

    private void StartInvestigating()
    {
        isInvestigating = true;
        investigateTimer = investigateDelay;
        PlayAnimation(INVESTIGATING);
    }

    private void GiveUpInvestigating()
    {
        state = State.Patrol;
        isInvestigating = false;
        GoToNextPatrolPoint();
    }


    // ===================================
    //           PLAYER CAPTURE
    // ===================================
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == manager.player)
        {
            OnPlayerCaught();
        }
        else
        {
            target = null;
            GiveUpChasing();
        }
    }

    protected virtual void OnPlayerCaught()
    {
        SceneManager.LoadScene(1);
    }


    // ===================================
    //               IDLE
    // ===================================
    protected virtual void Idle(float time, IdleType idle)
    {
        idleTimer = time;
        this.idle = idle;
        state = State.Idle;
    }

    protected virtual void Idle()
    {
        idleTimer -= Time.deltaTime;

        switch (idle)
        {
            case IdleType.IdleP:
                IdleP();
                break;
            case IdleType.Tired:
                Tired();
                break;
            case IdleType.BeControlled:
                BeControlled();
                break;
        }
    }

    protected virtual void IdleP()
    {
        PlayAnimation(IDLING);
        if (idleTimer <= 0.0f)
            BackToPatrol();
    }

    protected virtual void Tired()
    {
        canBeInterrupt = false;
        agent.isStopped = true;
        if (target != null) LookAtDir(target.transform.position, 1000.0f);
        PlayAnimation(TIRED);

        if (idleTimer <= 0.0f)
            GiveUpChasing();
    }

    protected virtual void BeControlled()
    {
        agent.isStopped = true;
        PlayAnimation(BECONTROLLED);

        if (idleTimer <= 0.0f)
            BackToPatrol();
    }



    // ===================================
    //           DAMAGE & DEATH
    // ===================================
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        PlayAnimation(TAKEDAMAGE);
        if (currentHealth < 0.0f)
        {
            Died();
        }
        else
        {
            Idle(2.0f, IdleType.BeControlled);
        }
    }

    private void Died()
    {
        agent.isStopped = true;

        if (isDead) return;
        isDead = true;
        canBeInterrupt = false;

        // Disable collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        animator.SetTrigger("Die");
        StartCoroutine(FadeAfterAnimation());
    }

    private IEnumerator FadeAfterAnimation()
    {
        // Wait for death animation to play
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.3f);

        StartCoroutine(FadeAndDestroy(0.5f));

        // Hide all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;
    }

    private IEnumerator FadeAndDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;
    }

    // ===================================
    //         ANIMATION HANDLER
    // ===================================

    private void SetUpAnimationTypes()
    {
        animationTypes.Add(WALKING);
        animationTypes.Add(RUNNING);
        animationTypes.Add(INVESTIGATING);
        animationTypes.Add(TIRED);
        animationTypes.Add(IDLING);
        animationTypes.Add(BECONTROLLED);
        animationTypes.Add(TAKEDAMAGE);
    }

    protected void PlayAnimation(string animationName)
    {
        List<string> temp = new List<string>(animationTypes);
        if (temp.Contains(animationName))
        {
            // animator.SetBool(animationName, true);
            temp.Remove(animationName);
            for (int i = 0; i < temp.Count; i++)
            {
                //animator.SetBool(temp[i], false);
            }
        }
    }

}
