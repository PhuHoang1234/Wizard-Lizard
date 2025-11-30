using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    public GameObject panelToClose;

    void Awake()
    {
        if (panelToClose == null)
            panelToClose = gameObject;
    }

    public void ClosePanel()
    {
        if (panelToClose != null)
            panelToClose.SetActive(false);
    }
}
