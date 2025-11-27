using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public GameManager manager;

    // Update is called once per frame
    void Update()
    {
        transform.position = manager.CameraFocus().transform.position + new Vector3(0, 15, 0);
    }
}