using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float panSpeed = 20f;
    private MapGenerator mapGen;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        mapGen = Object.FindAnyObjectByType<MapGenerator>();
    }

    void Update()
    {
        if (!Application.isFocused) return;

        // 1. Get Input
        float xInput = Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime;
        float yInput = Input.GetAxis("Vertical") * panSpeed * Time.deltaTime;

        // 2. Calculate New Position
        Vector3 newPos = transform.position + new Vector3(xInput, yInput, 0);

        // 3. Constrain to Map
        if (mapGen != null)
        {
            newPos = ClampCamera(newPos);
        }

        transform.position = newPos;
    }

    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        // Shift boundaries by 0.5 to account for tile center pivots
        float minX = -0.5f + camWidth;
        float maxX = (mapGen.width * mapGen.spacing) - 0.5f - camWidth;

        float minY = -0.5f + camHeight;
        float maxY = (mapGen.height * mapGen.spacing) - 0.5f - camHeight;

        // Centering logic if map is smaller than screen
        if (maxX < minX)
            targetPosition.x = ((mapGen.width * mapGen.spacing) - 1f) / 2f;
        else
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        if (maxY < minY)
            targetPosition.y = ((mapGen.height * mapGen.spacing) - 1f) / 2f;
        else
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }
}