using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
[DisallowMultipleComponent]
public class ZoneGizmo : MonoBehaviour
{
    public enum ZoneType { Custom }

    [Header("Config")]
    public ZoneType type = ZoneType.Custom;
    public Color customColor = new Color(0.6f, 0.3f, 0.9f, 1f); // violeta
    [Range(0f, 1f)] public float fillAlpha = 0.15f;
    public bool drawLabel = true;

    private Color GetColor() => customColor;

    private void OnDrawGizmos()
    {
        var bc2d = GetComponent<BoxCollider2D>();
        if (bc2d == null) return;

        var c = GetColor();
        var pos = (Vector3)bc2d.bounds.center;
        var size = (Vector3)bc2d.bounds.size;

        var fill = c; fill.a = fillAlpha;
        Gizmos.color = fill;      Gizmos.DrawCube(pos, size);
        Gizmos.color = c;         Gizmos.DrawWireCube(pos, size);

#if UNITY_EDITOR
        if (drawLabel)
        {
            UnityEditor.Handles.color = c;
            UnityEditor.Handles.Label(pos + new Vector3(0, size.y * 0.5f + 0.2f, 0), "PlazaZone");
        }
#endif
    }

    private void Reset()
    {
        var bc2d = GetComponent<BoxCollider2D>();
        if (bc2d) bc2d.isTrigger = true;
    }
}
