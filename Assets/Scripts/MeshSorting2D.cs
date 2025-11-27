<<<<<<< HEAD
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshSorting2D : MonoBehaviour
{
    public string sortingLayerName = "Characters";
    public int sortingOrderBase = 0; // per-prefab tweak
    public float precision = 100f;   // higher = finer sorting

    Renderer r;

    void Awake()
    {
        r = GetComponent<Renderer>();
        r.sortingLayerName = sortingLayerName;
    }

    void LateUpdate()
    {
        // Lower Z (closer to camera bottom) should render on top
        r.sortingOrder = sortingOrderBase - Mathf.RoundToInt(transform.position.z * precision);
    }
}
=======
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshSorting2D : MonoBehaviour
{
    public string sortingLayerName = "Characters";
    public int sortingOrderBase = 0; // per-prefab tweak
    public float precision = 100f;   // higher = finer sorting

    Renderer r;

    void Awake()
    {
        r = GetComponent<Renderer>();
        r.sortingLayerName = sortingLayerName;
    }

    void LateUpdate()
    {
        // Lower Z (closer to camera bottom) should render on top
        r.sortingOrder = sortingOrderBase - Mathf.RoundToInt(transform.position.z * precision);
    }
}
>>>>>>> origin/Matt
