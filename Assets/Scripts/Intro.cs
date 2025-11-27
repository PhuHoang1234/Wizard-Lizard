using UnityEngine;

public class Level1Intro : MonoBehaviour
{
    [Header("UI")]
    public GameObject introPanel;          // the panel with text + X

    [Header("What to freeze")]
    public MonoBehaviour[] playerScripts;  // movement, camera follow, etc.

    void Start()
    {
        // Show panel
        if (introPanel != null)
            introPanel.SetActive(true);

        // Disable player control
        foreach (var s in playerScripts)
            if (s != null) s.enabled = false;
        Time.timeScale = 0f;
    }

    // Call this from the X button
    public void CloseIntro()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        // Re-enable player control
        foreach (var s in playerScripts)
            if (s != null) s.enabled = true;

        Time.timeScale = 1f;
    }
}
