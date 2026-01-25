using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TownDisplay : MonoBehaviour
{
    public GameObject displayPanel;
    public TextMeshProUGUI townNameText;
    public TextMeshProUGUI statusText; // e.g., "Provincial Capital"
    public TextMeshProUGUI sizeText;   // e.g., "City"
    public GameObject buildingPrefab;
    public Transform buildingParent;

    public void OpenTownDisplay(Territory town)
    {
        if (town == null || town.type != TerritoryType.Town) return;

        townNameText.text = town.territoryName;
        sizeText.text = $"Settlement Type: {town.size}";

        if (town.isCapital)
        {
            statusText.text = "Strategic Importance: REGIONAL CAPITAL";
            statusText.color = Color.yellow;
        }
        else
        {
            statusText.text = "Strategic Importance: Standard Settlement";
            statusText.color = Color.white;
        }

        foreach (Transform child in buildingParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Spawn a new slot for every building the town actually has
        // Inside TownDisplay.cs
        foreach (BuildingData building in town.currentBuildings)
        {
            GameObject newSlot = Instantiate(buildingPrefab, buildingParent);

            // 1. Set the visuals (The Script that handles the Icon/Text inside the slot)
            if (newSlot.TryGetComponent<BuildingSlotUI>(out var slotUI))
            {
                slotUI.SetBuilding(building);
            }

            // 2. Set the Tooltip (The Script that handles the Hovering)
            if (newSlot.TryGetComponent<HoverTooltipTrigger>(out var trigger))
            {
                trigger.Setup(building); // This works because BuildingData is an IDescribable
            }
        }


        displayPanel.SetActive(true);

    }

    public void CloseDisplay()
    {
        displayPanel.SetActive(false);
    }
}