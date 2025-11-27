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
    public KeyCode illusionKey = KeyCode.R;       // <-- illusion on R
    public KeyCode distractionKey = KeyCode.E;    // <-- distraction on E

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

    [Header("Animation (Cast UpperBody Layer)")]
    public string illusionTrigger = "Illusion";     // Trigger in Animator
    public float illusionBlendIn = 0.10f;           // layer weight rise
    public float illusionBlendOut = 0.15f;           // layer weight fall
    public bool walkOnlyDuringIllusion = true;       // optional: prevent sprint while illusion

    [Header("Magic Components")]
    public float tailSpeed = 5.0f;
    public float tailRange = 50.0f;

    public GameManager manager;

    // Runtime
    Rigidbody rb;
    Vector2 rawInput, filteredInput, filteredInputVel;
    float targetMaxSpeed, currentMaxSpeed, speedVelRef;
    Vector2 desiredVelXZ, velRefXZ, lastMoveDirXZ;
    float _facingVel;

    // GameObjects for Distraction
    public GameObject tailPrefab;

    // player state for tail mode
    protected enum PlayerState
    {
        Normal,
        TailControl
    }
    private PlayerState state = PlayerState.Normal;

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
        if (manager.isGamePaused) return;

        if (state == PlayerState.Normal)
        {
            // ---- Movement input ----
            rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (rawInput.sqrMagnitude > 1f) rawInput.Normalize();

            filteredInput = SmoothDampVec2(filteredInput, rawInput, ref filteredInputVel,
                                           inputSmoothTime, Mathf.Infinity, Time.deltaTime);

            bool sprinting = Input.GetKey(sprintKey) && !(manager.powerManager.isSkilling() && walkOnlyDuringCast);
            targetMaxSpeed = sprinting ? sprintSpeed : walkSpeed;
            currentMaxSpeed = Mathf.SmoothDamp(currentMaxSpeed, targetMaxSpeed, ref speedVelRef, speedBlendTime);

            if (filteredInput.magnitude < minInput)
                desiredVelXZ = Vector2.zero;
            else
                desiredVelXZ = filteredInput.normalized * currentMaxSpeed;

            if (desiredVelXZ.sqrMagnitude > 0.0001f)
                lastMoveDirXZ = desiredVelXZ.normalized;

            if (sprinting) manager.voiceManager.MakeVoice(transform.position);


            // ---- Locomotion booleans ----
            if (animator)
            {
                bool isMoving = desiredVelXZ.sqrMagnitude > 0.0001f;
                bool isRunning = isMoving && sprinting;
                animator.SetBool(walkBool, isMoving && !sprinting);
                animator.SetBool(runBool, isRunning);
            }

            // ---- Distraction input on E ----
            HandleDistractionInput();

            // ---- Casting input on F ----
            HandleCastingInput();

            // ---- Illusion input on R ----
            HandleIllusionInput();

            // ---- Blend UpperBody layer weight ----
            UpdateCastLayerWeight();
        }
        else if (state == PlayerState.TailControl)
        {
            ControlTail();
        }
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

        if (Input.GetKeyDown(castKey) && manager.powerManager.isReady("lightning"))
        {
            if (!manager.powerManager.LightningStart(transform)) return;
            // fire the Cast trigger on the UpperBody layer
            animator.ResetTrigger(castTrigger);
            animator.SetTrigger(castTrigger);

        }
    }

    void UpdateCastLayerWeight()
    {
        if (!animator || upperBodyLayerIndex < 0 || upperBodyLayerIndex >= animator.layerCount) return;

        float current = animator.GetLayerWeight(upperBodyLayerIndex);
        float target = (manager.powerManager.isSkilling()) ? 1f : 0f;

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

    // ---- Illusion Helpers ----
    void HandleIllusionInput()
    {
        if (!animator) return;

        if (Input.GetKeyDown(illusionKey) && !manager.powerManager.isPlayerHidding() && manager.powerManager.isReady("illusion"))
        {
            if (!manager.powerManager.IllusionStart()) return;
            // fire the Cast trigger on the UpperBody layer
            animator.ResetTrigger(castTrigger);
            animator.SetTrigger(castTrigger);
        }


        if (manager.powerManager.isPlayerHidding() && manager.powerManager.isReady("illusionDuration"))
        {
            manager.powerManager.IllusionEnd();
        }

    }

    // ---- Distraction Helpers ----
    void HandleDistractionInput()
    {
        if (Input.GetKeyUp(distractionKey) && state == PlayerState.Normal && manager.powerManager.isReady("distraction"))
        {
            state = PlayerState.TailControl;

            Vector3 spawnPos = transform.position + transform.forward * 3f;
            manager.tail = Instantiate(tailPrefab, spawnPos, Quaternion.identity);
            TailObject to = manager.tail.GetComponent<TailObject>();
            to.powerManager = manager.powerManager;
            manager.CameraFocusChange(manager.tail);

        }
    }

    void ControlTail()
    {
        if (manager.tail == null && state == PlayerState.Normal) return;
        if (manager.tail == null && state == PlayerState.TailControl) state = PlayerState.Normal;

        if (Input.GetKeyUp(distractionKey) && state == PlayerState.TailControl)
        {
            state = PlayerState.Normal;
            ReleaseTail();
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * tailSpeed * Time.deltaTime;
        manager.tail.transform.position += move;

        Vector3 offset = manager.tail.transform.position - transform.position;
        if (offset.magnitude > tailRange)
            manager.tail.transform.position = transform.position + offset.normalized * tailRange;
    }

    void ReleaseTail()
    {
        manager.tail.GetComponent<TailObject>().ReleaseTail(manager.tail.transform.position);
        manager.CameraFocusChange(manager.player);
    }
}
