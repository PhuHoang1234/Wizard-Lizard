using UnityEngine;

public class LevelStartAudioSetup : MonoBehaviour
{
    void Start()
    {
        // Enable footsteps when any level starts
        Invoke(nameof(EnableGameplayAudio), 0.2f);
    }
    
    void EnableGameplayAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.EnableFootsteps();
            Debug.Log("🎮 Level started - Footsteps enabled for gameplay");
        }
    }
}
