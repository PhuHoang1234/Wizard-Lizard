using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOnLadder : MonoBehaviour
{
    [Header("UI")]
    public GameObject winPanel;     
    public string playerTag = "Player";

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";   

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (winPanel != null)
            winPanel.SetActive(true);

        // Stop footstep sounds when winning
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DisableFootsteps();
            Debug.Log("🔇 Stopped footsteps - level won!");
        }

        Time.timeScale = 0f;
    }
    public void Retry()
    {
        // Stop footsteps before retrying
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DisableFootsteps();
        }
        
        Time.timeScale = 1f; 
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);    
    }

    public void MainMenu()
    {
        // Stop footsteps before going to main menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DisableFootsteps();
            AudioManager.Instance.SwitchToMainMenuMusic();
        }
        
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuScene);
    }

    public void NextLevel()
    {
        // Stop footsteps before loading next level
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
        }
        
        Time.timeScale = 1f;
        
        // Get current scene and load next one
        Scene current = SceneManager.GetActiveScene();
        int nextLevelIndex = current.buildIndex + 1;
        
        // Check if next level exists in build settings
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"🚀 Loading next level: Scene index {nextLevelIndex}");
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            Debug.Log("🏁 No more levels! Going to main menu.");
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
