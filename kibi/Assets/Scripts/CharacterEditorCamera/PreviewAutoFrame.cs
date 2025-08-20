using UnityEngine;

[ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public class PreviewAutoFrame : MonoBehaviour
{
    [Header("Target")]
    public Transform root;                 // Avatar

    [Header("Framing")]
    public float margin = 1.2f;
    public float headBoost = 0.15f;

    [Header("Zoom")]
    public float minZoom = 0.8f;
    public float maxZoom = 1.6f;
    public float zoomSpeed = 0.8f;

    [Header("Orbit")]
    public float rotateSpeed = 3f;   // sensibilidad de giro
    private float yaw = 170f;        // eje Y
    private float pitch = 15f;       // eje X
    private float roll = 0f;         // eje Z opcional

    private float zoomFactor = 1f;
    private Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        if (!cam) return;

        cam.clearFlags = CameraClearFlags.SolidColor;
        var c = cam.backgroundColor; c.a = 1f; cam.backgroundColor = c;
        cam.useOcclusionCulling = false;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 100f;
    }

    void Update()
    {
        // --- Zoom ---
        float scroll = 0f;

        #if ENABLE_LEGACY_INPUT_MANAGER
        scroll = Input.GetAxis("Mouse ScrollWheel");
        #endif

        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
            scroll += UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y / 120f;
        #endif

        if (Mathf.Abs(scroll) > 0.0001f)
            zoomFactor = Mathf.Clamp(zoomFactor - scroll * zoomSpeed, minZoom, maxZoom);

        // --- Rotación con rueda pulsada ---
        #if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButton(2)) // 2 = MMB
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");

            yaw   += dx * rotateSpeed;
            pitch -= dy * rotateSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f); // evita volcar la cámara
        }
        #endif

        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null &&
            UnityEngine.InputSystem.Mouse.current.middleButton.isPressed)
        {
            var delta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();
            yaw   += delta.x * rotateSpeed * 0.1f;
            pitch -= delta.y * rotateSpeed * 0.1f;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
        }
        #endif
    }

    void LateUpdate()
    {
        if (!root || !cam) return;

        // Bounds combinados del avatar
        var rends = root.GetComponentsInChildren<Renderer>(true);
        if (rends.Length == 0) return;

        var b = new Bounds(rends[0].bounds.center, Vector3.zero);
        foreach (var r in rends) b.Encapsulate(r.bounds);

        var center = b.center + Vector3.up * (b.size.y * headBoost);
        float ext = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);

        float baseDist = (ext / Mathf.Sin(cam.fieldOfView * Mathf.Deg2Rad * 0.5f)) * margin;
        float dist = baseDist * zoomFactor;

        // Aplica rotación (yaw/pitch/roll)
        var rot = Quaternion.Euler(pitch, yaw, roll);
        var pos = center - rot * Vector3.forward * dist;
        transform.SetPositionAndRotation(pos, rot);

        cam.nearClipPlane = Mathf.Max(0.03f, dist * 0.05f);
        cam.farClipPlane  = Mathf.Max(10f,  dist * 10f);
    }
}
