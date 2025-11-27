using UnityEngine;

public class AudioDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔊 AUDIO DIAGNOSTIC STARTING...");
        Invoke("RunDiagnostic", 1f); // Wait 1 second for everything to initialize
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("🎵 Manual test: Playing footstep sound...");
            TestFootstep();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("🔍 Running full diagnostic...");
            RunDiagnostic();
        }
    }

    void RunDiagnostic()
    {
        Debug.Log("=== AUDIO SYSTEM DIAGNOSTIC ===");
        
        // Check AudioManager
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL!");
            
            AudioManager manager = FindObjectOfType<AudioManager>();
            if (manager == null)
            {
                Debug.LogError("❌ No AudioManager found in scene!");
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager exists but Instance is null - check Awake()");
            }
            return;
        }
        
        Debug.Log("✅ AudioManager.Instance found");
        
        // Check Player Sounds
        var playerSounds = AudioManager.Instance.playerSounds;
        if (playerSounds == null)
        {
            Debug.LogError("❌ playerSounds array is null!");
            return;
        }
        
        Debug.Log($"✅ playerSounds array has {playerSounds.Length} elements");
        
        // Check PlayerFootstep specifically
        bool foundFootstep = false;
        for (int i = 0; i < playerSounds.Length; i++)
        {
            var sound = playerSounds[i];
            if (sound == null)
            {
                Debug.LogWarning($"⚠️ playerSounds[{i}] is null");
                continue;
            }
            
            Debug.Log($"Sound {i}: Name='{sound.name}', Clip={(sound.clip != null ? sound.clip.name : "NULL")}, Volume={sound.volume}");
            
            if (sound.name == "PlayerFootstep")
            {
                foundFootstep = true;
                if (sound.clip == null)
                {
                    Debug.LogError("❌ PlayerFootstep found but AudioClip is NULL!");
                }
                else
                {
                    Debug.Log($"✅ PlayerFootstep configured: {sound.clip.name}, Volume: {sound.volume}");
                    
                    // Check if AudioSource was created
                    if (sound.source == null)
                    {
                        Debug.LogError("❌ PlayerFootstep AudioSource is NULL!");
                    }
                    else
                    {
                        Debug.Log($"✅ PlayerFootstep AudioSource ready, Volume: {sound.source.volume}");
                    }
                }
            }
        }
        
        if (!foundFootstep)
        {
            Debug.LogError("❌ No sound named 'PlayerFootstep' found!");
        }
        
        // Check system audio
        Debug.Log($"🔊 Unity Master Volume: {AudioListener.volume}");
        
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("❌ No AudioListener found in scene!");
        }
        else
        {
            Debug.Log($"✅ AudioListener found on: {listener.gameObject.name}");
        }
        
        Debug.Log("=== DIAGNOSTIC COMPLETE ===");
        Debug.Log("Press '1' to test footstep sound");
    }
    
    void TestFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerFootstep();
            Debug.Log("✅ PlayPlayerFootstep() called");
        }
        else
        {
            Debug.LogError("❌ Cannot test - AudioManager.Instance is null");
        }
    }
}
