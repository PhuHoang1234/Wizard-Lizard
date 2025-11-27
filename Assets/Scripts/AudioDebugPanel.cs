using UnityEngine;

public class AudioDebugPanel : MonoBehaviour
{
    [Header("Debug Controls")]
    public KeyCode testFootstepKey = KeyCode.F;
    public KeyCode testPickupKey = KeyCode.P;
    public KeyCode testDoorKey = KeyCode.O;
    public KeyCode toggleMusicKey = KeyCode.M;
    
    [Header("Volume Controls")]
    [Range(0f, 1f)] public float debugMusicVolume = 0.7f;
    [Range(0f, 1f)] public float debugSFXVolume = 1f;
    
    private float lastMusicVolume;
    private float lastSFXVolume;
    
    void Start()
    {
        lastMusicVolume = debugMusicVolume;
        lastSFXVolume = debugSFXVolume;
    }
    
    void Update()
    {
        // Test sound effects with keyboard
        if (Input.GetKeyDown(testFootstepKey))
        {
            TestFootstep();
        }
        
        if (Input.GetKeyDown(testPickupKey))
        {
            TestPickup();
        }
        
        if (Input.GetKeyDown(testDoorKey))
        {
            TestDoor();
        }
        
        if (Input.GetKeyDown(toggleMusicKey))
        {
            ToggleMusic();
        }
        
        // Check for volume changes
        if (Mathf.Abs(debugMusicVolume - lastMusicVolume) > 0.01f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(debugMusicVolume);
            }
            lastMusicVolume = debugMusicVolume;
        }
        
        if (Mathf.Abs(debugSFXVolume - lastSFXVolume) > 0.01f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(debugSFXVolume);
            }
            lastSFXVolume = debugSFXVolume;
        }
    }
    
    public void TestFootstep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstep();
            Debug.Log("Testing footstep sound!");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
    
    public void TestPickup()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupSound);
            Debug.Log("Testing pickup sound!");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
    
    public void TestDoor()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.doorSound);
            Debug.Log("Testing door sound!");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
    
    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusic();
            Debug.Log("Toggled background music!");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }
    
    void OnGUI()
    {
        if (AudioManager.Instance == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("🎵 Audio Debug Panel");
        
        GUILayout.Label($"Press {testFootstepKey} - Test Footstep");
        GUILayout.Label($"Press {testPickupKey} - Test Pickup");
        GUILayout.Label($"Press {testDoorKey} - Test Door");
        GUILayout.Label($"Press {toggleMusicKey} - Toggle Music");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🦶 Test Footstep"))
        {
            TestFootstep();
        }
        
        if (GUILayout.Button("💰 Test Pickup"))
        {
            TestPickup();
        }
        
        if (GUILayout.Button("🚪 Test Door"))
        {
            TestDoor();
        }
        
        if (GUILayout.Button("🎵 Toggle Music"))
        {
            ToggleMusic();
        }
        
        GUILayout.EndArea();
    }
}
