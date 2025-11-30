using UnityEngine;

public class Level1Intro : MonoBehaviour
{
    [Header("UI")]
    public GameObject introPanel;         

    [Header("What to freeze")]
    public MonoBehaviour[] playerScripts; 

    void Start()
    {
        
        if (introPanel != null)
            introPanel.SetActive(true);

       
        foreach (var s in playerScripts)
            if (s != null) s.enabled = false;
        Time.timeScale = 0f;
    }

   
    public void CloseIntro()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        
        foreach (var s in playerScripts)
            if (s != null) s.enabled = true;

        Time.timeScale = 1f;
    }
}
