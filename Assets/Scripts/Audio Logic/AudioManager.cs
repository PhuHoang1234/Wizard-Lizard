using UnityEngine;

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    
    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    private AudioSource musicSource;

    [Header("Player Sound Effects")]
    public SoundEffect[] playerSounds;

    [Header("Enemy Sound Effects")]
    public SoundEffect[] enemySounds;

    [Header("Environment Sound Effects")]
    public SoundEffect[] environmentSounds;

    [Header("UI Sound Effects")]
    public SoundEffect[] uiSounds;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Debug.Log("🎵 AudioManager: Initializing singleton instance...");
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
            Debug.Log("✅ AudioManager: Initialization complete!");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager: Duplicate instance found, destroying...");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void InitializeAudio()
    {
        // Create music audio source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Initialize sound effect audio sources
        InitializeSoundArray(playerSounds);
        InitializeSoundArray(enemySounds);
        InitializeSoundArray(environmentSounds);
        InitializeSoundArray(uiSounds);
    }

    void InitializeSoundArray(SoundEffect[] sounds)
    {
        // Check if sounds array is null or empty
        if (sounds == null || sounds.Length == 0) 
        {
            Debug.LogWarning("Sound array is null or empty - skipping initialization");
            return;
        }

        Debug.Log($"🎵 Initializing {sounds.Length} sounds in array...");

        foreach (SoundEffect sound in sounds)
        {
            // Check if sound is not null and has a clip
            if (sound != null && sound.clip != null)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
                sound.source.playOnAwake = false;
                sound.source.spatialBlend = 0f; // Force 2D sound (not 3D positioned)
                Debug.Log($"✅ Initialized sound: '{sound.name}' with clip: '{sound.clip.name}' as 2D audio");
            }
            else if (sound != null && sound.clip == null)
            {
                Debug.LogWarning($"❌ Sound '{sound.name}' has no AudioClip assigned!");
            }
            else if (sound == null)
            {
                Debug.LogWarning("❌ Found null sound in array!");
            }
        }
    }

    // Music control
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
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

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    // Stop all audio
    public void StopAllSounds()
    {
        // Stop music
        StopBackgroundMusic();
        
        // Stop all sound effects
        StopAllSoundEffects(playerSounds);
        StopAllSoundEffects(enemySounds);
        StopAllSoundEffects(environmentSounds);
        StopAllSoundEffects(uiSounds);
    }

    void StopAllSoundEffects(SoundEffect[] sounds)
    {
        if (sounds == null) return;
        
        foreach (SoundEffect sound in sounds)
        {
            if (sound != null && sound.source != null)
            {
                sound.source.Stop();
            }
        }
    }

    // Sound effect control
    public void PlaySound(string soundName)
    {
        Debug.Log($"🔍 Searching for sound: '{soundName}'");
        SoundEffect sound = FindSound(soundName);
        if (sound != null && sound.source != null)
        {
            sound.source.Play();
            Debug.Log($"✅ Playing sound: '{soundName}' at volume {sound.volume}");
        }
        else if (sound == null)
        {
            Debug.LogWarning($"❌ Sound '{soundName}' not found! Make sure it's added to AudioManager with exact name.");
        }
        else
        {
            Debug.LogWarning($"❌ Sound '{soundName}' found but AudioSource is null!");
        }
    }

    public void PlaySoundAtPosition(string soundName, Vector3 position)
    {
        SoundEffect sound = FindSound(soundName);
        if (sound != null && sound.clip != null)
        {
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
        }
    }

    public void StopSound(string soundName)
    {
        SoundEffect sound = FindSound(soundName);
        if (sound != null && sound.source != null)
        {
            sound.source.Stop();
        }
    }

    SoundEffect FindSound(string name)
    {
        // Search in all sound arrays with null checks
        SoundEffect found = null;
        
        if (playerSounds != null)
        {
            found = System.Array.Find(playerSounds, sound => sound != null && sound.name == name);
            if (found != null) return found;
        }

        if (enemySounds != null)
        {
            found = System.Array.Find(enemySounds, sound => sound != null && sound.name == name);
            if (found != null) return found;
        }

        if (environmentSounds != null)
        {
            found = System.Array.Find(environmentSounds, sound => sound != null && sound.name == name);
            if (found != null) return found;
        }

        if (uiSounds != null)
        {
            found = System.Array.Find(uiSounds, sound => sound != null && sound.name == name);
        }
        return found;
    }

    // Helper methods for common sounds
    public void PlayPlayerFootstep()
    {
        PlaySound("PlayerFootstep");
    }

    public void PlayPlayerRun()
    {
        PlaySound("PlayerRun");
    }

    public void PlayPlayerHide()
    {
        PlaySound("PlayerHide");
    }

    public void PlayKeyPickup()
    {
        PlaySound("KeyPickup");
    }

    public void PlayTreasurePickup()
    {
        PlaySound("TreasurePickup");
    }

    public void PlayDoorOpen()
    {
        PlaySound("DoorOpen");
    }

    public void PlayEnemyAlert()
    {
        PlaySound("EnemyAlert");
    }

    public void PlayEnemyChase()
    {
        PlaySound("EnemyChase");
    }

    public void PlayEnemyPatrol()
    {
        PlaySound("EnemyPatrol");
    }

    public void PlayPlayerCaptured()
    {
        Debug.Log("🎵 PlayPlayerCaptured() called");
        PlaySound("PlayerCaptured");
    }
}
