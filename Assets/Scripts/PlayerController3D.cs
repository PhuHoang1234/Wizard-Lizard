using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Base Speeds (units/sec)")]
    public float walkSpeed = 2.8f;
    public float sprintSpeed = 4.2f;

    [Header("Smoothing")]
    public float inputSmoothTime = 0.06f;
    public float speedBlendTime = 0.12f;
    public float moveSmoothTime = 0.10f;

    [Header("Control Feel")]
    public float turnBoost = 1.25f;
    public float lateralFriction = 10f;
    public float minInput = 0.08f;

    [Header("Facing (optional)")]
    public bool smoothFacing = true;
    public float facingSmoothTime = 0.08f;

    [Header("Keys")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode castKey = KeyCode.F;           // <-- cast on F

    [Header("Animation (Locomotion)")]
    public Animator animator;
    public string walkBool = "IsWalking";
    public string runBool = "IsRunning";

    [Header("Animation (Cast UpperBody Layer)")]
    public string castTrigger = "Cast";             // Trigger in Animator
    public int upperBodyLayerIndex = 1;          // Layer with Avatar Mask (arms/torso)
    public float castDuration = 0.70f;           // seconds (match your clip)
    public float castBlendIn = 0.10f;           // layer weight rise
    public float castBlendOut = 0.15f;           // layer weight fall
    public float castCooldown = 0.20f;           // min time between casts
    public bool walkOnlyDuringCast = true;       // optional: prevent sprint while casting

    public PowerManager powerManager;
    public VoiceManager voiceManager;

    // Runtime
    Rigidbody rb;
    Vector2 rawInput, filteredInput, filteredInputVel;
    float targetMaxSpeed, currentMaxSpeed, speedVelRef;
    Vector2 desiredVelXZ, velRefXZ, lastMoveDirXZ;
    float _facingVel;
    
    // Audio tracking
    private bool wasMoving = false;
    private bool wasRunning = false;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.5f; // Time between footsteps

    // Casting state
    float _castTimer = 0f;
    float _nextCastTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezePositionY;

        currentMaxSpeed = walkSpeed;
        targetMaxSpeed = walkSpeed;

        // start with upper-body layer off
        if (animator && upperBodyLayerIndex >= 0 && upperBodyLayerIndex < animator.layerCount)
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
    }

    void Update()
    {
        // ---- Movement input ----
        rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (rawInput.sqrMagnitude > 1f) rawInput.Normalize();

        filteredInput = SmoothDampVec2(filteredInput, rawInput, ref filteredInputVel,
                                       inputSmoothTime, Mathf.Infinity, Time.deltaTime);

        bool sprinting = Input.GetKey(sprintKey) && !(_castTimer > 0f && walkOnlyDuringCast);
        targetMaxSpeed = sprinting ? sprintSpeed : walkSpeed;
        currentMaxSpeed = Mathf.SmoothDamp(currentMaxSpeed, targetMaxSpeed, ref speedVelRef, speedBlendTime);

        if (filteredInput.magnitude < minInput)
            desiredVelXZ = Vector2.zero;
        else
            desiredVelXZ = filteredInput.normalized * currentMaxSpeed;

        if (desiredVelXZ.sqrMagnitude > 0.0001f)
            lastMoveDirXZ = desiredVelXZ.normalized;

        if (sprinting) voiceManager.MakeVoice(transform.position);
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            powerManager.Hide();
            // Play hide sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerHide();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.P)) 
        {
            powerManager.Unhide();
        }


        // ---- Locomotion booleans ----
        if (animator)
        {
            bool isMoving = desiredVelXZ.sqrMagnitude > 0.0001f;
            bool isRunning = isMoving && sprinting;
            animator.SetBool(walkBool, isMoving && !sprinting);
            animator.SetBool(runBool, isRunning);
            
            // Handle footstep audio
            HandleFootstepAudio();
        }

        // ---- Casting input on F ----
        HandleCastingInput();

        // ---- Blend UpperBody layer weight ----
        UpdateCastLayerWeight();
    }

    void FixedUpdate()
    {
        Vector3 v3 = rb.linearVelocity;
        Vector2 vXZ = new Vector2(v3.x, v3.z);

        float dirDot = 1f;
        if (vXZ.sqrMagnitude > 0.0001f && desiredVelXZ.sqrMagnitude > 0.0001f)
            dirDot = Vector2.Dot(vXZ.normalized, desiredVelXZ.normalized);
        float boost = (dirDot < -0.25f) ? turnBoost : 1f;

        Vector2 targetXZ = desiredVelXZ * boost;
        Vector2 newVelXZ = SmoothDampVec2(vXZ, targetXZ, ref velRefXZ,
                                          moveSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(newVelXZ.x, 0f, newVelXZ.y);

        if (desiredVelXZ.sqrMagnitude > 0.0001f && rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            Vector3 fwd3 = new Vector3(desiredVelXZ.x, 0f, desiredVelXZ.y).normalized;
            Vector3 right3 = Vector3.Cross(Vector3.up, fwd3).normalized;
            float lateralSpeed = Vector3.Dot(rb.linearVelocity, right3);
            float reduce = Mathf.MoveTowards(lateralSpeed, 0f, lateralFriction * Time.fixedDeltaTime);
            rb.linearVelocity += right3 * (reduce - lateralSpeed);
        }

        if (smoothFacing && lastMoveDirXZ.sqrMagnitude > 0.0001f)
        {
            float targetYaw = Mathf.Atan2(lastMoveDirXZ.x, lastMoveDirXZ.y) * Mathf.Rad2Deg;
            float currentYaw = transform.eulerAngles.y;
            float newYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref _facingVel, facingSmoothTime);
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }
    }

    // ---- Casting helpers ----
    void HandleCastingInput()
    {
        if (!animator) return;

        if (Input.GetKeyDown(castKey) && Time.time >= _nextCastTime)
        {
            // fire the Cast trigger on the UpperBody layer
            animator.ResetTrigger(castTrigger);
            animator.SetTrigger(castTrigger);

            _castTimer = castDuration;
            _nextCastTime = Time.time + castCooldown;
        }

        if (_castTimer > 0f)
            _castTimer -= Time.deltaTime;
    }

    void UpdateCastLayerWeight()
    {
        if (!animator || upperBodyLayerIndex < 0 || upperBodyLayerIndex >= animator.layerCount) return;

        float current = animator.GetLayerWeight(upperBodyLayerIndex);
        float target = (_castTimer > 0f) ? 1f : 0f;

        float blendTime = (target > current) ? castBlendIn : castBlendOut;
        float t = (blendTime <= 0f) ? 1f : 1f - Mathf.Exp(-Time.deltaTime / blendTime);
        float next = Mathf.Lerp(current, target, t);

        animator.SetLayerWeight(upperBodyLayerIndex, next);
    }

    // --- Helper: SmoothDamp for Vector2 ---
    static Vector2 SmoothDampVec2(Vector2 current, Vector2 target, ref Vector2 currentVelocity,
                                  float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float omega = 2f / smoothTime;
        float x = omega * deltaTime;
        float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

        Vector2 change = current - target;
        float maxChange = maxSpeed * smoothTime;
        if (change.sqrMagnitude > maxChange * maxChange)
            change = change.normalized * maxChange;

        target = current - change;
        Vector2 temp = (currentVelocity + omega * change) * deltaTime;
        currentVelocity = (currentVelocity - omega * temp) * exp;
        Vector2 output = target + (change + temp) * exp;
        return output;
    }
    
    private void HandleFootstepAudio()
    {
        // Null check for AudioManager
        if (AudioManager.Instance == null)
        {
            return;
        }
        
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(sprintKey);
        
        if (isMoving && !wasMoving)
        {
            // Started moving - play appropriate footstep sound
            if (isRunning)
            {
                AudioManager.Instance.PlayPlayerRun();
            }
            else
            {
                AudioManager.Instance.PlayPlayerFootstep();
            }
            footstepTimer = 0f;
        }
        else if (isMoving)
        {
            // Continue moving - play footsteps at intervals
            footstepTimer += Time.deltaTime;
            float stepInterval = isRunning ? 0.3f : 0.6f; // Faster steps when running
            
            if (footstepTimer >= stepInterval)
            {
                if (isRunning)
                {
                    AudioManager.Instance.PlayPlayerRun();
                }
                else
                {
                    AudioManager.Instance.PlayPlayerFootstep();
                }
                footstepTimer = 0f;
            }
        }
        
        // Check for running state changes
        if (isRunning && !wasRunning)
        {
            AudioManager.Instance.PlayPlayerRun();
            footstepTimer = 0f;
        }
        else if (wasRunning && !isRunning && isMoving)
        {
            AudioManager.Instance.PlayPlayerFootstep();
            footstepTimer = 0f;
        }
        
        wasMoving = isMoving;
        wasRunning = isRunning;
    }
}
