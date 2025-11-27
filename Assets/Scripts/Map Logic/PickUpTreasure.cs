using UnityEngine;

public class PickUpTreasure : MonoBehaviour
{
    private Renderer[] renderers;
    public bool HasBeenPickedUp { get; private set; }

    void Awake()
    {
        // Grab ALL renderers on this chest and its children
        renderers = GetComponentsInChildren<Renderer>();

        // Make sure this collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || HasBeenPickedUp)
            return;

        Debug.Log("Treasure picked up by: " + other.name);

        foreach (var r in renderers)
        {
            r.enabled = false;   // hide loot, top, body, whatever is under this chest
        }

        HasBeenPickedUp = true;

        // Play treasure pickup sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTreasurePickup();
        }
    }
}
