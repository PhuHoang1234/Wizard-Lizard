using UnityEngine;

public class EnemySoundDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== ENEMY SOUND DIAGNOSTIC ===");
        Debug.Log("E = Test Enemy Alert");
        Debug.Log("R = Test Enemy Chase");  
        Debug.Log("Q = Test Enemy Patrol");
        Debug.Log("C = Test Player Captured");
        Debug.Log("F = Check Enemy Sound Setup");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestEnemyAlert();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestEnemyChase();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestEnemyPatrol();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            TestPlayerCaptured();
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckEnemySoundSetup();
        }
    }
    
    void TestEnemyAlert()
    {
        Debug.Log("🧪 Testing Enemy Alert Sound...");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyAlert();
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void TestEnemyChase()
    {
        Debug.Log("🧪 Testing Enemy Chase Sound...");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyChase();
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void TestEnemyPatrol()
    {
        Debug.Log("🧪 Testing Enemy Patrol Sound...");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyPatrol();
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void TestPlayerCaptured()
    {
        Debug.Log("🧪 Testing Player Captured Sound...");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerCaptured();
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void CheckEnemySoundSetup()
    {
        Debug.Log("🔍 === CHECKING ENEMY SOUND SETUP ===");
        
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
            return;
        }
        
        // Check Enemy Sounds Array
        var enemySounds = AudioManager.Instance.enemySounds;
        if (enemySounds == null)
        {
            Debug.LogError("❌ enemySounds array is null!");
            return;
        }
        
        Debug.Log($"✅ enemySounds array has {enemySounds.Length} elements");
        
        // Check each required enemy sound
        string[] requiredSounds = { "EnemyAlert", "EnemyChase", "EnemyPatrol", "PlayerCaptured" };
        
        foreach (string soundName in requiredSounds)
        {
            CheckSpecificEnemySound(enemySounds, soundName);
        }
        
        Debug.Log("🎯 === ENEMY SOUND CHECK COMPLETE ===");
    }
    
    void CheckSpecificEnemySound(SoundEffect[] enemySounds, string soundName)
    {
        bool found = false;
        
        for (int i = 0; i < enemySounds.Length; i++)
        {
            var sound = enemySounds[i];
            if (sound == null)
            {
                continue;
            }
            
            if (sound.name == soundName)
            {
                found = true;
                Debug.Log($"✅ Found '{soundName}' at index {i}");
                
                if (sound.clip == null)
                {
                    Debug.LogError($"❌ PROBLEM: '{soundName}' has no AudioClip assigned!");
                    Debug.LogError($"💡 FIX: In AudioManager > Enemy Sounds[{i}] > drag an audio file to 'Clip' field");
                }
                else
                {
                    Debug.Log($"✅ '{soundName}' has clip: {sound.clip.name}");
                    
                    if (sound.source == null)
                    {
                        Debug.LogError($"❌ PROBLEM: '{soundName}' AudioSource is null!");
                        Debug.LogError($"💡 FIX: AudioManager initialization failed - check console for errors");
                    }
                    else
                    {
                        Debug.Log($"✅ '{soundName}' AudioSource ready, volume: {sound.source.volume}");
                    }
                }
                break;
            }
        }
        
        if (!found)
        {
            Debug.LogError($"❌ MISSING: '{soundName}' not found in enemySounds array!");
            Debug.LogError($"💡 FIX: Add '{soundName}' to AudioManager > Enemy Sounds array");
        }
    }
}
