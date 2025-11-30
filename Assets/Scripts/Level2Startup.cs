using UnityEngine;

public class Level2Startup : MonoBehaviour
{
    [Header("Level 2 Setup")]
    [SerializeField] private bool autoFixIssues = true;
    [SerializeField] private bool verboseLogging = true;
    
    void Start()
    {
        if (verboseLogging)
            Debug.Log("🎮 Level 2 starting up...");
            
        if (autoFixIssues)
        {
            FixLevelIssues();
        }
    }
    
    void FixLevelIssues()
    {
        
        CheckAudioManager();
        
        CheckPlayer();
        
        CheckEnemies();
        
        FixColliders();
        
        // 5. Fix duplicate  listeners
        FixAudioListeners();
        
        // 6. Fix duplicate event systems
        FixEventSystems();
        
        if (verboseLogging)
            Debug.Log("✅ Level 2 setup checks completed!");
    }
    
    void CheckAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("🎵 No AudioManager found. Audio may not work properly.");
            
            // Try to find one in the scene
            AudioManager am = FindFirstObjectByType<AudioManager>();
            if (am == null)
            {
                Debug.Log("🔧 Creating AudioManager automatically...");
                GameObject amObj = new GameObject("AudioManager");
                amObj.AddComponent<AudioManager>();
            }
        }
        else
        {
            if (verboseLogging)
                Debug.Log("✅ AudioManager found and working");
        }
    }
    
    void CheckPlayer()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogWarning("🎮 No PlayerMovement script found! Player may not work.");
        }
        else
        {
            if (verboseLogging)
                Debug.Log("✅ Player found and ready");
        }
    }
    
    void CheckEnemies()
    {
        Enemy_Back_And_Forth_Movement[] enemies = FindObjectsByType<Enemy_Back_And_Forth_Movement>(FindObjectsSortMode.None);
        
        if (enemies.Length == 0)
        {
            if (verboseLogging)
                Debug.Log("ℹ️ No enemies with movement scripts found");
            return;
        }
        
        foreach (var enemy in enemies)
        {
            if (enemy.points == null || enemy.points.Length == 0 || HasNullPoints(enemy.points))
            {
                Debug.LogWarning($"⚠️ Enemy '{enemy.gameObject.name}' has missing waypoints! Auto-creating...");
                
                // Auto-create waypoints for this enemy
                CreateWaypointsForEnemy(enemy);
            }
            else
            {
                Debug.Log($"✅ Enemy '{enemy.gameObject.name}' has {enemy.points.Length} waypoints");
            }
        }
    }
    
    bool HasNullPoints(Transform[] points)
    {
        if (points == null) return true;
        
        foreach (Transform point in points)
        {
            if (point == null) return true;
        }
        return false;
    }
    
    void CreateWaypointsForEnemy(Enemy_Back_And_Forth_Movement enemy)
    {
        // Create waypoints automatically
        GameObject waypoint1 = new GameObject($"{enemy.gameObject.name}_Waypoint1");
        GameObject waypoint2 = new GameObject($"{enemy.gameObject.name}_Waypoint2");
        
        // Set waypoint positions: current position and 3 units to the right
        waypoint1.transform.position = enemy.transform.position;
        waypoint2.transform.position = enemy.transform.position + enemy.transform.right * 3f;
        
        // Assign waypoints to enemy
        enemy.points = new Transform[] { waypoint1.transform, waypoint2.transform };
        
        Debug.Log($"🔧 Auto-created waypoints for enemy '{enemy.gameObject.name}'");
    }
    
    void FixColliders()
    {
        // Fix MeshCollider trigger issues
        MeshCollider[] meshColliders = FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        foreach (MeshCollider mc in meshColliders)
        {
            if (mc.isTrigger && !mc.convex)
            {
                mc.convex = true;
                fixedCount++;
                if (verboseLogging)
                    Debug.Log($"🔧 Fixed MeshCollider on {mc.gameObject.name}");
            }
        }
        
        if (fixedCount > 0)
        {
            Debug.Log($"✅ Fixed {fixedCount} collider issues");
        }
    }
    
    void FixAudioListeners()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        
        if (listeners.Length > 1)
        {
            Debug.LogWarning($"⚠️ Found {listeners.Length} Audio Listeners! Only one should exist.");
            
            // Keep the first one (usually on Main Camera), disable others
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
                Debug.Log($"🔧 Disabled extra Audio Listener on '{listeners[i].gameObject.name}'");
            }
            
            Debug.Log($"✅ Fixed Audio Listener issue - kept listener on '{listeners[0].gameObject.name}'");
        }
    }
    
    void FixEventSystems()
    {
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"⚠️ Found {eventSystems.Length} Event Systems! Only one should exist.");
            
            // Keep the first one, destroy others
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (verboseLogging)
                    Debug.Log($"🔧 Removing extra Event System from '{eventSystems[i].gameObject.name}'");
                
                Destroy(eventSystems[i].gameObject);
            }
            
            Debug.Log($"✅ Fixed Event System issue - kept system on '{eventSystems[0].gameObject.name}'");
        }
    }
}
