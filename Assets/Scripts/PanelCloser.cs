using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    // If left empty, it will close the GameObject this script is on
    public GameObject panelToClose;

    void Awake()
    {
        if (panelToClose == null)
            panelToClose = gameObject;
    }

    // Called by the UI Button
    public void ClosePanel()
    {
        if (panelToClose != null)
            panelToClose.SetActive(false);
    }
}
