using UnityEngine;

public class Level3Setup : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🐲 Level 3 Setup starting...");
        
        // Setup Level 3 with a small delay
        Invoke(nameof(SetupLevel3), 0.5f);
    }
    
    void SetupLevel3()
    {
        Debug.Log("🔧 Setting up Level 3 components...");
        
        // Ensure AudioManager exists and footsteps are enabled
        SetupAudio();
        
        // Check boss components
        SetupBoss();
        
        // Ensure player is ready
        SetupPlayer();
        
        Debug.Log("✅ Level 3 setup complete!");
    }
    
    void SetupAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.EnableFootsteps();
            AudioManager.Instance.PlayGameMusic();
            Debug.Log("🎵 Level 3 audio setup complete");
        }
        else
        {
            Debug.LogWarning("⚠️ No AudioManager found in Level 3!");
        }
    }
    
    void SetupBoss()
    {
        BossWyvernSimpleAI boss = FindObjectOfType<BossWyvernSimpleAI>();
        if (boss != null)
        {
            // Boss starts disabled, intro trigger enables it
            boss.enabled = false;
            Debug.Log("🐲 Boss found and disabled (waiting for intro trigger)");
        }
        else
        {
            Debug.LogWarning("⚠️ No boss found in Level 3!");
        }
        
        BossIntroTrigger introTrigger = FindObjectOfType<BossIntroTrigger>();
        if (introTrigger != null)
        {
            Debug.Log("🎬 Boss intro trigger found and ready");
        }
        else
        {
            Debug.LogWarning("⚠️ No boss intro trigger found in Level 3!");
        }
    }
    
    void SetupPlayer()
    {
        PlayerController3D player = FindObjectOfType<PlayerController3D>();
        if (player != null)
        {
            Debug.Log("🎮 Player found and ready for Level 3");
        }
        else
        {
            Debug.LogWarning("⚠️ No player found in Level 3!");
        }
    }
}
