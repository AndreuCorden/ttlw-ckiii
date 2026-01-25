using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SettlementEngine : MonoBehaviour
{

    public void MarkAsCapital(List<Title> kingdoms)
    {
        foreach (Title kingdom in kingdoms)
        {
            kingdom.seatOfPower.isCapital = true;
            MarkAsCapital(kingdom.vassals);
        }
    }

    // 1. Update the signature to accept the 'level' or 'container'
    private void CapitalSettlementSize(Territory t)
    {
        t.isCapital = true;

        // Determine size based on the RANK of the Title living there
        t.size = t.owner.rank switch
        {
            TitleRank.King => (SettlementSize)Random.Range((int)SettlementSize.SmallCity, (int)SettlementSize.City + 1),
            TitleRank.Duke => (SettlementSize)Random.Range((int)SettlementSize.Borough, (int)SettlementSize.BigTown + 1),
            TitleRank.Count => (SettlementSize)Random.Range((int)SettlementSize.SmallTown, (int)SettlementSize.MarketTown + 1),
            _ => SettlementSize.Village
        };

        t.RefreshUI();
    }

    public void AssignTerritorySizeAndPopulation()
    {
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);
        foreach (Territory t in allTerritories)
        {
            if (!t.isCapital)
            {
                t.size = GetRandomWeightedSize();
            }
            else
            {
                CapitalSettlementSize(t);
            }
            SetPopulation(t);
            SetBuildings(t);
        }
    }

    public SettlementSize GetRandomWeightedSize()
    {
        // Roll two random numbers and average them
        float roll1 = Random.Range(0f, 1f);
        float roll2 = Random.Range(0f, 1f);
        float averaged = (roll1 + roll2) / 2f;

        // Scale that 0-1 value to your Enum count
        int enumCount = System.Enum.GetValues(typeof(SettlementSize)).Length;
        int index = Mathf.FloorToInt(averaged * enumCount);

        return (SettlementSize)index;
    }

    public void SetPopulation(Territory t)
    {
        // Simple population assignment based on settlement size
        float capitalBonus = t.isCapital ? 1.5f : 1.0f;
        switch (t.size)
        {
            case SettlementSize.Hamlet:
                t.population = (int)(capitalBonus * Random.Range(50, 500));
                break;
            case SettlementSize.Village:
                t.population = (int)(capitalBonus * Random.Range(500, 1000));
                break;
            case SettlementSize.BigVillage:
                t.population = (int)(capitalBonus * Random.Range(1000, 3000));
                break;
            case SettlementSize.SmallTown:
                t.population = (int)(capitalBonus * Random.Range(3000, 5000));
                break;
            case SettlementSize.MarketTown:
                t.population = (int)(capitalBonus * Random.Range(5000, 7000));
                break;
            case SettlementSize.Borough:
                t.population = (int)(capitalBonus * Random.Range(7000, 10000));
                break;
            case SettlementSize.BigTown:
                t.population = (int)(capitalBonus * Random.Range(10000, 15000));
                break;
            case SettlementSize.SmallCity:
                t.population = (int)(capitalBonus * Random.Range(15000, 50000));
                break;
            case SettlementSize.City:
                t.population = (int)(capitalBonus * Random.Range(50000, 200000));
                break;
            default:
                t.population = 0;
                break;
        }
    }

    public void SetBuildings(Territory t)
    {
        // Simple building assignment based on settlement size
        BuildingLibrary buildingLibrary = Resources.Load<BuildingLibrary>("BuildingLibrary");
        if (buildingLibrary == null)
        {
            Debug.LogError("BuildingLibrary not found in Resources!");
            return;
        }

        // Always add the Town Hall of appropriate level
        BuildingData townHall = buildingLibrary.townHallLevels[Mathf.Clamp((int)t.size, 0, buildingLibrary.townHallLevels.Count - 1)];
        t.currentBuildings.Add(townHall);

        // Add additional buildings based on size
        int additionalBuildings = System.Math.Min((int)t.size, buildingLibrary.allPossibleBuildings.Count); // More buildings for larger settlements
        int numRep = 0;
        while (t.currentBuildings.Count < (additionalBuildings + 1) && numRep < 20)
        {
            int randomIndex = Random.Range(0, buildingLibrary.allPossibleBuildings.Count);
            BuildingData buildingToAdd = buildingLibrary.allPossibleBuildings[randomIndex];
            while (buildingToAdd.level < additionalBuildings)
            {
                if (buildingToAdd.nextUpgrade != null)
                {
                    buildingToAdd = buildingToAdd.nextUpgrade;
                }
                else
                {
                    break;
                }
            }
            if (!t.currentBuildings.Contains(buildingToAdd))
            {
                t.currentBuildings.Add(buildingToAdd);
            }
            numRep++;
        }
    }

    public void RunInitialEconomySimulation()
    {
        Territory[] territories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);

        foreach (Territory t in territories)
        {
            // B. Calculate Gold based on THIS new population
            float goldGenerated = t.GetGoldPerTurn();
            goldGenerated += t.population / 100f;

            // C. Store it
            t.localWealth = (int)goldGenerated;

            // Note: We don't add to treasury here yet because the game hasn't "started"
        }

        // D. Update the hierarchy so Counties/Kingdoms show the sum of these new numbers
        RefreshHierarchyStats();
    }

    private void RefreshHierarchyStats()
    {
        Title[] counties = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
            .Where(t => t.rank == TitleRank.Count).ToArray();
        Title[] provinces = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
            .Where(t => t.rank == TitleRank.Duke).ToArray();
        Title[] kingdoms = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
            .Where(t => t.rank == TitleRank.King).ToArray();

        foreach (var t in counties)
        {
            // Update localWealth for the UI/Summary
            t.personalTreasury = t.directDomain.Sum(sub => sub.localWealth);
        }
        foreach (var t in provinces)
        {
            // Update localWealth for the UI/Summary
            t.personalTreasury = t.vassals.Sum(sub => sub.personalTreasury);
        }
        foreach (var t in kingdoms)
        {
            // Update localWealth for the UI/Summary
            t.personalTreasury = t.vassals.Sum(sub => sub.personalTreasury);
        }
    }

    public void RunInitialPopulationAddition(List<Title> kingdoms, List<Territory> allLand)
    {
        foreach (Title king in kingdoms)
        {
            int pop = 0;
            foreach (Territory land in king.directDomain)
            {
                pop += land.population;
            }
            king.personalPopulation = pop;
            foreach (Title lord in king.vassals)
            {
                RefreshPopulationStats(lord);
            }
        }
        foreach (Territory land in allLand)
        {
            land.county.totalPopulation += land.population;
            land.duchy.totalPopulation += land.population;
            land.kingdom.totalPopulation += land.population;
        }
    }

    private void RefreshPopulationStats(Title lord)
    {
        int pop = 0;
        foreach (Territory land in lord.directDomain)
        {
            pop += land.population;
        }
        foreach (Title vassal in lord.vassals)
        {
            RefreshPopulationStats(vassal);
        }
        lord.personalPopulation = pop;
    }

    // public void ExecuteVassalAI(Territory vassalTerritory)
    // {
    //     if (vassalTerritory.isPlayerControlled) return; // Don't let AI spend player money

    //     // If they have enough money to build something
    //     if (vassalTerritory.personalTreasury > 500)
    //     {
    //         BuildingData choice = DecideBuilding(vassalTerritory.currentFocus);
    //         // Build in a random town within this territory
    //         Territory target = GetRandomTownRecursive(vassalTerritory);

    //         vassalTerritory.personalTreasury -= choice.cost;
    //         target.currentBuildings.Add(choice);
    //     }
    // }
}