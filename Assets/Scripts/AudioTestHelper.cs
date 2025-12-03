using UnityEngine;

public class AudioTestHelper : MonoBehaviour
{
    void Update()
    {
        // Press M key to test game music
        if (Input.GetKeyDown(KeyCode.M))
        {
            TestGameMusic();
        }
        
        // Press R key to force restart all audio
        if (Input.GetKeyDown(KeyCode.R))
        {
            ForceRestartAudio();
        }
    }
    
    void TestGameMusic()
    {
        Debug.Log("🎵 Testing game music...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameMusic();
            Debug.Log("🎵 Game music test executed");
        }
        else
        {
            Debug.LogWarning("❌ No AudioManager found!");
        }
    }
    
    void ForceRestartAudio()
    {
        Debug.Log("🔄 Force restarting audio...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ForceRestartGameAudio();
            Debug.Log("✅ Audio force restart completed");
        }
        else
        {
            Debug.LogWarning("❌ No AudioManager found!");
        }
    }
}
