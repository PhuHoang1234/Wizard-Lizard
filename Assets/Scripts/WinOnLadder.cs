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


        Time.timeScale = 0f;
    }
    public void Retry()
    {
        Time.timeScale = 1f; 
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);    
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(mainMenuScene);
    }
}
