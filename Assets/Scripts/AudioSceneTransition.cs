using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSceneTransition : MonoBehaviour
{
    [Header("Scene Audio Transition")]
    public bool debugMessages = true;
    
    void Start()
    {
        // This script should be in the game scene
        SetupGameSceneAudio();
    }
    
    void SetupGameSceneAudio()
    {
        if (debugMessages)
            Debug.Log("🎮 Setting up audio for game scene...");
        
        // Check if AudioManager exists from main menu
        if (AudioManager.Instance != null)
        {
            if (debugMessages)
                Debug.Log("✅ AudioManager found from main menu - switching to game music");
            
            // Switch to game music
            AudioManager.Instance.SwitchToGameMusic();
            
            // Ensure footstep audio is ready
            if (AudioManager.Instance.footstepSource == null)
            {
                SetupMissingAudioSources();
            }
        }
        else
        {
            if (debugMessages)
                Debug.LogWarning("⚠️ No AudioManager found! Creating one for game scene...");
            
            CreateGameSceneAudioManager();
        }
        
        // Test footstep setup
        TestFootstepSetup();
    }
    
    void SetupMissingAudioSources()
    {
        if (debugMessages)
            Debug.Log("🔧 Setting up missing audio sources...");
        
        GameObject audioManagerGO = AudioManager.Instance.gameObject;
        
        // Check and create missing audio sources
        AudioSource[] sources = audioManagerGO.GetComponents<AudioSource>();
        
        while (sources.Length < 3)
        {
            audioManagerGO.AddComponent<AudioSource>();
            sources = audioManagerGO.GetComponents<AudioSource>();
        }
        
        // Reassign sources to AudioManager
        AudioManager audioManager = AudioManager.Instance;
        
        if (audioManager.musicSource == null && sources.Length > 0)
            audioManager.musicSource = sources[0];
        
        if (audioManager.sfxSource == null && sources.Length > 1)
            audioManager.sfxSource = sources[1];
        
        if (audioManager.footstepSource == null && sources.Length > 2)
        {
            audioManager.footstepSource = sources[2];
            audioManager.footstepSource.loop = true;
            audioManager.footstepSource.volume = 0.6f;
        }
        
        if (debugMessages)
            Debug.Log("✅ Audio sources setup complete!");
    }
    
    void CreateGameSceneAudioManager()
    {
        // Create AudioManager GameObject
        GameObject audioManagerGO = new GameObject("AudioManager");
        DontDestroyOnLoad(audioManagerGO);
        
        // Add AudioManager script
        AudioManager audioManager = audioManagerGO.AddComponent<AudioManager>();
        
        // Add three AudioSource components
        AudioSource musicSource = audioManagerGO.AddComponent<AudioSource>();
        AudioSource sfxSource = audioManagerGO.AddComponent<AudioSource>();
        AudioSource footstepSource = audioManagerGO.AddComponent<AudioSource>();
        
        // Configure AudioSources
        musicSource.loop = true;
        musicSource.volume = 0.7f;
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
        footstepSource.loop = true;
        footstepSource.volume = 0.6f;
        
        if (debugMessages)
            Debug.Log("✅ Created new AudioManager for game scene!");
    }
    
    void TestFootstepSetup()
    {
        // Test if footsteps can be played
        if (AudioManager.Instance != null && AudioManager.Instance.footstepSource != null)
        {
            if (debugMessages)
                Debug.Log("✅ Footstep audio is ready!");
        }
        else
        {
            if (debugMessages)
                Debug.LogError("❌ Footstep audio is NOT ready!");
        }
    }
    
    // Call this when player starts moving to test footsteps
    public void TestPlayFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstep();
            if (debugMessages)
                Debug.Log("🦶 Testing footstep sound...");
        }
    }
}
