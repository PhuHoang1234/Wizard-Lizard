using UnityEngine;

public class Level2QuickFix : MonoBehaviour
{
    void Start()
    {
        // Simple and fast fixes for Level 2
        Invoke(nameof(DoQuickFixes), 0.2f); // Wait a moment for everything to initialize
    }
    
    void DoQuickFixes()
    {
        Debug.Log("🔧 Level 2 Quick Fix starting...");
        
        // 1. Fix enemy patrol points
        FixEnemyPatrolPoints();
        
        // 2. Ensure player is working
        EnsurePlayerWorks();
        
        Debug.Log("✅ Level 2 Quick Fix completed!");
    }
    
    void FixEnemyPatrolPoints()
    {
        // Fix EnemyPatrol scripts
        EnemyPatrol[] enemyPatrols = FindObjectsOfType<EnemyPatrol>();
        foreach (var patrol in enemyPatrols)
        {
            if (patrol.patrolPoints == null || patrol.patrolPoints.Length == 0)
            {
                Debug.Log($"🔧 Creating patrol points for {patrol.gameObject.name}");
                
                // Create two simple patrol points
                GameObject wp1 = new GameObject(patrol.gameObject.name + "_Point1");
                GameObject wp2 = new GameObject(patrol.gameObject.name + "_Point2");
                
                // Position them near the enemy
                Vector3 pos = patrol.transform.position;
                wp1.transform.position = pos + Vector3.left * 2f;
                wp2.transform.position = pos + Vector3.right * 2f;
                
                // Assign to patrol script
                patrol.patrolPoints = new Transform[] { wp1.transform, wp2.transform };
            }
        }
        
        // Fix Enemy_Back_And_Forth_Movement scripts  
        Enemy_Back_And_Forth_Movement[] enemies = FindObjectsOfType<Enemy_Back_And_Forth_Movement>();
        foreach (var enemy in enemies)
        {
            if (enemy.points == null || enemy.points.Length == 0)
            {
                Debug.Log($"🔧 Creating waypoints for {enemy.gameObject.name}");
                
                // Create two simple waypoints
                GameObject wp1 = new GameObject(enemy.gameObject.name + "_WP1");
                GameObject wp2 = new GameObject(enemy.gameObject.name + "_WP2");
                
                // Position them near the enemy
                Vector3 pos = enemy.transform.position;
                wp1.transform.position = pos + Vector3.left * 2f;
                wp2.transform.position = pos + Vector3.right * 2f;
                
                // Assign to enemy script
                enemy.points = new Transform[] { wp1.transform, wp2.transform };
            }
        }
    }
    
    void EnsurePlayerWorks()
    {
        // Make sure player controller exists and is working
        PlayerController3D player = FindObjectOfType<PlayerController3D>();
        if (player == null)
        {
            Debug.LogWarning("⚠️ No PlayerController3D found in Level 2!");
        }
        else
        {
            Debug.Log("✅ Player found and should be working");
        }
    }
}
