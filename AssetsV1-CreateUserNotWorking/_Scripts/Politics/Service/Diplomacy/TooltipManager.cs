using UnityEngine;
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameObject tooltipBox;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI descText;

    void Awake()
    {
        // This ensures that the Manager is ready the moment the game starts
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("TooltipManager initialized successfully.");
        }
    }

    public void ShowTooltip(string name, string desc, Vector3 position)
    {
        tooltipBox.SetActive(true);
        nameText.text = name;
        descText.text = desc;
        tooltipBox.transform.position = position + new Vector3(0, 0, 0); // Offset it above the icon
    }

    void Update()
    {
        if (tooltipBox.activeSelf)
        {
            Vector3 mousePos = Input.mousePosition;

            // Get the width and height of the tooltip box
            RectTransform rect = tooltipBox.GetComponent<RectTransform>();
            float width = rect.rect.width;
            float height = rect.rect.height;

            // Calculate target position with offset
            float targetX = mousePos.x - 15;
            float targetY = mousePos.y + 15;

            // Clamp the position so it stays within the screen edges
            // Screen.width and Screen.height are your boundaries
            targetX = Mathf.Clamp(targetX, 0, Screen.width - width);
            targetY = Mathf.Clamp(targetY, 0, Screen.height - height);

            tooltipBox.transform.position = new Vector3(targetX, targetY, 0);
        }
    }

    public void HideTooltip() { tooltipBox.SetActive(false); }
}