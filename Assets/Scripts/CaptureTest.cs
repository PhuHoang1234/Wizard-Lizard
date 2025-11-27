using UnityEngine;

public class CaptureTest : MonoBehaviour
{
    void Update()
    {
        // Press 'C' to test capture sound and sequence
        if (Input.GetKeyDown(KeyCode.C))
        {
            TestCaptureSequence();
        }
        
        // Press 'X' to stop all sounds
        if (Input.GetKeyDown(KeyCode.X))
        {
            StopAllAudio();
        }
    }
    
    void TestCaptureSequence()
    {
        Debug.Log("🧪 Testing Player Capture Sequence...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerCaptured();
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void StopAllAudio()
    {
        Debug.Log("🔇 Stopping all audio...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
            Debug.Log("✅ All sounds stopped");
        }
        else
        {
            Debug.LogError("❌ AudioManager.Instance is null!");
        }
    }
    
    void Start()
    {
        Debug.Log("=== CAPTURE TEST CONTROLS ===");
        Debug.Log("C = Test Player Capture Sound");
        Debug.Log("X = Stop All Sounds");
        Debug.Log("8 = Test Player Capture (AudioTester)");
    }
}
