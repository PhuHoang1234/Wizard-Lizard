using UnityEngine;

public class LightningPickUp : MonoBehaviour
{
    public GameObject panel;   // drag your UI panel here

    private void OnTriggerEnter(Collider other)
    {
        // Only the player can pick this up
        if (!other.CompareTag("Player"))
            return;

        // Find PowerManager on player
        PowerManager pm = other.GetComponentInParent<PowerManager>();
        if (pm == null)
            pm = other.GetComponentInChildren<PowerManager>();

        if (pm != null)
        {
            pm.UnlockLightning();     // 🔓 give lightning
        }
        else
        {
            Debug.LogWarning("LightningPickUp: Player has no PowerManager!");
        }

        // 👉 Show the info panel
        if (panel != null)
            panel.SetActive(true);

        // Remove pickup so it can't be taken twice
        gameObject.SetActive(false);
    }
}
