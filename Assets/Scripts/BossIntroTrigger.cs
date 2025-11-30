using System.Collections;
using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    [Header("UI & Cameras")]
    public GameObject cinematicBars;     // black bars canvas
    public Camera gameplayCamera;        // main camera
    public Camera cinematicCamera;       // boss intro cam

    [Header("Boss & Audio")]
    public Light range;                  // optional boss range light
    public AudioSource roarSource;       // AudioSource with roar clip
    public MonoBehaviour bossAI;         // BossWyvernSimpleAI

    [Header("Player Freeze")]
    public Rigidbody playerRb;           // Wizard Rigidbody
    public Animator playerAnim;          // Wizard Animator
    public MonoBehaviour[] playerScriptsToDisable; // PlayerController3D etc.

    public float extraWaitAfterRoar = 0.5f;

    bool hasPlayed = false;
    bool storedKinematic;
    RigidbodyConstraints storedConstraints;
    bool storedRootMotion;


    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        if (!other.CompareTag("Player")) return;

        hasPlayed = true;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 0) HARD STOP player movement + store RB state
        if (playerRb != null)
        {
            storedKinematic = playerRb.isKinematic;
            storedConstraints = playerRb.constraints;

            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;                         // no physics movement
            playerRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (playerAnim != null)
        {
            // remember original setting
            storedRootMotion = playerAnim.applyRootMotion;

            // stop any locomotion animation / root motion
            playerAnim.applyRootMotion = false;
            playerAnim.SetFloat("Speed", 0f);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("Walk", false);
        }


        // 1) Disable movement/input scripts
        foreach (var s in playerScriptsToDisable)
        {
            if (s != null) s.enabled = false;
        }

        // 2) Switch cameras + show bars
        if (gameplayCamera) gameplayCamera.enabled = false;
        if (cinematicCamera) cinematicCamera.enabled = true;
        if (cinematicBars) cinematicBars.SetActive(true);

        // 3) Play roar
        float waitTime = 1f;
        if (roarSource != null && roarSource.clip != null)
        {
            roarSource.Play();
            waitTime = roarSource.clip.length;
        }

        yield return new WaitForSeconds(waitTime + extraWaitAfterRoar);

        // 4) End cutscene: hide bars, restore main camera
        if (cinematicBars) cinematicBars.SetActive(false);
        if (cinematicCamera) cinematicCamera.enabled = false;
        if (gameplayCamera) gameplayCamera.enabled = true;

        // 5) Re-enable movement scripts
        foreach (var s in playerScriptsToDisable)
        {
            if (s != null) s.enabled = true;
        }

        // 6) Restore Rigidbody state
        if (playerRb != null)
        {
            playerRb.isKinematic = storedKinematic;
            playerRb.constraints = storedConstraints;
        }

        if (playerAnim != null)
        {
            // put it back exactly how it was before the cutscene
            playerAnim.applyRootMotion = storedRootMotion;
        }


        // 7) Start boss AI + range light
        if (bossAI != null) bossAI.enabled = true;
        if (range != null) range.enabled = true;

        Destroy(gameObject);
    }
}
