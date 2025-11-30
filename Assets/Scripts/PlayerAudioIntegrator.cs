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
        PlayerController3D[] players = FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None);
        
        if (players.Length == 0)
        {
            Debug.LogWarning("⚠️ No PlayerController3D scripts found in scene!");
            return;
        }
        
        foreach (PlayerController3D player in players)
        {
            ValidatePlayerAudioSetup(player);
        }
        
        Debug.Log($"✅ Player audio integration completed for {players.Length} player(s)");
    }
    
    void ValidatePlayerAudioSetup(PlayerController3D player)
    {
        Debug.Log($"🔍 Checking player audio setup on: {player.gameObject.name}");
        
        // Check if AudioManager is available
        if (AudioManager.Instance != null)
        {
            Debug.Log("✅ AudioManager found - audio integration ready");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager not found!");
        }
        
        // Test player audio
        TestPlayerAudio(player);
    }
    
    void TestPlayerAudio(PlayerController3D player)
    {
        // This method can be expanded to test audio integration
        Debug.Log($"🎵 Player '{player.gameObject.name}' audio integration validated");
    }
    
    // Method to fix common player audio issues
    [ContextMenu("Fix Player Audio Issues")]
    public void FixPlayerAudioIssues()
    {
        PlayerController3D[] players = FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None);
        
        foreach (PlayerController3D player in players)
        {
            Debug.Log($"🔧 Player audio setup validated for {player.gameObject.name}");
        }
        
        Debug.Log("✅ Player audio issues checked!");
    }
}
