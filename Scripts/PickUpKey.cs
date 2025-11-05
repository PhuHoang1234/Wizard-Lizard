using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    private Renderer rend;

    // Other scripts can read this if they want
    public bool HasBeenPickedUp { get; private set; }

    void Awake()
    {
        rend = GetComponent<Renderer>();   // or MeshRenderer, both are fine
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player, and only once
        if (!other.CompareTag("Player") || HasBeenPickedUp)
            return;

        if (rend != null)
        {
            rend.enabled = false;          // hide the key
        }

        HasBeenPickedUp = true;           // mark as picked up
    }
}
