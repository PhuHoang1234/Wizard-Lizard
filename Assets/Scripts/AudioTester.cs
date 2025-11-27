using UnityEngine;

public class AudioTester : MonoBehaviour
{
    [Header("Press Keys to Test Audio")]
    [Space]
    public KeyCode testFootstep = KeyCode.Alpha1;
    public KeyCode testRun = KeyCode.Alpha2;  
    public KeyCode testHide = KeyCode.Alpha3;
    public KeyCode testKey = KeyCode.Alpha4;
    public KeyCode testTreasure = KeyCode.Alpha5;
    public KeyCode testDoor = KeyCode.Alpha6;
    public KeyCode testEnemyAlert = KeyCode.Alpha7;

    void Start()
    {
        Debug.Log("=== AUDIO TESTER CONTROLS ===");
        Debug.Log("1 = Player Footstep");
        Debug.Log("2 = Player Run");
        Debug.Log("3 = Player Hide");
        Debug.Log("4 = Key Pickup");
        Debug.Log("5 = Treasure Pickup");
        Debug.Log("6 = Door Open");
        Debug.Log("7 = Enemy Alert");
        Debug.Log("Make sure AudioManager is set up first!");
    }

    void Update()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not found! Create AudioManager GameObject with AudioManager script.");
            return;
        }

        if (Input.GetKeyDown(testFootstep))
        {
            AudioManager.Instance.PlayPlayerFootstep();
            Debug.Log("Testing: Player Footstep");
        }

        if (Input.GetKeyDown(testRun))
        {
            AudioManager.Instance.PlayPlayerRun();
            Debug.Log("Testing: Player Run");
        }

        if (Input.GetKeyDown(testHide))
        {
            AudioManager.Instance.PlayPlayerHide();
            Debug.Log("Testing: Player Hide");
        }

        if (Input.GetKeyDown(testKey))
        {
            AudioManager.Instance.PlayKeyPickup();
            Debug.Log("Testing: Key Pickup");
        }

        if (Input.GetKeyDown(testTreasure))
        {
            AudioManager.Instance.PlayTreasurePickup();
            Debug.Log("Testing: Treasure Pickup");
        }

        if (Input.GetKeyDown(testDoor))
        {
            AudioManager.Instance.PlayDoorOpen();
            Debug.Log("Testing: Door Open");
        }

        if (Input.GetKeyDown(testEnemyAlert))
        {
            AudioManager.Instance.PlayEnemyAlert();
            Debug.Log("Testing: Enemy Alert");
        }
    }
}
