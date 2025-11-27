using UnityEngine;
using UnityEngine.SceneManagement;

public class Follow_player : MonoBehaviour
{
    public Transform Player;

    void LateUpdate()
    {
        Vector3 offset = new Vector3(0, 15, 0);

        if (SceneManager.GetActiveScene().name == "Level3")
        {
            offset = new Vector3(0, 30, 0);
        }

        transform.position = Player.position + offset;
    }
}
