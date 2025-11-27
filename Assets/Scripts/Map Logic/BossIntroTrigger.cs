using System.Collections;
using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    public GameObject cinematicBars;             // the Cinematic Canvas
    public Camera gameplayCamera;                // Main Camera
    public Camera cinematicCamera;               // BossCinematic
    public Light range;
    public AudioSource roarSource;               // AudioSource on the boss
    public MonoBehaviour[] playerScriptsToDisable;  // movement, camera, etc.
    public MonoBehaviour bossAI;                 // boss behaviour script

    public float extraWaitAfterRoar = 0.5f;      // small pause after roar

    bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        if (!other.CompareTag("Player")) return;

        hasPlayed = true;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 1. Disable player control
        foreach (var s in playerScriptsToDisable)
        {
            if (s != null) s.enabled = false;
        }

        // 2. Switch to cinematic camera
        if (gameplayCamera != null) gameplayCamera.enabled = false;
        if (cinematicCamera != null) cinematicCamera.enabled = true;

        // 3. Show cinematic bars
        if (cinematicBars != null) cinematicBars.SetActive(true);

        // 4. Play roar
        float waitTime = 1f;
        if (roarSource != null && roarSource.clip != null)
        {
            roarSource.Play();
            waitTime = roarSource.clip.length;
        }

        yield return new WaitForSeconds(waitTime + extraWaitAfterRoar);

        // 5. Hide bars and switch back
        if (cinematicBars != null) cinematicBars.SetActive(false);

        if (cinematicCamera != null) cinematicCamera.enabled = false;
        if (gameplayCamera != null) gameplayCamera.enabled = true;

        // 6. Re-enable player
        foreach (var s in playerScriptsToDisable)
        {
            if (s != null) s.enabled = true;
        }

        // 7. Start boss AI
        if (bossAI != null) bossAI.enabled = true;
        if (range != null) range.enabled = true;
        {
            
        }

        // 8. Trigger only once
        Destroy(gameObject);
    }
}
