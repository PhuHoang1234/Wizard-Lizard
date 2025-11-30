using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;        // assign your win panel here
    public string playerTag = "Player";

    [Header("Requirements")]
    public PickupObject keyPickup;     // key object (PickupObject)
    public PickupObject chestPickup;   // chest object (PickupObject)

    private void OnCollisionEnter(Collision collision)
    {
        // Check the object we collided with
        if (!collision.collider.CompareTag(playerTag))
            return;

        // Must have key
        if (keyPickup == null || chestPickup == null)
        {
            Debug.LogError("EndLevel: keyPickup or chestPickup is NOT assigned in Inspector. Door stays locked.");
            return;
        }

        Debug.Log($"Key picked? {keyPickup.HasBeenPickedUp} | Chest picked? {chestPickup.HasBeenPickedUp}");

        if (!keyPickup.HasBeenPickedUp || !chestPickup.HasBeenPickedUp)
        {
            Debug.Log("EndLevel: player reached door without all pickups. Door locked.");
            return;
        }

        // ✅ All conditions met → show win panel
        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("EndLevel: WIN!");
    }


    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
