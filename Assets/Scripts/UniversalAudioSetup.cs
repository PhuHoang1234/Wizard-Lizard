using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalAudioSetup : MonoBehaviour
{
    [Header("Universal Audio Setup")]
    [SerializeField] private bool setupOnSceneLoad = true;
    [SerializeField] private bool verboseLogging = true;
    
    void Start()
    {
        // Set this to persist across scenes
        DontDestroyOnLoad(gameObject);
        
        if (setupOnSceneLoad)
        {
            SetupAudioForCurrentScene();
        }
        
        // Subscribe to scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (verboseLogging)
            Debug.Log($"🎮 Scene '{scene.name}' loaded - setting up audio...");
            
        SetupAudioForCurrentScene();
    }
    
    [ContextMenu("Setup Audio For Current Scene")]
    public void SetupAudioForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (verboseLogging)
            Debug.Log($"🎵 Setting up audio system for {currentScene}...");
        
        // 1. Ensure AudioManager exists
        EnsureAudioManager();
        
        // 2. Check for player movement script
        CheckPlayerMovement();
        
        // 3. Setup sound generation if needed
        SetupSoundGeneration();
        
        // 4. Fix scene-specific issues
        FixSceneIssues();
        
        if (verboseLogging)
            Debug.Log($"✅ Audio setup completed for {currentScene}!");
    }
    
    void EnsureAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            // Check if there's an AudioManager in the scene
            AudioManager existingAM = FindFirstObjectByType<AudioManager>();
            if (existingAM == null)
            {
                Debug.Log("🔧 Creating AudioManager for this scene...");
                GameObject amObj = new GameObject("AudioManager");
                amObj.AddComponent<AudioManager>();
                
                // Add sound generation
                amObj.AddComponent<FootstepSoundGenerator>();
            }
        }
        else
        {
            if (verboseLogging)
                Debug.Log("✅ AudioManager already exists");
        }
    }
    
    void CheckPlayerMovement()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            if (verboseLogging)
                Debug.Log($"✅ Player movement found: {player.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No PlayerMovement script found in this scene!");
        }
    }
    
    void SetupSoundGeneration()
    {
        FootstepSoundGenerator generator = FindFirstObjectByType<FootstepSoundGenerator>();
        if (generator == null)
        {
            // Create generator if AudioManager exists
            if (AudioManager.Instance != null)
            {
                generator = AudioManager.Instance.gameObject.AddComponent<FootstepSoundGenerator>();
                Debug.Log("🔧 Added FootstepSoundGenerator to AudioManager");
            }
        }
        
        if (generator != null)
        {
            generator.GenerateAllSounds();
            if (verboseLogging)
                Debug.Log("✅ Sound effects generated");
        }
    }
    
    void FixSceneIssues()
    {
        // Fix common Unity issues across all scenes
        FixAudioListeners();
        FixEventSystems();
    }
    
    void FixAudioListeners()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        
        if (listeners.Length > 1)
        {
            Debug.LogWarning($"⚠️ Found {listeners.Length} Audio Listeners! Fixing...");
            
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
                if (verboseLogging)
                    Debug.Log($"🔧 Disabled extra Audio Listener on '{listeners[i].gameObject.name}'");
            }
        }
    }
    
    void FixEventSystems()
    {
        UnityEngine.EventSystems.EventSystem[] eventSystems = 
            FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"⚠️ Found {eventSystems.Length} Event Systems! Fixing...");
            
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (verboseLogging)
                    Debug.Log($"🔧 Removing extra Event System from '{eventSystems[i].gameObject.name}'");
                Destroy(eventSystems[i].gameObject);
            }
        }
    }
}
