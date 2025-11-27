using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public string firstLevelName = "Level1";

    public void PlayGame()
    {
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
