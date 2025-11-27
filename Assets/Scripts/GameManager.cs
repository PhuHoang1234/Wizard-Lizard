using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject tail;
    public GameObject cameraFocus;
    public PowerManager powerManager;
    public VoiceManager voiceManager;

    public bool isGamePaused = false;

    private void Start()
    {
        cameraFocus = player;
    }

    private void Update()
    {
        if (!isGamePaused)
        {
            powerManager.CooldownTimersRun();
        }
    }

    public void CameraFocusChange(GameObject target)
    {
        if (target == null) cameraFocus = player; else cameraFocus = target;
    }

    public GameObject CameraFocus()
    {
        if (cameraFocus == null) cameraFocus = player;
        return cameraFocus;
    }
}
