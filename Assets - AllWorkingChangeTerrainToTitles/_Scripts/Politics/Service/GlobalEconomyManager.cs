using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GlobalEconomyManager : MonoBehaviour
{
    public static GlobalEconomyManager Instance;

    [Header("Simulation Settings")]
    private float timer;

    void Awake() { Instance = this; }

    public void ProcessEconomy()
    {
        // 1. Get ONLY the Towns to do the heavy lifting
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);
        List<Territory> towns = allTerritories.Where(t => t.type == TerritoryType.Town).ToList();
        List<Territory> kingdoms = allTerritories.Where(t => t.type == TerritoryType.Kingdom).ToList();
        foreach (var k in kingdoms)
        {
            k.ownerKingdom.treasury += k.localWealth;
        }

        foreach (Territory t in towns)
        {
            // Population Growth
            t.population += Mathf.RoundToInt(t.population * 0.05f);

            // Calculate Gold for this turn
            // This checks buildings + population tax
            float goldGenerated = t.GetGoldPerTurn();
            goldGenerated += t.population / 100f;

            // Update the local wealth display variable
            t.localWealth = goldGenerated;
        }

        // 2. Refresh the stats for the higher-ups (Counties -> Provinces -> Kingdoms)
        // We do this AFTER all towns are updated
        RefreshHierarchyStats(allTerritories);
    }

    private void RefreshHierarchyStats(Territory[] all)
    {
        // Sort by type so we update bottom-up: County -> Province -> Kingdom
        // This ensures that when a Kingdom calculates pop, its Provinces are already accurate
        List<Territory> counties = all.Where(t => t.type == TerritoryType.County).ToList();
        List<Territory> provinces = all.Where(t => t.type == TerritoryType.Province).ToList();
        List<Territory> kingdoms = all.Where(t => t.type == TerritoryType.Kingdom).ToList();

        foreach (var t in counties)
        {
            t.population = t.subTerritories.Sum(sub => sub.population);
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
        foreach (var t in provinces)
        {
            t.population = t.subTerritories.Sum(sub => sub.population);
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
        foreach (var t in kingdoms)
        {
            t.population = t.subTerritories.Sum(sub => sub.population);
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
    }

    public void ProcessFeudalEconomy()
{
    // 1. All Towns generate base gold
    var allTowns = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None)
                          .Where(t => t.type == TerritoryType.Town);

    foreach (Territory town in allTowns)
    {
        float gold = town.GetGoldPerTurn() + (town.population / 100f);
        town.localWealth = gold;
        
        // Pass the money to the immediate parent (the County ruler)
        if (town.parentTerritory != null)
        {
            town.parentTerritory.personalTreasury += gold;
        }
    }

    // 2. Pass taxes UP the chain (County -> Province -> Kingdom)
    // We sort by type to ensure we process bottom-up
    var hierarchy = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None)
                          .OrderByDescending(t => (int)t.type).ToList();

    foreach (Territory t in hierarchy)
    {
        if (t.type == TerritoryType.Kingdom || t.type == TerritoryType.Town) continue;

        if (t.parentTerritory != null)
        {
            float taxAmount = t.personalTreasury * t.taxRateToLord;
            t.personalTreasury -= taxAmount;
            t.parentTerritory.personalTreasury += taxAmount;
        }
    }
}
}