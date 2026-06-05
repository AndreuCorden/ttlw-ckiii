using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage; // Drag the Image component from this Prefab here in Inspector
    private IDescribable assignedObject;

    public void Setup(IDescribable describable)
    {
        if (describable == null) return;
        assignedObject = describable;

        // Sets the UI Image to the sprite defined in the ScriptableObject
        iconImage.sprite = describable.GetIcon();

        // Optional: Name the object for easier debugging in the Hierarchy
        gameObject.name = "Icon_" + describable.GetName();
    }

    public void OnPointerEnter(PointerEventData eventData)
{
    Debug.Log("In");
    if (TooltipManager.Instance != null && assignedObject != null)
    {
        TooltipManager.Instance.ShowTooltip(
            assignedObject.GetName(), 
            assignedObject.GetDescription(), 
            transform.position
        );
        Debug.Log("In again");
    }
}

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}