using UnityEngine;

public class InvisibleEffectController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float effectStrength = 0f;

    private Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void MakeInvisible()
    {
        effectStrength = 1f;
        ApplyEffect();
    }

    public void MakeVisible()
    {
        effectStrength = 0f;
        ApplyEffect();
    }

    private void ApplyEffect()
    {
        foreach (var rend in renderers)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_InvisibleAmount"))
                {
                    mat.SetFloat("_InvisibleAmount", effectStrength);
                }
            }
        }
    }
}
