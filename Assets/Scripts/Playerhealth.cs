using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("On Death")]
    public Transform respawnPoint;  // Drag safe spawn
    public float respawnDelay = 2f;

    public bool IsAlive() => currentHealth > 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Debug.Log($"Player damaged! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died! Respawning...");
        
        // Stop all audio when player dies
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopFootstep();
            AudioManager.Instance.StopBackgroundMusic();
            Debug.Log("🔇 Stopped all audio - player died");
        }
        
        // Game Over UI or fade
        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        if (respawnPoint) transform.position = respawnPoint.position;
        // Revive effects
    }
}