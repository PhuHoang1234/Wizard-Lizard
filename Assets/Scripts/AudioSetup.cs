using UnityEngine;

public class AudioSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(4, 8)]
    public string instructions = @"1. Add this script to an empty GameObject named 'AudioManager'
2. Assign AudioManager script to the same GameObject  
3. Add audio clips to the AudioManager component
4. Audio clips should be named:
   - PlayerFootstep, PlayerRun, PlayerHide
   - KeyPickup, TreasurePickup, DoorOpen  
   - EnemyAlert, EnemyChase, EnemyPatrol
   - BackgroundMusic for ambient music";

    void Start()
    {
        // This script is just for instructions, destroy after reading
        Destroy(this, 5f);
    }
}
