using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void playGame()
    {
        SceneManager.LoadScene("Level1");  
    }
    public void Quit()
    {
        Application.Quit();
    }
}
