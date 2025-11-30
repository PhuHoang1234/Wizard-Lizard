using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenDoor : MonoBehaviour
{
    // Other scripts (like the door) can read this
    public bool HasBeenPickedUp { get; private set; }

    private Renderer[] renderers;
    private Collider col;

    void Awake()
    {
        // get all renderers on this object + children
        renderers = GetComponentsInChildren<Renderer>();

        col = GetComponent<Collider>();
        col.isTrigger = true;   // make sure it’s a trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player, and only once
        if (!other.CompareTag("Player") || HasBeenPickedUp)
            return;

        // hide the key visually
        foreach (var r in renderers)
            r.enabled = false;

        // disable collider so we don’t trigger again
        col.enabled = false;

        HasBeenPickedUp = true; // mark as picked up
    }
}
