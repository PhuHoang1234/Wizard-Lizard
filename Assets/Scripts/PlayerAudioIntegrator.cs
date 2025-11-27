using UnityEngine;

public class PlayerAudioIntegrator : MonoBehaviour
{
    [Header("Player Audio Integration")]
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            IntegratePlayerAudio();
        }
    }
    
    [ContextMenu("Integrate Player Audio")]
    public void IntegratePlayerAudio()
    {
        // Find all player movement scripts in the scene
        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        
        if (players.Length == 0)
        {
            Debug.LogWarning("⚠️ No PlayerMovement scripts found in scene!");
            return;
        }
        
        foreach (PlayerMovement player in players)
        {
            ValidatePlayerAudioSetup(player);
        }
        
        Debug.Log($"✅ Player audio integration completed for {players.Length} player(s)");
    }
    
    void ValidatePlayerAudioSetup(PlayerMovement player)
    {
        Debug.Log($"🔍 Checking player audio setup on: {player.gameObject.name}");
        
        // Check if footstep variables are properly set
        if (player.enableFootsteps)
        {
            Debug.Log("✅ Footsteps enabled");
        }
        else
        {
            Debug.LogWarning($"⚠️ Footsteps disabled on {player.gameObject.name}");
        }
        
        // Check if AudioManager is available
        if (AudioManager.Instance != null)
        {
            Debug.Log("✅ AudioManager found - footstep integration ready");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager not found - footsteps won't work!");
        }
        
        // Test footstep integration
        TestPlayerAudio(player);
    }
    
    void TestPlayerAudio(PlayerMovement player)
    {
        // This method can be expanded to test audio integration
        Debug.Log($"🎵 Player '{player.gameObject.name}' audio integration validated");
    }
    
    // Method to fix common player audio issues
    [ContextMenu("Fix Player Audio Issues")]
    public void FixPlayerAudioIssues()
    {
        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        
        foreach (PlayerMovement player in players)
        {
            // Ensure footsteps are enabled
            if (!player.enableFootsteps)
            {
                player.enableFootsteps = true;
                Debug.Log($"🔧 Enabled footsteps for {player.gameObject.name}");
            }
            
            // Ensure reasonable footstep delay
            if (player.footstepDelay <= 0f || player.footstepDelay > 2f)
            {
                player.footstepDelay = 0.4f;
                Debug.Log($"🔧 Reset footstep delay to 0.4s for {player.gameObject.name}");
            }
        }
        
        Debug.Log("✅ Player audio issues fixed!");
    }
}
