using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject tail;
    public GameObject cameraFocus;
    public PowerManager powerManager;
    public VoiceManager voiceManager;
    public GameObject panel;

    public bool isGamePaused = false;

    private void Start()
    {
        cameraFocus = player;
        panel.SetActive(true);
        isGamePaused = true;
    }

    private void Update()
    {
        if (!isGamePaused)
        {
            powerManager.CooldownTimersRun();
        }
        else
        {
            if(Input.GetKey(KeyCode.Space))
            {
                panel.SetActive(false);
                isGamePaused = false;
            }
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
