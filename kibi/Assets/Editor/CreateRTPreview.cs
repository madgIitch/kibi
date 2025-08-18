using UnityEditor;
using UnityEngine;

public static class CreateRTPreview
{
    [MenuItem("Tools/Create RT_Preview (1024)")]
    public static void CreateRT()
    {
        const string folder = "Assets/Render";
        const string path = folder + "/RT_Preview.renderTexture";

        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "Render");

        // Crea un RT 1024x1024, depth 24
        var rt = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32)
        {
            name = "RT_Preview",
            antiAliasing = 1,
            useMipMap = false,
            autoGenerateMips = false,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        AssetDatabase.CreateAsset(rt, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);

        Debug.Log("Created RenderTexture at " + path);
    }
}
