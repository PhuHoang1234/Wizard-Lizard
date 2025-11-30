using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeath : MonoBehaviour
{
    [Header("Death Options")]
    public float destroyDelay = 3f;  // Time corpse stays (0 = instant destroy)
    public bool enableRagdoll = false;  // Pro: Physics flop after anim
    public GameObject panel;
    private Animator anim;
    private Collider mainCollider;
    private Rigidbody rb;
    private MonoBehaviour[] movementScripts;  // Patrol, player controller
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        movementScripts = GetComponents<MonoBehaviour>();  // Disable all behaviors
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop footstep sounds when character dies
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
            Debug.Log("🔇 Stopped footsteps - character died");
        }

        // Play death + freeze
        anim.SetTrigger("Die");
        if (mainCollider) mainCollider.enabled = false;
        if (rb) rb.isKinematic = true;

        foreach (MonoBehaviour script in movementScripts)
        {
            if (script != this && script != anim) script.enabled = false;
        }

        // Cleanup (same as before)
        if (enableRagdoll)
        {
            Invoke(nameof(EnableRagdoll),
                anim.GetCurrentAnimatorStateInfo(0).length - 0.2f);
        }
        else if (destroyDelay > 0)
        {
            Invoke(nameof(DestroyCorpse), destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }

        // Wait 2 seconds, THEN show panel + pause
        Invoke(nameof(ShowDeathPanel), 2.5f);
    }

    void ShowDeathPanel()
    {
        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;
    }


    void DestroyCorpse()
    {
        Destroy(gameObject);
    }

    void EnableRagdoll()
    {
        anim.enabled = false;
        if (rb) rb.isKinematic = false;
        // Assumes ragdoll colliders/rigidbodies on child bones
    }

    // TRIGGER DEATH ON HIT (one-tap!)
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Die();
        }
    }

}
