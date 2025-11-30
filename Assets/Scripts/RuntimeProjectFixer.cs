using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RuntimeProjectFixer : MonoBehaviour
{
    [Header("Auto Fix Options")]
    public bool autoFixOnStart = true;
    public bool showDebugMessages = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            StartCoroutine(FixProjectIssues());
        }
    }
    
    [ContextMenu("Fix All Project Issues")]
    public void FixAllIssues()
    {
        StartCoroutine(FixProjectIssues());
    }
    
    IEnumerator FixProjectIssues()
    {
        yield return new WaitForSeconds(0.1f); // Let everything initialize
        
        FixEnemyWaypoints();
        yield return new WaitForSeconds(0.1f);
        
        FixCameraTargets();
        yield return new WaitForSeconds(0.1f);
        
        FixEventSystems();
        
        if (showDebugMessages)
        {
            Debug.Log("🎮 Wizard Lizard Project fixes completed!");
        }
    }
    
    void FixEnemyWaypoints()
    {
        var enemies = FindObjectsOfType<Enemy_Back_And_Forth_Movement>();
        int fixedCount = 0;
        
        foreach (var enemy in enemies)
        {
            if (enemy.points == null || enemy.points.Length == 0 || HasNullPoints(enemy.points))
            {
                CreateWaypointsForEnemy(enemy);
                fixedCount++;
            }
        }
        
        if (showDebugMessages && fixedCount > 0)
        {
            Debug.Log($"✅ Fixed {fixedCount} enemy waypoint issues!");
        }
    }
    
    bool HasNullPoints(Transform[] points)
    {
        if (points == null) return true;
        foreach (var point in points)
        {
            if (point == null) return true;
        }
        return false;
    }
    
    void CreateWaypointsForEnemy(Enemy_Back_And_Forth_Movement enemy)
    {
        // Create two waypoints for patrol
        GameObject waypoint1 = new GameObject($"{enemy.gameObject.name}_Waypoint1");
        GameObject waypoint2 = new GameObject($"{enemy.gameObject.name}_Waypoint2");
        
        // Position them relative to enemy's current position and forward direction
        Vector3 enemyPos = enemy.transform.position;
        Vector3 forward = enemy.transform.forward;
        Vector3 right = enemy.transform.right;
        
        // Create waypoints along the enemy's right axis for side-to-side movement
        waypoint1.transform.position = enemyPos + right * -3f;
        waypoint2.transform.position = enemyPos + right * 3f;
        
        // Assign to enemy
        enemy.points = new Transform[] { waypoint1.transform, waypoint2.transform };
        
        if (showDebugMessages)
        {
            Debug.Log($"🎯 Created waypoints for {enemy.gameObject.name}");
        }
    }
    
    void FixCameraTargets()
    {
        var cameras = FindObjectsOfType<TopDownCamera3D>();
        var players = FindObjectsOfType<PlayerController3D>();
        
        if (players.Length == 0)
        {
            if (showDebugMessages)
                Debug.LogWarning("⚠️ No PlayerController3D found to assign to cameras!");
            return;
        }
        
        var player = players[0]; // Use first player found
        int fixedCount = 0;
        
        foreach (var cam in cameras)
        {
            if (cam.target == null)
            {
                cam.target = player.transform;
                cam.targetRb = player.GetComponent<Rigidbody>();
                fixedCount++;
            }
        }
        
        if (showDebugMessages && fixedCount > 0)
        {
            Debug.Log($"🎥 Fixed {fixedCount} camera targets!");
        }
    }
    
    void FixEventSystems()
    {
        var eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        
        if (eventSystems.Length > 1)
        {
            // Disable all but the first one
            for (int i = 1; i < eventSystems.Length; i++)
            {
                eventSystems[i].gameObject.SetActive(false);
            }
            
            if (showDebugMessages)
            {
                Debug.Log($"🎛️ Disabled {eventSystems.Length - 1} duplicate Event Systems!");
            }
        }
    }
}
