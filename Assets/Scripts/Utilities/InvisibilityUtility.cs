using UnityEngine;

public static class InvisibilityUtility
{
    public static void SetInvisible (Transform root, float alpha = 0.3f)
    {
        foreach (var rend in root.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                SetTransparentMaterial(mat, alpha);
            }
        }
    }

    public static void SetVisible(Transform root)
    {
        foreach (var rend in root.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                SetOpaqueMaterial(mat);
            }
        }
    }

    private static void SetTransparentMaterial(Material mat, float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private static void SetOpaqueMaterial(Material mat)
    {
        Color c = mat.color;
        c.a = 1f;
        mat.color = c;

        mat.SetFloat("_Mode", 0); // Opaque
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.EnableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}
