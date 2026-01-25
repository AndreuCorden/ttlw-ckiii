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
        if (town == null || town.territoryType != TerritoryType.Land) return;

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

    // public void RefreshBuildMenu(Territory selectedTerritory)
    // {
    //     // 1. Find the current Town Hall level of this territory
    //     int currentTHLevel = selectedTerritory.GetBuildingLevel("Town Hall");

    //     // 2. Clear your current UI list
    //     foreach (Transform child in buildMenuContainer) Destroy(child.gameObject);

    //     // 3. Loop through your master list of all possible buildings
    //     foreach (BuildingData building in settlementEngine.allPossibleBuildings)
    //     {
    //         // Instantiate a button for this building
    //         GameObject btnObj = Instantiate(buildingButtonPrefab, buildMenuContainer);
    //         BuildButton script = btnObj.GetComponent<BuildButton>();

    //         bool isUnlocked = currentTHLevel >= building.townHallLevelRequired;

    //         // Setup the button visuals
    //         script.Setup(building, isUnlocked);

    //         // If locked, maybe make the button non-interactable or grayed out
    //         btnObj.GetComponent<Button>().interactable = isUnlocked;
    //     }
    // }

    // public void TryBuild(BuildingData building, Territory t)
    // {
    //     int currentTH = t.GetBuildingLevel("Town Hall");

    //     if (currentTH < building.townHallLevelRequired)
    //     {
    //         Debug.Log("Town Hall level too low!");
    //         return;
    //     }

    //     if (t.owner.gold >= building.goldCost)
    //     {
    //         t.owner.gold -= building.goldCost;
    //         t.AddBuilding(building);
    //         RefreshBuildMenu(t); // Update the UI
    //     }
    // }
}