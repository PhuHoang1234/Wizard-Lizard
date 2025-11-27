using UnityEngine;

public class AudioDiagnosticAdvanced : MonoBehaviour
{
    void Start()
    {
        // Wait for AudioManager to initialize, then run diagnostics
        Invoke("RunComprehensiveDiagnostic", 2f);
    }

    void Update()
    {
        // Press 'D' for detailed diagnostic
        if (Input.GetKeyDown(KeyCode.D))
        {
            RunComprehensiveDiagnostic();
        }
        
        // Press 'T' to test PlayerCaptured sound directly
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestPlayerCapturedDirect();
        }
        
        // Press 'V' to check audio volume settings
        if (Input.GetKeyDown(KeyCode.V))
        {
            CheckVolumeSettings();
        }
    }

    void RunComprehensiveDiagnostic()
    {
        Debug.Log("🔍 === COMPREHENSIVE AUDIO DIAGNOSTIC ===");
        
        // Check AudioManager Instance
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ CRITICAL: AudioManager.Instance is NULL!");
            
            AudioManager manager = FindObjectOfType<AudioManager>();
            if (manager == null)
            {
                Debug.LogError("❌ CRITICAL: No AudioManager found in scene!");
                Debug.LogError("💡 SOLUTION: Make sure you have an AudioManager GameObject in your scene");
                return;
            }
            else
            {
                Debug.LogError("❌ CRITICAL: AudioManager exists but Instance is null - check Awake()");
                return;
            }
        }
        
        Debug.Log("✅ AudioManager.Instance found");
        
        // Check Enemy Sounds Array
        var enemySounds = AudioManager.Instance.enemySounds;
        if (enemySounds == null)
        {
            Debug.LogError("❌ enemySounds array is null!");
            return;
        }
        
        Debug.Log($"✅ enemySounds array has {enemySounds.Length} elements");
        
        // Look for PlayerCaptured sound specifically
        bool foundPlayerCaptured = false;
        for (int i = 0; i < enemySounds.Length; i++)
        {
            var sound = enemySounds[i];
            if (sound == null)
            {
                Debug.LogWarning($"⚠️ enemySounds[{i}] is null");
                continue;
            }
            
            Debug.Log($"🎵 Enemy Sound {i}: Name='{sound.name}', Clip={(sound.clip != null ? sound.clip.name : "NULL")}, Volume={sound.volume}");
            
            if (sound.name == "PlayerCaptured")
            {
                foundPlayerCaptured = true;
                
                if (sound.clip == null)
                {
                    Debug.LogError("❌ FOUND ISSUE: PlayerCaptured sound found but AudioClip is NULL!");
                    Debug.LogError("💡 SOLUTION: Drag an audio file to the 'Clip' field in AudioManager");
                    return;
                }
                else
                {
                    Debug.Log($"✅ PlayerCaptured sound configured: {sound.clip.name}, Volume: {sound.volume}");
                    
                    if (sound.source == null)
                    {
                        Debug.LogError("❌ FOUND ISSUE: PlayerCaptured AudioSource is NULL!");
                        Debug.LogError("💡 SOLUTION: AudioManager initialization failed");
                        return;
                    }
                    else
                    {
                        Debug.Log($"✅ PlayerCaptured AudioSource ready, Volume: {sound.source.volume}");
                    }
                }
            }
        }
        
        if (!foundPlayerCaptured)
        {
            Debug.LogError("❌ FOUND ISSUE: No sound named 'PlayerCaptured' found!");
            Debug.LogError("💡 SOLUTION: Add a sound to enemySounds array with name EXACTLY 'PlayerCaptured'");
            return;
        }
        
        // Check Audio System
        CheckAudioSystem();
        
        Debug.Log("🎯 === DIAGNOSTIC COMPLETE ===");
        Debug.Log("💡 If all checks passed, try pressing 'T' to test PlayerCaptured sound");
    }
    
    void TestPlayerCapturedDirect()
    {
        Debug.Log("🧪 === TESTING PlayerCaptured SOUND DIRECTLY ===");
        
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ Cannot test - AudioManager.Instance is null");
            return;
        }
        
        Debug.Log("🎵 Calling PlayPlayerCaptured()...");
        AudioManager.Instance.PlayPlayerCaptured();
        Debug.Log("✅ PlayPlayerCaptured() called - check if you hear sound now");
    }
    
    void CheckVolumeSettings()
    {
        Debug.Log("🔊 === VOLUME SETTINGS CHECK ===");
        
        // Check Unity audio settings
        Debug.Log($"🔊 Unity Master Volume (AudioListener.volume): {AudioListener.volume}");
        
        // Find AudioListener
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("❌ No AudioListener found in scene!");
            Debug.LogError("💡 SOLUTION: Make sure your Main Camera has AudioListener component");
        }
        else
        {
            Debug.Log($"✅ AudioListener found on: {listener.gameObject.name}");
        }
        
        // Check if any AudioSources are playing
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        Debug.Log($"🎵 Found {allSources.Length} AudioSources in scene");
        
        int playingSources = 0;
        foreach (var source in allSources)
        {
            if (source.isPlaying)
            {
                playingSources++;
                Debug.Log($"▶️ Playing: {source.gameObject.name} - Clip: {(source.clip ? source.clip.name : "None")} - Volume: {source.volume}");
            }
        }
        
        Debug.Log($"🎵 Currently playing: {playingSources} AudioSources");
        
        // Test system audio with a beep
        Debug.Log("🔔 Playing test beep in 2 seconds...");
        Invoke("PlayTestBeep", 2f);
    }
    
    void PlayTestBeep()
    {
        // Create a temporary audio source to test if audio works at all
        GameObject tempGO = new GameObject("TempAudioTest");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        
        // Generate a simple sine wave beep
        AudioClip beepClip = CreateBeepClip();
        tempSource.clip = beepClip;
        tempSource.volume = 0.5f;
        tempSource.Play();
        
        Debug.Log("🔔 Test beep playing! If you don't hear this, check your computer's audio settings");
        
        // Clean up after 2 seconds
        Destroy(tempGO, 2f);
    }
    
    AudioClip CreateBeepClip()
    {
        int sampleRate = 44100;
        float frequency = 440f; // A note
        float duration = 0.5f;
        
        AudioClip clip = AudioClip.Create("TestBeep", (int)(duration * sampleRate), 1, sampleRate, false);
        float[] samples = new float[(int)(duration * sampleRate)];
        
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate) * 0.3f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    void CheckAudioSystem()
    {
        Debug.Log("🔊 === AUDIO SYSTEM CHECK ===");
        Debug.Log($"🔊 Unity Master Volume: {AudioListener.volume}");
        
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("❌ No AudioListener found!");
        }
        else
        {
            Debug.Log($"✅ AudioListener found on: {listener.gameObject.name}");
        }
    }
}
