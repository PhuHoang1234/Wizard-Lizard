using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    [Header("Main Menu Audio")]
    public bool autoPlayOnStart = true;
    public AudioClip mainMenuMusicClip;
    
    void Start()
    {
        if (autoPlayOnStart)
        {
            SetupMainMenuAudio();
        }
    }
    
    public void SetupMainMenuAudio()
    {
        if (AudioManager.Instance != null)
        {
            // Assign main menu music if provided
            if (mainMenuMusicClip != null)
            {
                AudioManager.Instance.mainMenuMusic = mainMenuMusicClip;
            }
            
            // Switch to main menu music
            AudioManager.Instance.SwitchToMainMenuMusic();
            
            // Disable footsteps completely (in case player came from a game level)
            AudioManager.Instance.DisableFootsteps();
            Debug.Log("🏠 Footsteps disabled for main menu");
            
            Debug.Log("🎵 Main menu audio setup complete");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioManager not found! Main menu music won't play.");
        }
    }
    
    public void OnPlayGameClicked()
    {
        // This method can be called by the Play button
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SwitchToGameMusic();
            Debug.Log("🎮 Switching to game music");
        }
    }
}
