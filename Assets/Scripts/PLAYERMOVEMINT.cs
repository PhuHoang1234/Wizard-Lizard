using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;          // Horizontal movement speed (reduced from 5.0f)
    [SerializeField] private float gravityMult = 3.0f;
    private float gravity = -9.81f;         // Base gravity (m/s^2)

    [Header("Footstep Audio")]
    public float footstepDelay = 0.4f;      // Time between footstep sounds (faster for testing)
    public bool enableFootsteps = true;     // Toggle footsteps on/off

    private CharacterController controller;
    private Vector3 velocity;               // Stores vertical velocity (and could store more later)
    private float footstepTimer;            // Timer for footstep sounds
    private bool wasMovingLastFrame;        // Track if player was moving last frame
    private bool isPlayingFootsteps;        // Track if footsteps are currently playing
    private Vector3 lastPosition;           // Track actual movement for better footstep detection

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position; // Initialize position tracking
    }

    void Update()
    {
        // Check EACH key individually for maximum clarity
        bool pressingW = Input.GetKey(KeyCode.W);
        bool pressingA = Input.GetKey(KeyCode.A);
        bool pressingS = Input.GetKey(KeyCode.S);
        bool pressingD = Input.GetKey(KeyCode.D);
        
        // Are we pressing ANY WASD key?
        bool isPressingWASD = pressingW || pressingA || pressingS || pressingD;

        // Get input from WASD keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D 
        float vertical = Input.GetAxis("Vertical");     // W/S 

        // Create movement vector relative to camera/player direction
        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;

        // Combine and normalize to prevent faster diagonal movement
        Vector3 moveDirection = (forward + right).normalized;

        // Horizontal movement
        Vector3 horizontalMove = Vector3.zero;
        if (moveDirection.magnitude >= 0.1f)
        {
            horizontalMove = moveDirection * moveSpeed;
        }

        // Handle footstep sounds ONLY based on WASD key presses
        HandleFootstepAudio(isPressingWASD);

        // Apply gravity to velocity.y
        ApplyGravity();

        // Combine horizontal + vertical movement
        Vector3 finalMove = horizontalMove;
        finalMove.y = velocity.y;

        // Move the controller
        controller.Move(finalMove * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        // If grounded, keep a small downward force so the controller stays snapped to the ground
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // v = v + g * dt
        velocity.y += gravity * gravityMult * Time.deltaTime;
    }

    private void HandleFootstepAudio(bool isPressingWASD)
    {
        if (!enableFootsteps) return;
        
        // Debug: Show current state
        if (Time.frameCount % 60 == 0) // Every 60 frames (about once per second)
        {
            Debug.Log($"🔍 WASD pressed: {isPressingWASD}, On ground: {controller.isGrounded}, Playing footsteps: {isPlayingFootsteps}");
        }
        
        // Simple logic: Are we pressing WASD AND on ground?
        bool shouldPlayFootsteps = isPressingWASD && controller.isGrounded;
        
        if (shouldPlayFootsteps)
        {
            // Start footstep loop if not already playing
            if (!isPlayingFootsteps)
            {
                isPlayingFootsteps = true;
                Debug.Log("🦶 STARTED footstep loop - pressing WASD!");
                
                // Start the looping footstep sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstep();
                }
            }
        }
        else
        {
            // Stop footstep loop immediately when NOT pressing WASD
            if (isPlayingFootsteps)
            {
                isPlayingFootsteps = false;
                Debug.Log("🛑 STOPPED footstep loop - NO WASD keys pressed!");
                
                // Stop the looping footstep sound immediately
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopFootstep();
                }
            }
        }
    }
}
