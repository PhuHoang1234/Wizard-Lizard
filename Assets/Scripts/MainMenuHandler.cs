using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Scene Management")]
    public string firstLevelName = "Level1";
    
    [Header("Audio")]
    public MainMenuAudioController audioController;

    void Start()
    {
        // Find audio controller if not assigned
        if (audioController == null)
        {
            audioController = FindObjectOfType<MainMenuAudioController>(true);
        }
    }

    public void PlayGame()
    {
        // Switch to game music before loading level
        if (audioController != null)
        {
            audioController.OnPlayGameClicked();
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SwitchToGameMusic();
            // Enable footsteps for gameplay
            AudioManager.Instance.EnableFootsteps();
            Debug.Log("🎮 Enabled footsteps for Level 1");
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        // This is allowed because it's wrapped in UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
