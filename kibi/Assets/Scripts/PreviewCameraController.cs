using UnityEngine;

public class PreviewCameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 3f, minDist = 1.5f, maxDist = 5f;
    public float orbitSpeed = 120f, zoomSpeed = 4f;
    float yaw, pitch = 10f;

    void LateUpdate()
    {
        if (!target) return;

        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * orbitSpeed * 0.6f * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -10f, 60f);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDist, maxDist);

        var rot = Quaternion.Euler(pitch, yaw, 0);
        var pos = target.position + rot * new Vector3(0, 0, -distance);
        transform.SetPositionAndRotation(pos, rot);
    }
}
