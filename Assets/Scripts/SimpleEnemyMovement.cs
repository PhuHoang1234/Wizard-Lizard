using UnityEngine;

public class SimpleEnemyMovement : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    
    private Vector3 startPosition;
    private bool movingRight = true;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Simple left-right movement
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= startPosition.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= startPosition.x - moveDistance)
            {
                movingRight = true;
            }
        }
    }
}
