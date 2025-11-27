using UnityEngine;

public class Enemy_Back_And_Forth_Movement : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2.0f;
    public float reachDist = 0.1f;
    public float turnSpeedDegPerSec = 360f;   // how fast to rotate

    int i = 0;      // current target index
    int dir = 1;    // 1 = forward, -1 = backward
    
    void Start()
    {
        // Validate setup on start
        ValidateSetup();
    }

    void Update()
    {
        // Better null checking with helpful error messages
        if (points == null)
        {
            Debug.LogWarning($"Enemy_Back_And_Forth_Movement on {gameObject.name}: Points array is null! Please assign waypoints.");
            return;
        }
        
        if (points.Length == 0)
        {
            Debug.LogWarning($"Enemy_Back_And_Forth_Movement on {gameObject.name}: Points array is empty! Please assign waypoints.");
            return;
        }
        
        // Check if any points are null
        if (points[i] == null)
        {
            Debug.LogWarning($"Enemy_Back_And_Forth_Movement on {gameObject.name}: Point {i} is null! Please assign all waypoints.");
            return;
        }

        // Current target point
        Vector3 targetPos = points[i].position;

        // --------- ROTATE TOWARDS TARGET ---------
        // Direction to target (ignore vertical for Y-rotation only)
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeedDegPerSec * Time.deltaTime
            );
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
        if (Vector3.Distance(transform.position, targetPos) <= reachDist)
        {
            if (i == 0) dir = 1;
            else if (i == points.Length - 1) dir = -1;

            i += dir;
        }
    }
    
    void ValidateSetup()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogError($"❌ {gameObject.name}: Enemy movement needs waypoints! Create empty GameObjects and assign them to 'Points' array.");
            enabled = false; // Disable this component to prevent errors
            return;
        }
        
        // Check for null points
        for (int j = 0; j < points.Length; j++)
        {
            if (points[j] == null)
            {
                Debug.LogError($"❌ {gameObject.name}: Point {j} is null! Please assign all waypoints.");
                enabled = false;
                return;
            }
        }
        
        Debug.Log($"✅ {gameObject.name}: Enemy movement setup correctly with {points.Length} waypoints.");
    }
    
    // Helper method to create waypoints automatically
    [ContextMenu("Auto Create Waypoints")]
    void CreateDefaultWaypoints()
    {
        if (points != null && points.Length > 0)
        {
            Debug.Log("Waypoints already exist. Remove them first if you want to recreate.");
            return;
        }
        
        // Create two waypoints: current position and 5 units forward
        GameObject waypoint1 = new GameObject($"{gameObject.name}_Waypoint1");
        GameObject waypoint2 = new GameObject($"{gameObject.name}_Waypoint2");
        
        waypoint1.transform.position = transform.position;
        waypoint2.transform.position = transform.position + transform.forward * 5f;
        
        points = new Transform[] { waypoint1.transform, waypoint2.transform };
        
        Debug.Log($"✅ Created default waypoints for {gameObject.name}");
    }
}
