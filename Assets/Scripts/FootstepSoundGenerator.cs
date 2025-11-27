using UnityEngine;

public class FootstepSoundGenerator : MonoBehaviour
{
    [Header("Sound Generation Settings")]
    public int sampleRate = 22050;
    public float footstepDuration = 0.1f;
    public float pickupDuration = 0.3f;
    public float doorDuration = 0.8f;
    
    void Start()
    {
        // Generate all sound effects on start
        GenerateAllSounds();
    }
    
    public void GenerateAllSounds()
    {
        // Create sound effects
        AudioClip footstepClip = CreateFootstepClip();
        AudioClip pickupClip = CreatePickupClip();
        AudioClip doorClip = CreateDoorClip();
        
        // Assign to AudioManager if it exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.footstepSound = footstepClip;
            AudioManager.Instance.pickupSound = pickupClip;
            AudioManager.Instance.doorSound = doorClip;
            
            Debug.Log("Generated and assigned all sound effects!");
        }
    }
    
    AudioClip CreateFootstepClip()
    {
        int sampleLength = Mathf.FloorToInt(sampleRate * footstepDuration);
        AudioClip clip = AudioClip.Create("Generated_Footstep", sampleLength, 1, sampleRate, false);
        
        float[] samples = new float[sampleLength];
        
        // Generate a simple footstep sound (noise burst with envelope)
        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / sampleRate;
            float envelope = Mathf.Exp(-time * 15f); // Quick decay envelope
            
            // Mix of noise and low frequency thump
            float noise = (Random.Range(-1f, 1f) * 0.4f);
            float thump = Mathf.Sin(2 * Mathf.PI * 100f * time) * 0.6f; // 100Hz thump
            
            samples[i] = (noise + thump) * envelope * 0.4f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    AudioClip CreatePickupClip()
    {
        int sampleLength = Mathf.FloorToInt(sampleRate * pickupDuration);
        AudioClip clip = AudioClip.Create("Generated_Pickup", sampleLength, 1, sampleRate, false);
        
        float[] samples = new float[sampleLength];
        
        // Generate a pickup sound (ascending chime)
        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / sampleRate;
            float progress = time / pickupDuration;
            float envelope = Mathf.Sin(progress * Mathf.PI); // Bell curve envelope
            
            // Ascending frequencies for a magical pickup sound
            float freq1 = 440f + progress * 220f; // 440Hz to 660Hz
            float freq2 = 880f + progress * 440f; // 880Hz to 1320Hz
            
            float wave1 = Mathf.Sin(2 * Mathf.PI * freq1 * time) * 0.3f;
            float wave2 = Mathf.Sin(2 * Mathf.PI * freq2 * time) * 0.2f;
            
            samples[i] = (wave1 + wave2) * envelope * 0.5f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    AudioClip CreateDoorClip()
    {
        int sampleLength = Mathf.FloorToInt(sampleRate * doorDuration);
        AudioClip clip = AudioClip.Create("Generated_Door", sampleLength, 1, sampleRate, false);
        
        float[] samples = new float[sampleLength];
        
        // Generate a door opening sound (creaking with low frequency rumble)
        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / sampleRate;
            float progress = time / doorDuration;
            float envelope = 1f - (progress * 0.7f); // Gradual decay
            
            // Creaking sound (irregular frequency)
            float creak = Mathf.Sin(2 * Mathf.PI * (200f + Mathf.Sin(time * 10f) * 50f) * time);
            
            // Low rumble
            float rumble = Mathf.Sin(2 * Mathf.PI * 60f * time) * 0.5f;
            
            // Add some noise for texture
            float noise = (Random.Range(-1f, 1f) * 0.2f);
            
            samples[i] = (creak * 0.4f + rumble + noise) * envelope * 0.3f;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
}
