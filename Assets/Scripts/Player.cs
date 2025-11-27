using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float walkSpeed = 2f;
    public float runningSpeed = 5f;

    public PowerManager powerManager;

    private CharacterController controller;
    private Vector3 moveDirection;
    public VoiceManager voiceManager;
    
    // Audio variables
    private bool wasMoving = false;
    private bool wasRunning = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ);

        if (moveDirection.magnitude > 1)
            moveDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = runningSpeed;
            voiceManager.MakeVoice(transform.position);
            
            // Play running sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerRun();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = walkSpeed;
            
            // Stop running sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopSound("PlayerRun");
            }
        }

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

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        
        // Handle movement audio
        bool isMoving = moveDirection.magnitude > 0.1f;
        bool isRunning = isMoving && moveSpeed > walkSpeed;
        
        // Play footstep sounds for walking
        if (isMoving && !isRunning && !wasMoving && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerFootstep();
        }
        
        // Update previous frame states
        wasMoving = isMoving;
        wasRunning = isRunning;
    }
}
