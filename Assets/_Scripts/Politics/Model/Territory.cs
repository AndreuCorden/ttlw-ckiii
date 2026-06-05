using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Territory : MonoBehaviour
{
    [Header("Physical Data")]
    public string territoryName;
    public SettlementSize size;
    public bool isCapital = false;
    public int population;
    public int localWealth; // For UI display of wealth generated this turn
    public TerritoryType territoryType;
    public Title county;
    public Title duchy;
    public Title kingdom;
    public Title owner;

    [Header("Visuals")]
    public GameObject capitalIcon;
    public GameObject isDisplayed;

    [Header("Infrastructure")]
    public List<BuildingData> currentBuildings = new List<BuildingData>();
    public List<BuildingData> buildableBuildings = new List<BuildingData>();
    public float instability = 0;

    [Header("Adjacency")]
    public List<Territory> neighbors = new List<Territory>();

    public SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool HasBuilding(string buildingName)
    {
        return currentBuildings.Any(b => b.buildingName == buildingName);
    }

    public void RefreshUI()
    {
        if (capitalIcon != null)
        {
            capitalIcon.SetActive(isCapital);

            // Optional: Scale the icon based on how big the settlement is
            if (isCapital)
            {
                float scale = size switch
                {
                    SettlementSize.BigVillage => 0.2f,
                    SettlementSize.SmallTown => 0.3f,
                    SettlementSize.MarketTown => 0.4f,
                    SettlementSize.Borough => 0.5f,
                    SettlementSize.BigTown => 0.65f,
                    SettlementSize.SmallCity => 0.8f,
                    SettlementSize.City => 0.95f,
                    _ => 0.0f,
                };
                capitalIcon.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    public int GetBuildingLevel(string buildingName)
    {
        foreach (BuildingData b in currentBuildings)
        {
            if (b.buildingName == buildingName)
            {
                return b.level;
            }
        }
        return 0; // Not built
    }

    public float GetGoldPerTurn() // Changed to float to prevent rounding errors
    {
        float total = 0;
        foreach (var b in currentBuildings)
        {
            total += b.GetGoldPerTurn();
        }
        // Add the population tax here too so it's included in the "Town's output"
        total += population / 100f;
        localWealth = Mathf.RoundToInt(total);
        return total;
    }

    public void AddBuilding(BuildingData buildingData)
    {
        // 1. Find the existing building if it exists
        // We use FirstOrDefault to get the actual object reference
        var existingBuilding = currentBuildings.FirstOrDefault(b => b.buildingName == buildingData.buildingName);

        if (existingBuilding != null)
        {
            // 2. Remove the old version (useful for upgrading levels/stats)
            currentBuildings.Remove(existingBuilding);
        }

        // 3. Add the new building data
        currentBuildings.Add(buildingData);
    }

    public void IsDisplayed(bool isBeingDisplayed)
    {
        if(isBeingDisplayed)
        {
            isDisplayed.SetActive(true);
        }
        else
        {
            isDisplayed.SetActive(false);
        }
    }
    // Inside Territory.cs
    // public bool MeetsRequirements(BuildingData b)
    // {
    //     if (GetBuildingLevel("Town Hall") < b.townHallLevelRequired.level) return false;
    //     if (this.population < b.requiredPopulation) return false;

    //     if (!string.IsNullOrEmpty(b.prerequisiteBuildingName))
    //     {
    //         if (GetBuildingLevel(b.prerequisiteBuildingName) < 1) return false;
    //     }

    //     return true;
    // }
}

public enum SettlementSize { Hamlet, Village, BigVillage, SmallTown, MarketTown, Borough, BigTown, SmallCity, City }