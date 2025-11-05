using UnityEngine;

public class Follow_player : MonoBehaviour
{

    public Transform Player;

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position + new Vector3(0, 15, 0);
    }
}