using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float panSpeed = 20f;

    void Update()
    {
        // Check if the game window is actually focused to prevent "Frustum" errors
        if (!Application.isFocused) return;

        float x = Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * panSpeed * Time.deltaTime;

        // Move only if there is input
        if (x != 0 || y != 0)
        {
            transform.Translate(x, y, 0);
        }
    }
}