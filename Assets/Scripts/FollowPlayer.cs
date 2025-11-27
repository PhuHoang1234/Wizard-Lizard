using UnityEngine;
using UnityEngine.SceneManagement;

public class Follow_player : MonoBehaviour
{
    public GameManager manager;


    void LateUpdate()
    {
        Vector3 offset = new Vector3(0, 15, 0);

        if (SceneManager.GetActiveScene().name == "Level3")
        {
            offset = new Vector3(0, 30, 0);
        }

        transform.position = manager.CameraFocus().transform.position + offset;
    }
}
