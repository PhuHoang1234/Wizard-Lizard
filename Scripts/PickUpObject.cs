using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupObject : MonoBehaviour
{
    // Other scripts can read this
    public bool HasBeenPickedUp { get; private set; }

    private Renderer[] renderers;
    private Collider col;

    private void Awake()
    {
        // Get all renderers (this object + children)
        renderers = GetComponentsInChildren<Renderer>();

        col = GetComponent<Collider>();
        col.isTrigger = true;   // make this object a trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        // If you REALLY want to limit to Player later:
        // if (!other.CompareTag("Player")) return;

        if (HasBeenPickedUp)
            return;

        // Hide everything under this object
        foreach (var r in renderers)
            r.enabled = false;

        col.enabled = false;    // stop future triggers
        HasBeenPickedUp = true;
    }
}
