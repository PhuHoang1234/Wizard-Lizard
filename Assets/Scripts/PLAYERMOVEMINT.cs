using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
<<<<<<< HEAD
    public float moveSpeed = 5.0f; // Movement speed

    private CharacterController controller;
=======
    public float moveSpeed = 5.0f;          // Horizontal movement speed
    [SerializeField] private float gravityMult = 3.0f;
    private float gravity = -9.81f;         // Base gravity (m/s^2)

    private CharacterController controller;
    private Vector3 velocity;               // Stores vertical velocity (and could store more later)
>>>>>>> origin/Mahdi

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get input from WASD or Arrow keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

<<<<<<< HEAD
        // Create movement vector relative to camera direction
=======
        // Create movement vector relative to camera/player direction
>>>>>>> origin/Mahdi
        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;

        // Combine and normalize to prevent faster diagonal movement
        Vector3 moveDirection = (forward + right).normalized;

<<<<<<< HEAD
        // Apply movement
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
=======
        // Horizontal movement
        Vector3 horizontalMove = Vector3.zero;
        if (moveDirection.magnitude >= 0.1f)
        {
            horizontalMove = moveDirection * moveSpeed;
        }

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
}
>>>>>>> origin/Mahdi
