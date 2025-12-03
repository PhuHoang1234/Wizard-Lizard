using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton pattern for easy access
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource footstepSource; // Dedicated source for footsteps that we can control
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioClip mainMenuMusic;
    
    [Header("Sound Effects")]
    public AudioClip footstepSound;
    public AudioClip pickupSound;
    public AudioClip doorSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Control")]
    public bool footstepsEnabled = true; // Flag to control footstep playback

    private void Awake()
    {
        // Singleton pattern - ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Auto-create audio sources if not assigned
            SetupAudioSources();
            
            // Listen for scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from scene events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🔄 Scene loaded: {scene.name} - Restarting audio setup");
        Debug.Log($"🔄 AudioManager Instance: {(Instance != null ? "Found" : "NULL")}");
        Debug.Log($"🔄 Audio Sources - Music: {(musicSource != null ? "Found" : "NULL")}, Footsteps: {(footstepSource != null ? "Found" : "NULL")}");
        
        // Small delay to let everything initialize
        Invoke(nameof(SetupSceneAudio), 0.2f);
    }

    private void Start()
    {
        // Load audio files from Resources if not assigned
        LoadAudioFilesFromResources();
        
        // Wait a moment for everything to load, then setup audio
        Invoke(nameof(SetupSceneAudio), 0.1f);
    }
    
    void SetupSceneAudio()
    {
        // Check what scene we're in and start appropriate audio
        string sceneName = SceneManager.GetActiveScene().name;
        
        Debug.Log($"🎵 Setting up audio for scene: {sceneName}");
        Debug.Log($"🎵 backgroundMusic: {(backgroundMusic != null ? backgroundMusic.name : "NULL")}");
        Debug.Log($"🎵 musicSource: {(musicSource != null ? "Found" : "NULL")}");
        
        // Stop any current music first
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("🔇 Stopped current music");
        }
        
        if (sceneName == "MainMenu")
        {
            // Main menu - play main menu music, disable footsteps
            PlayMainMenuMusic();
            DisableFootsteps();
            Debug.Log("🏠 AudioManager: Main menu setup complete");
        }
        else
        {
            // Game level - play game music, enable footsteps
            if (backgroundMusic != null)
            {
                PlayGameMusic();
                Debug.Log("✅ Game music started");
            }
            else
            {
                Debug.LogWarning("❌ Cannot start game music - backgroundMusic is null!");
                // Try to force load the music
                LoadAudioFilesFromResources();
                Invoke(nameof(RetryGameMusic), 0.1f);
            }
            
            EnableFootsteps();
            Debug.Log($"🎮 AudioManager: Game level setup complete for {sceneName}");
        }
    }
    
    void RetryGameMusic()
    {
        if (backgroundMusic != null)
        {
            PlayGameMusic();
            Debug.Log("✅ Game music started after reload");
        }
        else
        {
            Debug.LogError("❌ Still no background music after reload!");
        }
    }

    private void LoadAudioFilesFromResources()
    {
        // Load background music if not assigned - use the correct Level 1 music
        if (backgroundMusic == null)
        {
            backgroundMusic = Resources.Load<AudioClip>("Music/ambient-soundscapes-007-space-atmosphere-304974");
            
            if (backgroundMusic != null)
            {
                Debug.Log("✅ Loaded Level 1 ambient music: " + backgroundMusic.name);
            }
            else
            {
                Debug.LogWarning("⚠️ Could not load Level 1 ambient music from Resources/Music/ambient-soundscapes-007-space-atmosphere-304974");
            }
        }
        
        // Load footstep sound if not assigned
        if (footstepSound == null)
        {
            footstepSound = Resources.Load<AudioClip>("Audio/SFX/Footsteps/concrete-footsteps-6752");
            if (footstepSound != null)
            {
                Debug.Log("✅ Loaded footstep sound: " + footstepSound.name);
            }
            else
            {
                Debug.LogWarning("⚠️ Could not load footstep sound from Resources/Audio/SFX/Footsteps/concrete-footsteps-6752");
            }
        }
    }

    private void SetupAudioSources()
    {
        // Create Music Audio Source if not assigned
        if (musicSource == null)
        {
            GameObject musicObject = new GameObject("Music Source");
            musicObject.transform.SetParent(this.transform);
            musicSource = musicObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        
        // Create SFX Audio Source if not assigned
        if (sfxSource == null)
        {
            GameObject sfxObject = new GameObject("SFX Source");
            sfxObject.transform.SetParent(this.transform);
            sfxSource = sfxObject.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;
        }
        
        // Create dedicated Footstep Audio Source
        if (footstepSource == null)
        {
            GameObject footstepObject = new GameObject("Footstep Source");
            footstepObject.transform.SetParent(this.transform);
            footstepSource = footstepObject.AddComponent<AudioSource>();
            footstepSource.volume = sfxVolume;
            footstepSource.loop = true; // Enable looping
            footstepSource.playOnAwake = false; // Don't play automatically
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayFootstep()
    {
        if (footstepSound != null && footstepSource != null && footstepsEnabled)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepSound;
                footstepSource.Play();
                Debug.Log($"🦶 Started footstep loop: {footstepSound.name}");
            }
        }
        else
        {
            if (footstepSound == null)
                Debug.LogWarning("❌ No footstep sound assigned to AudioManager!");
            if (footstepSource == null)
                Debug.LogWarning("❌ No footstep AudioSource found in AudioManager!");
        }
    }
    
    public void StopFootstep()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
            Debug.Log("🛑 STOPPED footstep audio source!");
        }
    }

    public void StopAllAudio()
    {
        // Stop all audio sources
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("🔇 Stopped background music");
        }
        
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
            Debug.Log("🔇 Stopped footstep audio");
        }
        
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
            Debug.Log("🔇 Stopped SFX audio");
        }
        
        // Disable footsteps to prevent restart
        footstepsEnabled = false;
        Debug.Log("🔇 ALL AUDIO STOPPED - Footsteps disabled");
    }

    public void EnableFootsteps()
    {
        footstepsEnabled = true;
        Debug.Log("👟 Footsteps re-enabled");
    }
    
    public void DisableFootsteps()
    {
        footstepsEnabled = false;
        StopFootstep();
        Debug.Log("🚫 Footsteps disabled");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
            else
            {
                musicSource.UnPause();
            }
        }
    }
    
    // Methods for main menu music
    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null && musicSource != null)
        {
            if (musicSource.clip != mainMenuMusic || !musicSource.isPlaying)
            {
                musicSource.clip = mainMenuMusic;
                musicSource.Play();
                Debug.Log("🎵 Playing main menu music");
            }
        }
        else if (mainMenuMusic == null)
        {
            Debug.LogWarning("⚠️ No main menu music assigned to AudioManager!");
        }
    }
    
    public void PlayGameMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            if (musicSource.clip != backgroundMusic || !musicSource.isPlaying)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
                Debug.Log("🎵 Playing game music");
            }
        }
        else if (backgroundMusic == null)
        {
            Debug.LogWarning("⚠️ No game music assigned to AudioManager!");
        }
    }
    
    public void SwitchToMainMenuMusic()
    {
        StopBackgroundMusic();
        PlayMainMenuMusic();
    }
    
    public void SwitchToGameMusic()
    {
        StopBackgroundMusic();
        PlayGameMusic();
        
        // Enable footsteps when entering gameplay
        EnableFootsteps();
        Debug.Log("🎮 Switched to game music and enabled footsteps");
    }

    public void ForceRestartGameAudio()
    {
        Debug.Log("🔄 Force restarting game audio...");
        
        // Stop everything first
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        
        // Reload audio files
        LoadAudioFilesFromResources();
        
        // Start game audio
        PlayGameMusic();
        EnableFootsteps();
        
        Debug.Log("✅ Game audio force restarted!");
    }
}