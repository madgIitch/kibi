using UnityEngine;
using Cinemachine;

public class FreeLookZoom : MonoBehaviour
{
    public CinemachineFreeLook freeLook;
    public float zoomSpeed = 2f;
    public float minFOV = 25f;
    public float maxFOV = 70f;

    void Update()
    {
        if (freeLook == null) return;
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            var lens = freeLook.m_Lens;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView - scroll * zoomSpeed, minFOV, maxFOV);
            freeLook.m_Lens = lens;
        }
    }
}
