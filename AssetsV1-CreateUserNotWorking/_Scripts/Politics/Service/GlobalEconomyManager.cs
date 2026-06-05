using UnityEngine;
using System.Collections.Generic;

public class GlobalEconomyManager : MonoBehaviour
{
    public static GlobalEconomyManager Instance;

    [Header("Simulation Settings")]
    private float timer;

    void Awake() { Instance = this; }

    public void ProcessEconomy()
    {
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);

        // 1. Update the BOTTOM level (Towns)
        foreach (Territory t in allTerritories)
        {
            if (t.type != TerritoryType.Town) continue;

            // Growth
            t.population += Mathf.RoundToInt(t.population * 0.05f);

            // Taxes
            float gold = 0;
            foreach (var b in t.currentBuildings) gold += b.goldGeneration;
            gold += (t.population / 100f);

            // Add to Kingdom Treasury
            if (t.ownerKingdom != null) t.ownerKingdom.treasury += gold;
        }

        // 2. IMPORTANT: Recalculate Hierarchy totals
        // We go through Kingdoms and tell them to recalculate down the tree
        foreach (Territory t in allTerritories)
        {
            if (t.type == TerritoryType.Kingdom)
            {
                UpdateHierarchyPopulation(t);
            }
        }
    }

    // Simple recursive function to make sure parents match children
    private void UpdateHierarchyPopulation(Territory parent)
    {
        if (parent.subTerritories.Count == 0) return;

        int total = 0;
        foreach (Territory sub in parent.subTerritories)
        {
            // Recursively ask the children to update first
            UpdateHierarchyPopulation(sub);
            total += sub.population;
        }
        parent.population = total;
    }

    void UpdateKingdomPopulations(Territory[] territories)
    {
        // Logic to sum up all territory pops and assign to the Kingdom total
    }
}