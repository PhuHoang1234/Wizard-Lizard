using UnityEngine;

public class TailObject : MonoBehaviour
{
    private Vector3 position;
    private bool isReleased = false;
    public PowerManager powerManager;

    void Update()
    {
        if (!isReleased) return;

        if (!powerManager.Distraction(position))
        {
            Destroy(gameObject);
            isReleased = false;
        }
    }

    public void ReleaseTail(Vector3 position)
    {
        this.position = position;
        powerManager.DistracitonStart();
        isReleased = true;
    }

    // ---- Pick up tail ----
    private void OnTriggerEnter(Collider other)
    {
        if (isReleased)
        {
            if (other.CompareTag("Player"))
            {
                // refresh cd
                powerManager.RefreshDistractionCooldown();
            }
            Destroy(gameObject);
            isReleased = false;
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                isReleased = false;
            }
        }


    }
}
