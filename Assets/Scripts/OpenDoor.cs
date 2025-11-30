using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenDoor : MonoBehaviour
{
    public bool HasBeenPickedUp { get; private set; }

    private Renderer[] renderers;
    private Collider col;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        col = GetComponent<Collider>();
        col.isTrigger = true;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || HasBeenPickedUp)
            return;

        foreach (var r in renderers)
            r.enabled = false;

        col.enabled = false;

        HasBeenPickedUp = true; 
    }
}
