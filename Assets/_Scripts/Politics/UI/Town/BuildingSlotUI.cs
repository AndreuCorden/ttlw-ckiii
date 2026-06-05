using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingSlotUI : MonoBehaviour
{
    public Image buildingIcon;
    public TextMeshProUGUI buildingName;

    public void SetBuilding(BuildingData data)
    {
        buildingName.text = data.buildingName;
        if(data.icon != null) buildingIcon.sprite = data.icon;
    }
}