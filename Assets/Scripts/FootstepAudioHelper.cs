using UnityEngine;

public class FootstepAudioHelper : MonoBehaviour
{
    [Header("Drag Footstep Audio Files Here")]
    public AudioClip[] footstepClips;
    
    [Header("Settings")]
    public bool randomizeFootsteps = true;
    
    void Start()
    {
        AssignFootstepsToAudioManager();
    }
    
    public void AssignFootstepsToAudioManager()
    {
        if (AudioManager.Instance != null && footstepClips.Length > 0)
        {
            // Use the first footstep clip as the main one
            AudioManager.Instance.footstepSound = footstepClips[0];
            Debug.Log($"Assigned footstep sound: {footstepClips[0].name}");
            
            if (footstepClips.Length > 1)
            {
                Debug.Log($"Found {footstepClips.Length} footstep sounds. Using first one for now.");
                Debug.Log("Tip: You can enhance the FootstepAudioHelper to randomly play different sounds!");
            }
        }
        else if (footstepClips.Length == 0)
        {
            Debug.LogWarning("No footstep clips assigned to FootstepAudioHelper!");
        }
    }
    
    // Enhanced method for random footstep sounds
    public AudioClip GetRandomFootstep()
    {
        if (footstepClips.Length == 0) return null;
        
        if (randomizeFootsteps && footstepClips.Length > 1)
        {
            int randomIndex = Random.Range(0, footstepClips.Length);
            return footstepClips[randomIndex];
        }
        
        return footstepClips[0];
    }
}
