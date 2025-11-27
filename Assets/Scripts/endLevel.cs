using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public GameObject panel;        // assign your UI Panel here in Inspector
    public string playerTag = "Player";
    public GameData gameData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            panel.SetActive(true);  
            Time.timeScale = 0f;
        }
    }
    public void Retry()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1f;

    }
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;


    }
    public void NextLevel()
    {

     int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
        Time.timeScale = 1f;

    }

}
