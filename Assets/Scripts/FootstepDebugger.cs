using UnityEngine;

public class FootstepDebugger : MonoBehaviour
{
    void Start()
    {
        // Check footstep setup after a delay
        Invoke(nameof(DebugFootstepSetup), 1f);
    }
    
    void Update()
    {
        // Press F key to test footsteps manually
        if (Input.GetKeyDown(KeyCode.F))
        {
            TestFootsteps();
        }
    }
    
    void DebugFootstepSetup()
    {
        Debug.Log("🔍 === FOOTSTEP DEBUG REPORT ===");
        
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ No AudioManager found!");
            return;
        }
        
        Debug.Log($"✅ AudioManager found: {AudioManager.Instance.name}");
        Debug.Log($"🦶 Footsteps enabled: {AudioManager.Instance.footstepsEnabled}");
        Debug.Log($"🎵 Footstep sound assigned: {(AudioManager.Instance.footstepSound != null ? AudioManager.Instance.footstepSound.name : "NULL")}");
        Debug.Log($"🔊 Footstep audio source: {(AudioManager.Instance.footstepSource != null ? "Found" : "NULL")}");
        
        if (AudioManager.Instance.footstepSource != null)
        {
            Debug.Log($"📢 Footstep source volume: {AudioManager.Instance.footstepSource.volume}");
            Debug.Log($"🔇 Footstep source muted: {AudioManager.Instance.footstepSource.mute}");
        }
        
        Debug.Log("🔍 === END DEBUG REPORT ===");
    }
    
    void TestFootsteps()
    {
        Debug.Log("🧪 Testing footsteps manually...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.EnableFootsteps();
            AudioManager.Instance.PlayFootstep();
            Debug.Log("🦶 Manual footstep test executed");
        }
    }
}
