using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔊 AUDIO DEBUG STARTED");
        CheckAudioManager();
    }

    void Update()
    {
        // Test keys for debugging
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("🎵 Testing PlayerFootstep...");
            TestFootstep();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("🔍 Checking AudioManager status...");
            CheckAudioManager();
        }
    }

    void TestFootstep()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL! AudioManager not found.");
            return;
        }

        Debug.Log("✅ AudioManager found! Attempting to play footstep...");
        AudioManager.Instance.PlayPlayerFootstep();
        Debug.Log("🎵 PlayPlayerFootstep() called successfully");
    }

    void CheckAudioManager()
    {
        Debug.Log("=== AUDIO MANAGER STATUS ===");
        
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL");
            
            // Try to find AudioManager manually
            AudioManager manager = FindObjectOfType<AudioManager>();
            if (manager == null)
            {
                Debug.LogError("❌ No AudioManager script found in scene!");
                Debug.LogError("💡 Solution: Add AudioManager script to AudioManager GameObject");
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager script found but Instance is null");
                Debug.LogWarning("💡 Check AudioManager Awake() method");
            }
        }
        else
        {
            Debug.Log("✅ AudioManager.Instance found!");
            
            // Check if playerSounds array exists and has sounds
            var manager = AudioManager.Instance;
            if (manager.playerSounds == null)
            {
                Debug.LogError("❌ playerSounds array is null");
            }
            else if (manager.playerSounds.Length == 0)
            {
                Debug.LogError("❌ playerSounds array is empty");
            }
            else
            {
                Debug.Log($"✅ playerSounds array has {manager.playerSounds.Length} sounds");
                
                // Check PlayerFootstep specifically
                bool foundFootstep = false;
                for (int i = 0; i < manager.playerSounds.Length; i++)
                {
                    var sound = manager.playerSounds[i];
                    Debug.Log($"Player Sound {i}: Name='{sound.name}', Clip={sound.clip != null}, Volume={sound.volume}");
                    
                    if (sound.name == "PlayerFootstep")
                    {
                        foundFootstep = true;
                        if (sound.clip == null)
                        {
                            Debug.LogError("❌ PlayerFootstep sound found but Clip is NULL!");
                            Debug.LogError("💡 Drag your audio file to the Clip slot");
                        }
                        else
                        {
                            Debug.Log("✅ PlayerFootstep sound is properly configured!");
                        }
                    }
                }
                
                if (!foundFootstep)
                {
                    Debug.LogError("❌ No sound named 'PlayerFootstep' found!");
                    Debug.LogError("💡 Make sure Name field is exactly 'PlayerFootstep'");
                }
            }
        }
        
        Debug.Log("=== END STATUS ===");
    }
}
