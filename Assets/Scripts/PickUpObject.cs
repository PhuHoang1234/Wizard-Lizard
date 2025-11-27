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
        // Only the player can pick this up
        if (!other.CompareTag("Player"))
            return;

        if (HasBeenPickedUp)
            return;

        foreach (var r in renderers)
            r.enabled = false;

        col.enabled = false;
        HasBeenPickedUp = true;
    }

}
