using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton pattern for easy access
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource footstepSource; // Dedicated source for footsteps that we can control
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    
    [Header("Sound Effects")]
    public AudioClip footstepSound;
    public AudioClip pickupSound;
    public AudioClip doorSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton pattern - ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Auto-create audio sources if not assigned
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start playing background music
        PlayBackgroundMusic();
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
        if (footstepSound != null && footstepSource != null)
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
}