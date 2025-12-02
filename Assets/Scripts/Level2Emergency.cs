using UnityEngine;

public class Level2Emergency : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🚨 EMERGENCY LEVEL 2 FIX STARTING...");
        
        // Wait a moment then fix everything
        Invoke(nameof(EmergencyFix), 0.1f);
    }
    
    void EmergencyFix()
    {
        // 1. DISABLE ALL PROBLEMATIC ENEMY SCRIPTS
        DisableProblematicScripts();
        
        // 2. ENSURE BASIC GAME WORKS
        EnsureBasicGameplay();
        
        Debug.Log("✅ EMERGENCY FIX COMPLETE - Level 2 should work now!");
    }
    
    void DisableProblematicScripts()
    {
        // Disable all EnemyPatrol scripts that are causing errors
        EnemyPatrol[] enemyPatrols = FindObjectsOfType<EnemyPatrol>();
        foreach (var patrol in enemyPatrols)
        {
            if (patrol.patrolPoints == null || patrol.patrolPoints.Length == 0)
            {
                Debug.Log($"🔧 Disabling problematic EnemyPatrol on {patrol.gameObject.name}");
                patrol.enabled = false;
            }
        }
        
        // Disable problematic Enemy_Back_And_Forth_Movement scripts and add simple ones
        Enemy_Back_And_Forth_Movement[] enemies = FindObjectsOfType<Enemy_Back_And_Forth_Movement>();
        foreach (var enemy in enemies)
        {
            if (enemy.points == null || enemy.points.Length == 0)
            {
                Debug.Log($"🔧 Disabling problematic Enemy movement on {enemy.gameObject.name}");
                enemy.enabled = false;
                
                // Add simple movement instead
                if (enemy.gameObject.GetComponent<SimpleEnemyMovement>() == null)
                {
                    enemy.gameObject.AddComponent<SimpleEnemyMovement>();
                    Debug.Log($"✅ Added SimpleEnemyMovement to {enemy.gameObject.name}");
                }
            }
        }
        
        // Disable Level2Startup if it exists (might be causing conflicts)
        Level2Startup startup = FindObjectOfType<Level2Startup>();
        if (startup != null)
        {
            Debug.Log("🔧 Disabling Level2Startup to prevent conflicts");
            startup.enabled = false;
        }
        
        // Disable RuntimeProjectFixer if it exists
        RuntimeProjectFixer fixer = FindObjectOfType<RuntimeProjectFixer>();
        if (fixer != null)
        {
            Debug.Log("🔧 Disabling RuntimeProjectFixer to prevent conflicts");
            fixer.enabled = false;
        }
    }
    
    void EnsureBasicGameplay()
    {
        // Make sure AudioManager exists and works
        if (AudioManager.Instance == null)
        {
            AudioManager am = FindObjectOfType<AudioManager>();
            if (am == null)
            {
                Debug.Log("🔧 Creating basic AudioManager");
                GameObject amObj = new GameObject("AudioManager");
                amObj.AddComponent<AudioManager>();
            }
        }
        
        // Make sure player exists
        PlayerController3D player = FindObjectOfType<PlayerController3D>();
        if (player == null)
        {
            Debug.LogWarning("⚠️ No player found in Level 2!");
        }
        else
        {
            Debug.Log("✅ Player found - Level 2 should be playable now");
        }
        
        // Remove duplicate audio listeners (common issue)
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        for (int i = 1; i < listeners.Length; i++)
        {
            listeners[i].enabled = false;
        }
        
        // Remove duplicate event systems
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 1; i < eventSystems.Length; i++)
        {
            Destroy(eventSystems[i].gameObject);
        }
    }
}
