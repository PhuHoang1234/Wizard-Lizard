using UnityEngine;

public class AudioManagerFixer : MonoBehaviour
{
    void Start()
    {
        FixAudioManager();
    }
    
    void FixAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("❌ AudioManager not found!");
            return;
        }
        
        Debug.Log("🔧 Checking AudioManager setup...");
        
        // Fix footstep sound
        if (AudioManager.Instance.footstepSound == null)
        {
            // Try to load the footstep sound from Resources
            AudioClip footstepClip = Resources.Load<AudioClip>("Audio/SFX/Footsteps/concrete-footsteps-6752");
            
            if (footstepClip == null)
            {
                // Try alternative path
                footstepClip = Resources.Load<AudioClip>("concrete-footsteps-6752");
            }
            
            if (footstepClip != null)
            {
                AudioManager.Instance.footstepSound = footstepClip;
                Debug.Log("✅ Fixed footstep sound assignment!");
            }
            else
            {
                Debug.LogError("❌ Could not find footstep audio file!");
                Debug.Log("Please manually assign 'concrete-footsteps-6752.mp3' to AudioManager in the inspector");
            }
        }
        else
        {
            Debug.Log("✅ Footstep sound already assigned");
        }
        
        // Fix background music
        if (AudioManager.Instance.backgroundMusic == null)
        {
            AudioClip bgMusic = Resources.Load<AudioClip>("Audio/Music/videoplayback");
            if (bgMusic != null)
            {
                AudioManager.Instance.backgroundMusic = bgMusic;
                Debug.Log("✅ Fixed background music assignment!");
            }
        }
        
        // Test footstep functionality
        TestFootstepFunctionality();
    }
    
    void TestFootstepFunctionality()
    {
        if (AudioManager.Instance.footstepSound != null && AudioManager.Instance.footstepSource != null)
        {
            Debug.Log("🦶 Testing footstep sound in 2 seconds...");
            Invoke(nameof(PlayTestFootstep), 2f);
        }
    }
    
    void PlayTestFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstep();
            Debug.Log("🦶 Footstep test sound played!");
            
            // Stop it after 1 second
            Invoke(nameof(StopTestFootstep), 1f);
        }
    }
    
    void StopTestFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
            Debug.Log("🔇 Footstep test sound stopped");
        }
    }
}
