using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Movement speed

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get input from WASD or Arrow keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Create movement vector relative to camera direction
        Vector3 forward = transform.forward * vertical;
        Vector3 right = transform.right * horizontal;

        // Combine and normalize to prevent faster diagonal movement
        Vector3 moveDirection = (forward + right).normalized;

        // Apply movement
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}