using UnityEngine;

public class Sims3CameraController : MonoBehaviour
{
    [Header("Rig hierarchy")]
    public Transform yawPivot;
    public Transform pitchPivot;
    public Camera cam;

    [Header("Move (WASD)")]
    public float moveSpeed = 15f;
    public float fastMultiplier = 2f;
    public float slowMultiplier = 0.5f;

    [Header("Rotate (MMB drag)")]
    public float rotateSpeed = 150f;
    public float minPitch = 15f;
    public float maxPitch = 75f;

    [Header("Zoom (Wheel)")]
    public float minDistance = 6f;
    public float maxDistance = 50f;
    public float zoomSpeed = 10f;
    public float zoomDamp = 10f;

    [Header("Edge Scrolling")]
    public bool edgeScroll = true;
    public int edgePixels = 12;         // ancho del borde
    public float edgeSpeed = 12f;

    [Header("Focus (double click)")]
    public LayerMask groundMask;        // asigna tu capa de suelo (p.ej. "Ground")
    public float focusHeight = 0f;      // offset en Y al centrar
    public float focusLerp = 12f;

    // estado
    float targetDistance;
    float currentDistance;
    float pitch; // grados
    Vector3 moveVelocity;               // para focus lerp
    Vector3 targetRigPos;               // destino de focus

    float lastClickTime;
    const float doubleClickWindow = 0.25f;

    void Reset()
    {
        moveSpeed = 15f; fastMultiplier = 2f; slowMultiplier = 0.5f;
        rotateSpeed = 150f; minPitch = 15f; maxPitch = 75f;
        minDistance = 6f; maxDistance = 50f; zoomSpeed = 10f; zoomDamp = 10f;
        edgeScroll = true; edgePixels = 12; edgeSpeed = 12f;
        focusHeight = 0f; focusLerp = 12f;
        groundMask = ~0; // todo
    }

    void Start()
    {
        if (!yawPivot || !pitchPivot)
        { Debug.LogError("Asigna yawPivot y pitchPivot."); enabled = false; return; }
        if (!cam) cam = Camera.main;
        if (!cam)
        { Debug.LogError("Asigna la Camera."); enabled = false; return; }

        currentDistance = Mathf.Abs(cam.transform.localPosition.z);
        if (currentDistance < 0.1f) currentDistance = 15f;
        targetDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        pitch = pitchPivot.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        targetRigPos = transform.position;
        ApplyTransforms(true);
    }

    void Update()
    {
        HandleMove();
        HandleEdgeScroll();
        HandleRotate();
        HandleZoom();
        HandleDoubleClickFocus();

        // si estamos enfocando, interpolamos la posición del rig
        transform.position = Vector3.Lerp(transform.position, targetRigPos, 1f - Mathf.Exp(-focusLerp * Time.deltaTime));

        ApplyTransforms(false);
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Approximately(h,0f) && Mathf.Approximately(v,0f)) return;

        float mult = 1f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) mult *= fastMultiplier;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) mult *= slowMultiplier;

        Vector3 forward = Vector3.ProjectOnPlane(yawPivot.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(yawPivot.right,   Vector3.up).normalized;
        Vector3 delta   = (forward * v + right * h) * moveSpeed * mult * Time.deltaTime;

        targetRigPos += delta;
    }

    void HandleEdgeScroll()
    {
        if (!edgeScroll) return;
        if (!Application.isFocused) return;

        Vector3 delta = Vector3.zero;
        Vector3 forward = Vector3.ProjectOnPlane(yawPivot.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(yawPivot.right,   Vector3.up).normalized;

        Vector3 mouse = Input.mousePosition;
        bool left   = mouse.x <= edgePixels;
        bool rightE = mouse.x >= Screen.width  - edgePixels;
        bool down   = mouse.y <= edgePixels;
        bool up     = mouse.y >= Screen.height - edgePixels;

        if (left)   delta -= right;
        if (rightE) delta += right;
        if (down)   delta -= forward;
        if (up)     delta += forward;

        if (delta.sqrMagnitude > 0.0001f)
            targetRigPos += delta.normalized * edgeSpeed * Time.deltaTime;
    }

    void HandleRotate()
    {
        if (Input.GetMouseButton(2))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            // yaw
            yawPivot.Rotate(Vector3.up, mx * rotateSpeed * Time.deltaTime, Space.World);

            // pitch con clamp
            pitch -= my * rotateSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f)
            targetDistance = Mathf.Clamp(targetDistance - scroll * zoomSpeed, minDistance, maxDistance);

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, 1f - Mathf.Exp(-zoomDamp * Time.deltaTime));
    }

    void HandleDoubleClickFocus()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.unscaledTime - lastClickTime <= doubleClickWindow)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
                {
                    Vector3 p = hit.point;
                    p.y = focusHeight;
                    targetRigPos = p;
                }
            }
            lastClickTime = Time.unscaledTime;
        }
    }

    void ApplyTransforms(bool instant)
    {
        // pitch
        pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // distancia (la cámara está en -Z local del pitchPivot)
        Vector3 desiredLocal = new Vector3(0f, 0f, -currentDistance);
        if (instant) cam.transform.localPosition = desiredLocal;
        else cam.transform.localPosition = Vector3.Lerp(
            cam.transform.localPosition, desiredLocal,
            1f - Mathf.Exp(-zoomDamp * Time.deltaTime)
        );
        cam.transform.localRotation = Quaternion.identity;
    }
}
