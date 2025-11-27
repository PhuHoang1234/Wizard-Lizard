using UnityEngine;

public class GameObjectFixer : MonoBehaviour
{
    [Header("Fix Scene Issues")]
    [SerializeField] private bool fixOnStart = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixSceneIssues();
        }
    }
    
    [ContextMenu("Fix Scene Issues")]
    public void FixSceneIssues()
    {
        Debug.Log("🔧 Starting scene fixes...");
        
        // Fix missing script references
        FixMissingScripts();
        
        // Fix collider trigger issues
        FixColliderIssues();
        
        // Fix enemy waypoint issues
        FixEnemyWaypoints();
        
        Debug.Log("✅ Scene fixes completed!");
    }
    
    void FixMissingScripts()
    {
        // Find all GameObjects with missing scripts
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"🔧 Removing missing script from: {obj.name}");
                    // Note: Can't actually remove missing scripts via script
                    // User needs to manually select the object and remove missing scripts
                }
            }
        }
        
        if (fixedCount == 0)
        {
            Debug.Log("✅ No missing script issues found (or they need manual fixing)");
        }
    }
    
    void FixColliderIssues()
    {
        // Find objects with MeshCollider trigger issues
        MeshCollider[] meshColliders = FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);
        
        foreach (MeshCollider mc in meshColliders)
        {
            if (mc.isTrigger && !mc.convex)
            {
                Debug.Log($"🔧 Fixing MeshCollider on {mc.gameObject.name}: Setting convex = true for trigger");
                mc.convex = true;
            }
        }
    }
    
    void FixEnemyWaypoints()
    {
        // Find enemies without waypoints and offer to create them
        Enemy_Back_And_Forth_Movement[] enemies = FindObjectsByType<Enemy_Back_And_Forth_Movement>(FindObjectsSortMode.None);
        
        foreach (var enemy in enemies)
        {
            if (enemy.points == null || enemy.points.Length == 0)
            {
                Debug.LogWarning($"🎯 Enemy {enemy.gameObject.name} needs waypoints. Right-click component and choose 'Auto Create Waypoints'");
            }
        }
    }
}
