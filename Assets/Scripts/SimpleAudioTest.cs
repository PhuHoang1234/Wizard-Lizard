using UnityEngine;

public class SimpleAudioTest : MonoBehaviour
{
    public AudioClip testClip;
    
    void Start()
    {
        // Wait 2 seconds then play test sound
        Invoke("PlayTestSound", 2f);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayTestSound();
        }
    }
    
    void PlayTestSound()
    {
        if (testClip != null)
        {
            // Play sound directly without AudioManager
            AudioSource.PlayClipAtPoint(testClip, Camera.main.transform.position, 1.0f);
            Debug.Log("🎵 Playing test sound directly!");
        }
        else
        {
            Debug.LogError("❌ Test clip not assigned!");
        }
    }
}
