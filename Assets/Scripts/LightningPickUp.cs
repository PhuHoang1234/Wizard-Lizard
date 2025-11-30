using UnityEngine;

public class LightningPickUp : MonoBehaviour
{
    public GameObject panel;   

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Player"))
            return;

       
        PowerManager pm = other.GetComponentInParent<PowerManager>();
        if (pm == null)
            pm = other.GetComponentInChildren<PowerManager>();

        if (pm != null)
        {
            pm.UnlockLightning();     
        }
        else
        {
            Debug.LogWarning("LightningPickUp: Player has no PowerManager!");
        }

        if (panel != null)
            panel.SetActive(true);

        gameObject.SetActive(false);
    }
}
