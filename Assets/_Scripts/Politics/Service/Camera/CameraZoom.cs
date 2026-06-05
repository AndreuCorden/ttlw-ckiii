using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;

    [Header("Zoom Settings")]
    public float zoomSpeed;
    public float minZoom = 1f;
    public float maxZoom = 20f;
    
    // Smoothness
    public float smoothTime = 0.15f;
    private float targetZoom;
    private float velocity = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        // Input.mouseScrollDelta works for both Scroll Wheels and Thumb Pad scrolling/pinching
        float scrollInput = Input.mouseScrollDelta.y;

        if (scrollInput != 0)
        {
            // Subtracting because 'Up' scroll usually means 'In'
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Smoothly transition to the target zoom
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref velocity, smoothTime);
    }
}