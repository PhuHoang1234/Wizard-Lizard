using UnityEngine;

public class Level1Setup : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🎮 Level 1 Setup starting...");
        
        // Small delay to ensure AudioManager is ready
        Invoke(nameof(SetupLevel1Audio), 0.3f);
    }
    
    void SetupLevel1Audio()
    {
        if (AudioManager.Instance != null)
        {
            // Ensure footsteps are enabled
            AudioManager.Instance.EnableFootsteps();
            
            // Ensure game music is playing
            AudioManager.Instance.PlayGameMusic();
            
            Debug.Log("✅ Level 1 setup complete - Footsteps enabled, game music playing");
        }
        else
        {
            Debug.LogWarning("⚠️ No AudioManager found in Level 1!");
        }
    }
}
