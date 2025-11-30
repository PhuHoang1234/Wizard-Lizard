using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOnLadder : MonoBehaviour
{
    [Header("UI")]
    public GameObject winPanel;     // your WinLevel UI panel
    public string playerTag = "Player";

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";   // set this in Inspector if name is different

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (winPanel != null)
            winPanel.SetActive(true);

        // Pause game when you win
        Time.timeScale = 0f;
    }

    // ---- UI BUTTON METHODS ----

    // Called by Retry button
    public void Retry()
    {
        Time.timeScale = 1f; // un-pause
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);    // reload current level
    }

    // Called by Main Menu button
    public void MainMenu()
    {
        Time.timeScale = 1f; // un-pause
        SceneManager.LoadScene(mainMenuScene);
    }
}
