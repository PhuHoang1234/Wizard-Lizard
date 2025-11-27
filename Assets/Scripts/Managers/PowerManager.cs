using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    // ==============================
    // Config Settings
    // ==============================

    [Header("Illusion Skill Settings")]
    public float illusionDuration = 10.0f;        // State duration
    public float illusionCooldown = 15.0f;        // Total cooldown including duration

    [Header("Distraction Skill Settings")]
    public float distractionDuration = 20f;
    public float distractionCooldown = 30f;

    [Header("Lightning Skill Settings")]
    public float lightningRange = 20.0f;         // Detect & Raycast range
    public float lightningCooldown = 3.0f;
    public float aimAssistAngle = 30.0f;         // Auto-aim cone angle
    public LineRenderer lightningLine;
    public LayerMask enemyLayer;                 // (Not used currently but reserved)
    public float damage = 20.0f;

    // ==============================
    // Runtime Variables
    // ==============================

    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>(); // Cooldowns & durations
    public VoiceManager voiceManager;
    public GameData gameData;
    private bool isHidding = false;              // Hidden state flag


    // ==============================
    // Common Skill Helpers
    // ==============================

    // Return true if skill is not in cooldown or cooldown <= 0
    public bool isReady(string skillName)
    {
        return !cooldownTimers.ContainsKey(skillName) || cooldownTimers[skillName] <= 0.0f;
    }

    // Check whether any skill is currently active (duration not finished)
    public bool isSkilling()
    {
        return cooldownTimers.Count > 0;
    }

    // Start cooldown or duration timer
    private void StartCooldown(string skillName, float cdTime)
    {
        cooldownTimers[skillName] = cdTime;
    }

    // Run all cooldown timers, remove keys when finished
    public void CooldownTimersRun()
    {
        List<string> keys = new List<string>(cooldownTimers.Keys);
        foreach (var key in keys)
        {
            cooldownTimers[key] -= Time.deltaTime;

            if (cooldownTimers[key] <= 0)
            {
                cooldownTimers.Remove(key);
            }
        }
    }


    // ==============================
    // Illusion Skill
    // ==============================

    public bool IllusionStart()
    {
        if (!gameData.illusionUnlock) return false;

        isHidding = true;
        StartCooldown("illusion", illusionCooldown);
        StartCooldown("illusionDuration", illusionDuration);
        return true;
    }

    public void IllusionEnd()
    {
        isHidding = false;
    }

    public bool isPlayerHidding()
    {
        return isHidding;
    }


    // ==============================
    // Distraction Skill
    // ==============================

    // Remove distraction cooldown timers immediately
    public void RefreshDistractionCooldown()
    {
        cooldownTimers.Remove("distractionDuration");
        cooldownTimers.Remove("distraction");
    }

    public void DistracitonStart()
    {
        StartCooldown("distraction", distractionCooldown);
        StartCooldown("distractionDuration", distractionDuration);
    }

    // Try distract enemies, returns false if duration over (cancel)
    public bool Distraction(Vector3 voicePosition)
    {
        if (isReady("distractionDuration"))
        {
            RefreshDistractionCooldown();
            return false;
        }

        voiceManager.MakeVoice(voicePosition);
        return true;
    }


    // ==============================
    // Lightning Skill
    // ==============================

    public bool LightningStart(Transform player)
    {
        if (!gameData.castUnlock) return false;

        StartCooldown("lightning", lightningCooldown);
        CastLightning(player);
        return true;
    }

    // Target select ¡ú Raycast ¡ú Damage ¡ú Visual
    void CastLightning(Transform player)
    {
        Vector3 direction = player.forward;
        Collider[] enemies = Physics.OverlapSphere(player.position, lightningRange);
        Transform bestTarget = null;
        float bestAngle = aimAssistAngle;

        foreach (var e in enemies)
        {
            if (!e.CompareTag("Enemy")) continue;

            Vector3 dirToEnemy = (e.transform.position - player.position).normalized;
            float angle = Vector3.Angle(player.forward, dirToEnemy);

            // Aim-assist: find closest target within angle
            if (angle < bestAngle)
            {
                RaycastHit hit;
                if (Physics.Raycast(player.position, dirToEnemy, out hit, lightningRange))
                {
                    if (hit.collider.transform == e.transform)
                    {
                        bestAngle = angle;
                        bestTarget = e.transform;
                    }
                }
            }
        }

        // If valid target found ¡ú lock aim
        if (bestTarget != null)
        {
            direction = (bestTarget.position - player.position).normalized;
        }

        RaycastHit finalHit;
        if (Physics.Raycast(player.position, direction, out finalHit, lightningRange))
        {
            DrawLightning(player.position, finalHit.point);

            EnemyBase enemy = finalHit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else
        {
            DrawLightning(player.position, player.position + direction * lightningRange);
        }
    }

    // ==============================
    // Lightning Visual Effects
    // ==============================

    void DrawLightning(Vector3 playerPos, Vector3 endPos)
    {
        if (!lightningLine) return;

        lightningLine.enabled = true;
        lightningLine.SetPosition(0, playerPos);
        lightningLine.SetPosition(1, endPos);

        CancelInvoke(nameof(HideLightning));
        Invoke(nameof(HideLightning), 0.08f); // auto-hide line effect
    }

    void HideLightning()
    {
        lightningLine.enabled = false;
    }
}
