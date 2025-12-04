using UnityEngine;

public class TailObject : MonoBehaviour
{
    private Vector3 position;
    private bool isReleased = false;
    public KeyCode releaseKey = KeyCode.E;
    public float tailSpeed = 5.0f;
    public bool canControl = true;
    public PowerManager powerManager;

    void Update()
    {
        if (isReleased) return;

        if (Input.GetKeyDown(releaseKey))
        {
            ReleaseTail();
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * tailSpeed * Time.deltaTime;
        transform.position += move;

    }
    void ReleaseTail()
    {
        isReleased = true;
        powerManager.TailReleased();
    }

    // ---- Pick up tail ----
    private void OnTriggerEnter(Collider other)
    {
        if (isReleased)
        {
            if (other.CompareTag("Player")) powerManager.TailPicked(true);
            else powerManager.TailPicked(false);
            isReleased = false;
            Destroy(gameObject);
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                powerManager.TailPicked(false);
                isReleased = false;
                Destroy(gameObject);
            }
        }

    }
}
