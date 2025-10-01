using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
[RequireComponent(typeof(BoxCollider2D))]
public class WorldZoneFitToCamera : MonoBehaviour
{
    [Header("Target")]
    public Camera targetCamera;

    [Header("Fit Options")]
    public float padding = 0f;     // unidades extra alrededor
    public bool autoUpdate = true; // actualiza en editor y en runtime

    private BoxCollider2D box;

    private void OnEnable()
    {
        box = GetComponent<BoxCollider2D>();
        if (targetCamera == null) targetCamera = Camera.main;
        UpdateSize();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying && autoUpdate)
            UpdateSize();
    }
#endif

    public void UpdateSize()
    {
        if (box == null || targetCamera == null) return;
        if (!targetCamera.orthographic)
        {
            Debug.LogWarning("[WorldZoneFitToCamera] La cámara debe ser ortográfica.");
            return;
        }

        float height = targetCamera.orthographicSize * 2f;
        float width  = height * targetCamera.aspect;

        width  += padding * 2f;
        height += padding * 2f;

        box.isTrigger = true;
        box.size = new Vector2(width, height);
        box.offset = Vector2.zero;

        // centra el collider en la posición de la cámara (Z ignorada)
        var cp = targetCamera.transform.position;
        transform.position = new Vector3(cp.x, cp.y, 0f);
    }
}
