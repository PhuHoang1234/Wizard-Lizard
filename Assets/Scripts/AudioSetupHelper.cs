using UnityEngine;

public class AudioSetupHelper : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool generateFootsteps = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupAudioSystem();
        }
    }
    
    [ContextMenu("Setup Audio System")]
    public void SetupAudioSystem()
    {
        Debug.Log("Setting up audio system...");
        
        // Find or create AudioManager
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();
            Debug.Log("Created AudioManager");
        }
        
        // Generate footstep sounds if needed
        if (generateFootsteps)
        {
            FootstepSoundGenerator generator = FindFirstObjectByType<FootstepSoundGenerator>();
            if (generator == null)
            {
                GameObject generatorObj = new GameObject("FootstepGenerator");
                generator = generatorObj.AddComponent<FootstepSoundGenerator>();
            }
            
            generator.GenerateAllSounds();
            Debug.Log("Generated all sound effects (footsteps, pickup, door)");
        }
        
        // Try to assign background music from existing files
        AssignBackgroundMusic(audioManager);
        
        Debug.Log("Audio system setup complete!");
    }
    
    private void AssignBackgroundMusic(AudioManager audioManager)
    {
        // Try to load music from Resources or Assets
        AudioClip musicClip = null;
        
        // First try to load from Resources
        musicClip = Resources.Load<AudioClip>("Music/videoplayback");
        
        if (musicClip == null)
        {
            // Try alternative paths
            string[] possiblePaths = {
                "videoplayback",
                "Audio/Music/videoplayback",
                "Music/videoplayback"
            };
            
            foreach (string path in possiblePaths)
            {
                musicClip = Resources.Load<AudioClip>(path);
                if (musicClip != null) break;
            }
        }
        
        if (musicClip != null)
        {
            audioManager.backgroundMusic = musicClip;
            Debug.Log("Assigned background music: " + musicClip.name);
        }
        else
        {
            Debug.LogWarning("Could not find background music file. Please assign manually in AudioManager.");
        }
    }
}
