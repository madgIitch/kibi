using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Camera))]
public class PreviewAutoFrame : MonoBehaviour
{
    public Transform root;                 // Avatar
    public float margin = 1.2f;
    public float headBoost = 0.15f;
    public Vector2 angles = new(15f, 170f);
    Camera cam;
    void OnEnable(){
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        var c = cam.backgroundColor; c.a = 1f; cam.backgroundColor = c;
        cam.nearClipPlane = 0.05f; cam.farClipPlane = 100f;
        cam.useOcclusionCulling = false;
    }
    void LateUpdate(){
        if (!root || !cam) return;
        var rends = root.GetComponentsInChildren<Renderer>(true);
        if (rends.Length==0) return;
        var b = new Bounds(rends[0].bounds.center, Vector3.zero);
        foreach (var r in rends) b.Encapsulate(r.bounds);

        var center = b.center + Vector3.up*(b.size.y*headBoost);
        float ext = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);
        float dist = (ext / Mathf.Sin(cam.fieldOfView*Mathf.Deg2Rad*0.5f)) * margin;

        var rot = Quaternion.Euler(angles.x, angles.y, 0f);
        var pos = center - rot * Vector3.forward * dist;
        transform.SetPositionAndRotation(pos, rot);

        cam.nearClipPlane = Mathf.Max(0.03f, dist*0.05f);
        cam.farClipPlane  = Mathf.Max(10f,  dist*10f);
    }
}
