using UnityEngine;

public class AudioSystemPrefab : MonoBehaviour
{
    [Header("Audio System Components")]
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private bool createOnStart = true;
    
    void Start()
    {
        if (createOnStart)
        {
            CreateCompleteAudioSystem();
        }
    }
    
    [ContextMenu("Create Complete Audio System")]
    public void CreateCompleteAudioSystem()
    {
        Debug.Log("🎵 Creating complete audio system...");
        
        // 1. Create AudioManager
        CreateAudioManager();
        
        // 2. Create UniversalAudioSetup
        CreateUniversalSetup();
        
        // 3. Create PlayerAudioIntegrator
        CreatePlayerIntegrator();
        
        Debug.Log("✅ Complete audio system created!");
    }
    
    void CreateAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            GameObject amObj = new GameObject("🎵 AudioManager");
            AudioManager am = amObj.AddComponent<AudioManager>();
            
            // Add sound generator
            FootstepSoundGenerator generator = amObj.AddComponent<FootstepSoundGenerator>();
            
            Debug.Log("✅ AudioManager created with sound generation");
        }
        else
        {
            Debug.Log("✅ AudioManager already exists");
        }
    }
    
    void CreateUniversalSetup()
    {
        UniversalAudioSetup existing = FindFirstObjectByType<UniversalAudioSetup>();
        if (existing == null)
        {
            GameObject setupObj = new GameObject("🌍 UniversalAudioSetup");
            setupObj.AddComponent<UniversalAudioSetup>();
            Debug.Log("✅ UniversalAudioSetup created");
        }
        else
        {
            Debug.Log("✅ UniversalAudioSetup already exists");
        }
    }
    
    void CreatePlayerIntegrator()
    {
        PlayerAudioIntegrator existing = FindFirstObjectByType<PlayerAudioIntegrator>();
        if (existing == null)
        {
            GameObject integratorObj = new GameObject("🎮 PlayerAudioIntegrator");
            integratorObj.AddComponent<PlayerAudioIntegrator>();
            Debug.Log("✅ PlayerAudioIntegrator created");
        }
        else
        {
            Debug.Log("✅ PlayerAudioIntegrator already exists");
        }
    }
    
    [ContextMenu("Test Audio System")]
    public void TestAudioSystem()
    {
        Debug.Log("🔍 Testing audio system...");
        
        // Test AudioManager
        if (AudioManager.Instance != null)
        {
            Debug.Log("✅ AudioManager: WORKING");
            
            if (AudioManager.Instance.footstepSound != null)
            {
                Debug.Log("✅ Footstep Sound: ASSIGNED");
            }
            else
            {
                Debug.LogWarning("⚠️ Footstep Sound: NOT ASSIGNED");
            }
        }
        else
        {
            Debug.LogError("❌ AudioManager: NOT FOUND");
        }
        
        // Test Player
        PlayerController3D player = FindFirstObjectByType<PlayerController3D>();
        if (player != null)
        {
            Debug.Log($"✅ Player: FOUND ({player.gameObject.name})");
        }
        else
        {
            Debug.LogWarning("⚠️ Player: NOT FOUND");
        }
        
        Debug.Log("🔍 Audio system test completed!");
    }
}
