using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 500f;
    public float minHeight = 5f;
    public float maxHeight = 40f;

    void Update()
    {
        // 1. Movement (WASD)
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        
        // Move relative to the camera's looking direction
        transform.Translate(move, Space.World);

        // 2. Zooming (Mouse Wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoom = transform.forward * scroll * zoomSpeed * Time.deltaTime;
        
        // Apply zoom and clamp height so we don't go through the floor
        transform.position += zoom;
        
        float clampedY = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }
}