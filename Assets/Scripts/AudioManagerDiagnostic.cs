using UnityEngine;

public class AudioManagerDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 AudioManager Diagnostic Starting...");
        
        // Check if AudioManager exists
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager.Instance is NULL! AudioManager may not be in the scene.");
            return;
        }
        
        Debug.Log("✅ AudioManager.Instance found!");
        
        // Check each sound array
        CheckSoundArray("Player Sounds", AudioManager.Instance.playerSounds);
        CheckSoundArray("Enemy Sounds", AudioManager.Instance.enemySounds);
        CheckSoundArray("Environment Sounds", AudioManager.Instance.environmentSounds);
        CheckSoundArray("UI Sounds", AudioManager.Instance.uiSounds);
    }
    
    void CheckSoundArray(string arrayName, SoundEffect[] sounds)
    {
        if (sounds == null)
        {
            Debug.LogWarning($"⚠️ {arrayName} array is NULL!");
            return;
        }
        
        if (sounds.Length == 0)
        {
            Debug.LogWarning($"⚠️ {arrayName} array is empty!");
            return;
        }
        
        Debug.Log($"📊 {arrayName}: {sounds.Length} sounds configured");
        
        for (int i = 0; i < sounds.Length; i++)
        {
            var sound = sounds[i];
            if (sound == null)
            {
                Debug.LogError($"❌ {arrayName}[{i}] is NULL!");
                continue;
            }
            
            if (sound.clip == null)
            {
                Debug.LogError($"❌ {arrayName}[{i}] '{sound.name}' has no AudioClip!");
                continue;
            }
            
            if (sound.source == null)
            {
                Debug.LogError($"❌ {arrayName}[{i}] '{sound.name}' has no AudioSource!");
                continue;
            }
            
            Debug.Log($"✅ {arrayName}[{i}] '{sound.name}' - OK");
        }
    }
    
    void Update()
    {
        // Quick test keys
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("🧪 Testing player footstep sound...");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerFootstep();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("🧪 Testing enemy alert sound...");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnemyAlert();
            }
        }
    }
}
