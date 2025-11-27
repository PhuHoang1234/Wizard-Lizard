using UnityEngine;

public class WinOnLadder : MonoBehaviour
{
    public GameObject winPanel;     // your WinLevel UI panel
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;  // pause game when you win
    }
}
