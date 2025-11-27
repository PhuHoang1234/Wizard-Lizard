using UnityEngine;

public class SystemAudioTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== SYSTEM AUDIO TEST ===");
        Debug.Log("Press 'B' to test system audio with a loud beep");
        Debug.Log("Press 'M' to check all audio settings");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TestSystemAudio();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            CheckAllAudioSettings();
        }
    }
    
    void TestSystemAudio()
    {
        Debug.Log("🔊 TESTING SYSTEM AUDIO - You should hear a LOUD beep!");
        
        // Create a very loud, obvious test sound
        GameObject testObj = new GameObject("SystemAudioTest");
        AudioSource testSource = testObj.AddComponent<AudioSource>();
        
        // Create a loud beep
        AudioClip beep = CreateLoudBeep();
        testSource.clip = beep;
        testSource.volume = 1.0f; // Maximum volume
        testSource.spatialBlend = 0f; // 2D sound (not 3D positioned)
        testSource.Play();
        
        Debug.Log("🔊 If you don't hear this LOUD beep, your computer audio is the problem!");
        
        // Clean up
        Destroy(testObj, 2f);
    }
    
    void CheckAllAudioSettings()
    {
        Debug.Log("🔍 === CHECKING ALL AUDIO SETTINGS ===");
        
        // Unity audio settings
        Debug.Log($"🔊 AudioListener.volume (Unity master): {AudioListener.volume}");
        Debug.Log($"🔊 AudioListener.pause: {AudioListener.pause}");
        
        // Find AudioListener
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("❌ PROBLEM: No AudioListener found!");
        }
        else
        {
            Debug.Log($"✅ AudioListener found on: {listener.gameObject.name}");
            Debug.Log($"🔊 AudioListener enabled: {listener.enabled}");
            Debug.Log($"🔊 AudioListener gameObject active: {listener.gameObject.activeInHierarchy}");
        }
        
        // Check AudioManager volume
        if (AudioManager.Instance != null)
        {
            var enemySounds = AudioManager.Instance.enemySounds;
            if (enemySounds != null)
            {
                foreach (var sound in enemySounds)
                {
                    if (sound != null && sound.name == "EnemyAlert")
                    {
                        Debug.Log($"🎵 EnemyAlert volume setting: {sound.volume}");
                        if (sound.source != null)
                        {
                            Debug.Log($"🎵 EnemyAlert AudioSource volume: {sound.source.volume}");
                            Debug.Log($"🎵 EnemyAlert AudioSource enabled: {sound.source.enabled}");
                            Debug.Log($"🎵 EnemyAlert AudioSource mute: {sound.source.mute}");
                        }
                        break;
                    }
                }
            }
        }
        
        // Check all AudioSources in scene
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        Debug.Log($"🎵 Total AudioSources in scene: {allSources.Length}");
        
        int playingSources = 0;
        foreach (var source in allSources)
        {
            if (source.isPlaying)
            {
                playingSources++;
                Debug.Log($"▶️ PLAYING: {source.gameObject.name} - Volume: {source.volume}");
            }
        }
        Debug.Log($"▶️ Currently playing AudioSources: {playingSources}");
    }
    
    AudioClip CreateLoudBeep()
    {
        int sampleRate = 44100;
        float frequency = 800f; // Higher frequency = more noticeable
        float duration = 1f; // Longer duration
        
        AudioClip clip = AudioClip.Create("LOUD_BEEP", (int)(duration * sampleRate), 1, sampleRate, false);
        float[] samples = new float[(int)(duration * sampleRate)];
        
        for (int i = 0; i < samples.Length; i++)
        {
            // Create a loud sine wave
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate) * 0.8f; // 80% volume
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
}
