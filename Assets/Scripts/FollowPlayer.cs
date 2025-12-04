using UnityEngine;
using UnityEngine.SceneManagement;

public class Follow_player : MonoBehaviour
{
    public Transform Player;
    public Transform target;

    void Start()
    {
        target = Player;
    }

    void LateUpdate()
    {
        Vector3 offset = new Vector3(0, 15, 0);

        if (SceneManager.GetActiveScene().name == "Level3")
        {
            offset = new Vector3(0, 30, 0);
        }

        transform.position = target.position + offset;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetTarget()
    {
        target = Player;
    }
}
