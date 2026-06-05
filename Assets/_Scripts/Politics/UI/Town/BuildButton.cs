using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Image iconImage;
    public Button actionButton;

    private BuildingData data;
    private Territory currentTerritory;

    public void Setup(BuildingData building, Territory territory)
    {
        data = building;
        currentTerritory = territory;

        nameText.text = building.buildingName;
        costText.text = $"Gold: {building.cost}";

        // Clear existing listeners and add the new one
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnBuildClicked);
    }

    private void OnBuildClicked()
    {
        // Tell the manager to build this
        Object.FindAnyObjectByType<TownDisplay>().TryBuild(data, currentTerritory);
    }
}