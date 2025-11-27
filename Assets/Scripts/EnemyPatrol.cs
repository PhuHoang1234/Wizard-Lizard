using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Area (Back/Forth Bounds)")]
    public float patrolXMin = -5f;
    public float patrolXMax = 5f;
    public float patrolZMin = -3f;
    public float patrolZMax = 3f;
    public float patrolSpeed = 2f;

    [Header("Chase")]
    public Transform player;        // Drag Player GameObject here
    public float chaseRange = 10f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f; // How fast body turns (smoother than instant)

    [Header("Head Look IK")]
    public Transform lookTarget;    // Drag Player's HEAD bone or Camera here for better eye contact
    public float lookSpeed = 2f;    // How fast head turns to look
    public float lookWeight = 1f;   // 0-1: How much head focuses on player (1=full)

    private Animator anim;
    private float patrolXTime = 0f;
    private float patrolZTime = 0f;
    private Vector3 targetPos;
    private bool movingHoriz = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (lookTarget == null) lookTarget = player;  // Fallback to player root if no head assigned
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer < chaseRange)
        {
            // CHASE: Run towards player + body rotates to face + head looks
            anim.SetBool("IsChasing", true);
            anim.SetFloat("Speed", chaseSpeed);
            ChasePlayer();
        }
        else
        {
            // PATROL: Walk back/forth + face movement direction
            anim.SetBool("IsChasing", false);
            anim.SetFloat("Speed", patrolSpeed);
            Patrol();
        }
    }

    void LateUpdate()
    {
        // HEAD IK: Only during chase - overrides animation for realistic looking
        if (anim.GetBool("IsChasing"))
        {
            anim.SetLookAtWeight(lookWeight, 0.3f, 1f, 0f);  // Head/Neck/Spine focus
            anim.SetLookAtPosition(lookTarget.position);
        }
        else
        {
            anim.SetLookAtWeight(0f);  // Reset when not chasing
        }
    }

    void ChasePlayer()
    {
        // Smooth body rotation + forward movement
        Vector3 chaseDir = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(chaseDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move forward (after rotation)
        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        patrolXTime += Time.deltaTime;
        patrolZTime += Time.deltaTime;

        if (movingHoriz)
        {
            float xPos = Mathf.PingPong(patrolXTime * patrolSpeed * 0.5f, patrolXMax - patrolXMin) + patrolXMin;
            targetPos = new Vector3(xPos, transform.position.y, transform.position.z);
        }
        else
        {
            float zPos = Mathf.PingPong(patrolZTime * patrolSpeed * 0.5f, patrolZMax - patrolZMin) + patrolZMin;
            targetPos = new Vector3(transform.position.x, transform.position.y, zPos);
        }

        if (patrolXTime > 10f)
        {
            patrolXTime = 0f;
            movingHoriz = !movingHoriz;
            patrolZTime = 0f;
        }

        // Face patrol direction + smooth move
        Vector3 moveDir = (targetPos - transform.position).normalized;
        Quaternion patrolRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, patrolRot, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, patrolSpeed * Time.deltaTime);
    }
}