using UnityEngine;

[System.Serializable]
public class MainMenuAudioSetup : MonoBehaviour
{
    [Header("Main Menu Audio Setup")]
    public AudioClip mainMenuMusicClip;
    
    void Start()
    {
        SetupMainMenuAudio();
    }
    
    void SetupMainMenuAudio()
    {
        // Check if AudioManager already exists
        if (AudioManager.Instance == null)
        {
            Debug.Log("🎵 Creating AudioManager for Main Menu...");
            CreateAudioManagerForMainMenu();
        }
        else
        {
            Debug.Log("🎵 AudioManager found! Setting up main menu music...");
            ConfigureMainMenuMusic();
        }
    }
    
    void CreateAudioManagerForMainMenu()
    {
        // Create AudioManager GameObject
        GameObject audioManagerGO = new GameObject("AudioManager");
        
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
        
        // Wait a frame for AudioManager to initialize, then configure
        Invoke(nameof(ConfigureMainMenuMusic), 0.1f);
        
        Debug.Log("✅ AudioManager created for Main Menu!");
    }
    
    void ConfigureMainMenuMusic()
    {
        if (AudioManager.Instance != null && mainMenuMusicClip != null)
        {
            // Assign the main menu music
            AudioManager.Instance.mainMenuMusic = mainMenuMusicClip;
            
            // Play the main menu music
            AudioManager.Instance.SwitchToMainMenuMusic();
            
            Debug.Log("🎶 Main menu music should now be playing!");
        }
        else if (mainMenuMusicClip == null)
        {
            Debug.LogError("❌ Main Menu Music Clip is not assigned! Please drag your MP3 file to this script.");
        }
    }
}
