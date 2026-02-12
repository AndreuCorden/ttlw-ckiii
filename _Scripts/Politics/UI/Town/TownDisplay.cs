using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TownDisplay : MonoBehaviour
{
    [Header("Main View (Current Buildings)")]
    public GameObject displayPanel;
    public TextMeshProUGUI townNameText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI sizeText;
    public GameObject buildingPrefab; // Prefab for buildings already built
    public Transform buildingParent;

    [Header("Build Menu (Construction)")]
    public GameObject buildViewPanel; // The Panel that shows possible builds
    public GameObject buildingButtonPrefab; // Prefab for the build buttons
    public Transform buildMenuContainer; // The Content object for the buttons

    private Territory currentOpenedTown;
    public BuildingLibrary buildingLibrary;

    // --- MAIN DISPLAY LOGIC ---

    public void OpenTownDisplay(Territory town)
    {
        if (town == null || town.territoryType != TerritoryType.Land) return;
        currentOpenedTown = town;

        townNameText.text = town.territoryName;
        sizeText.text = $"Settlement Type: {town.size}";

        // Set Strategic Importance
        statusText.text = town.isCapital ? "Strategic Importance: REGIONAL CAPITAL" : "Strategic Importance: Standard Settlement";
        statusText.color = town.isCapital ? Color.yellow : Color.white;

        RefreshCurrentBuildings();
        displayPanel.SetActive(true);
    }

    private void RefreshCurrentBuildings()
    {
        foreach (Transform child in buildingParent) Destroy(child.gameObject);

        foreach (BuildingData building in currentOpenedTown.currentBuildings)
        {
            GameObject newSlot = Instantiate(buildingPrefab, buildingParent);

            if (newSlot.TryGetComponent<BuildingSlotUI>(out var slotUI))
                slotUI.SetBuilding(building);

            if (newSlot.TryGetComponent<HoverTooltipTrigger>(out var trigger))
                trigger.Setup(building);
        }
    }

    public void CloseDisplay()
    {
        displayPanel.SetActive(false);
        buildViewPanel.SetActive(false);
    }

    // --- CONSTRUCTION MENU LOGIC ---

    public void OpenBuildMenu()
    {
        if (currentOpenedTown == null) return;

        buildViewPanel.SetActive(true);
        RefreshBuildMenu(currentOpenedTown);
    }

    public void RefreshBuildMenu(Territory selectedTerritory)
    {
        foreach (Transform child in buildMenuContainer) Destroy(child.gameObject);

        foreach (BuildingData building in selectedTerritory.buildableBuildings)
        {
            GameObject btnObj = Instantiate(buildingButtonPrefab, buildMenuContainer);
            BuildButton script = btnObj.GetComponent<BuildButton>();

            // Comparison happens here: The library doesn't need to know the buildings,
            // because the building knows its own requirement.

            script.Setup(building, selectedTerritory);
        }
    }

    public void TryUpgradeTownHall(Territory t)
    {
        int currentLevel = t.GetBuildingLevel("Town Hall");
        int nextLevelIndex = currentLevel; // If current level is 1 (index 0), next is index 1

        if (nextLevelIndex < buildingLibrary.townHallLevels.Count)
        {
            BuildingData nextTH = buildingLibrary.townHallLevels[nextLevelIndex];

            if (t.owner.holder.treasury >= nextTH.cost)
            {
                t.owner.holder.treasury -= nextTH.cost;
                t.AddBuilding(nextTH); // Your AddBuilding logic handles replacing the old one

                RefreshCurrentBuildings();
                RefreshBuildMenu(t);
            }
        }
        else
        {
            Debug.Log("Town Hall is already at Max Level!");
        }
    }

    public void TryBuild(BuildingData building, Territory t)
    {
        int currentTH = t.GetBuildingLevel("Town Hall");

        // Fixed: Check level and cost correctly
        if (currentTH < building.level) // Also need to change for townhalllevel
        {
            Debug.Log("Town Hall level too low!");
            return;
        }

        // Assuming Title.holder has the treasury
        if (t.owner.holder.treasury >= building.cost)
        {
            t.owner.holder.treasury -= building.cost;
            t.AddBuilding(building);

            // Refresh both UIs
            RefreshCurrentBuildings();
            RefreshBuildMenu(t);
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void CloseBuildDisplay()
    {
        buildViewPanel.SetActive(false);
    }
}