using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SettlementEngine : MonoBehaviour
{
    public BuildingData townHallPrefab;

    public void AssignCapitals(List<Territory> kingdoms)
    {
        foreach (Territory kingdom in kingdoms)
        {
            // 1. Pick a random Town within the Kingdom to be the "Imperial Capital"
            Territory kingdomCapital = GetRandomTownRecursive(kingdom);

            // 2. Mark it and set it as a City
            MarkAsCapital(kingdomCapital, kingdom);
            kingdomCapital.ownerKingdom.capitalTerritory = kingdomCapital;

            // 3. For every other Province, pick its own capital
            foreach (Territory province in kingdom.subTerritories)
            {
                Territory provinceCapital;
                // If the Kingdom capital is inside this province, it's ALREADY the capital
                if (!IsChildOf(province, kingdomCapital))
                {
                    provinceCapital = GetRandomTownRecursive(province);
                    MarkAsCapital(provinceCapital, province);
                }
                else
                {
                    provinceCapital = kingdomCapital;
                }
                // 4. For every other County, pick its own capital
                foreach (Territory county in province.subTerritories)
                {
                    if (IsChildOf(county, provinceCapital)) continue;

                    Territory countyCapital = GetRandomTownRecursive(county);
                    MarkAsCapital(countyCapital, county);
                }
            }
        }
    }

    // 1. Update the signature to accept the 'level' or 'container'
    private void MarkAsCapital(Territory t, Territory container)
    {
        t.isCapital = true;

        // We check the type of the CONTAINER (Kingdom/Province/County) 
        // to decide how big the CAPITAL TILE should be.
        SetSizeCapitol(t, container);

        if (townHallPrefab != null)
        {
            t.currentBuildings.Add(townHallPrefab);
        }
        t.RefreshUI();
    }

    private Territory GetRandomTownRecursive(Territory container)
    {
        // 1. If we are already at the Town level, return this
        if (container.type == TerritoryType.Town) return container;

        // 2. If this container has no children, we can't find a town
        if (container.subTerritories == null || container.subTerritories.Count == 0)
        {
            Debug.LogWarning($"Container {container.name} has no sub-territories to pick a capital from!");
            return null;
        }

        // 3. Pick a random child branch
        int randomIndex = Random.Range(0, container.subTerritories.Count);
        Territory randomChild = container.subTerritories[randomIndex];

        // 4. Recurse down until we hit a Town
        return GetRandomTownRecursive(randomChild);
    }

    private bool IsChildOf(Territory parent, Territory child)
    {
        if (child == null || parent == null) return false;

        Territory current = child;

        // Climb the tree from the child upwards
        while (current != null)
        {
            if (current == parent) return true;
            current = current.parentTerritory;
        }

        return false;
    }

    public void AssignTerritorySizeAndPopulation(List<Territory> kingdoms)
    {
        foreach (Territory Kingdom in kingdoms)
        {
            foreach (Territory Province in Kingdom.subTerritories)
            {
                foreach (Territory County in Province.subTerritories)
                {
                    foreach (Territory Town in County.subTerritories)
                    {
                        if (!Town.isCapital)
                        {
                            SetSize(Town);
                        }
                        SetPopulation(Town);
                        SetBuildings(Town);
                    }
                    County.CalculatePopulation();
                }
                Province.CalculatePopulation();
            }
            Kingdom.CalculatePopulation();
        }
    }

    public void SetSize(Territory t)
    {
        switch (t.type)
        {
            case TerritoryType.County:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.SmallTown, (int)SettlementSize.MarketTown + 1);
                break;
            case TerritoryType.Province:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.Borough, (int)SettlementSize.BigTown + 1);
                break;
            case TerritoryType.Kingdom:
                // This ensures Kingdom capitals are SmallCity or City
                t.size = (SettlementSize)Random.Range((int)SettlementSize.SmallCity, (int)SettlementSize.City + 1);
                break;
            default:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.Hamlet, (int)SettlementSize.City + 1);
                break;
        }
    }

    public void SetSizeCapitol(Territory t, Territory container)
    {
        switch (container.type)
        {
            case TerritoryType.County:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.SmallTown, (int)SettlementSize.MarketTown + 1);
                break;
            case TerritoryType.Province:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.Borough, (int)SettlementSize.BigTown + 1);
                break;
            case TerritoryType.Kingdom:
                // This ensures Kingdom capitals are SmallCity or City
                t.size = (SettlementSize)Random.Range((int)SettlementSize.SmallCity, (int)SettlementSize.City + 1);
                break;
            default:
                t.size = (SettlementSize)Random.Range((int)SettlementSize.Hamlet, (int)SettlementSize.City + 1);
                break;
        }
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
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);
        List<Territory> towns = allTerritories.Where(t => t.type == TerritoryType.Town).ToList();

        foreach (Territory t in towns)
        {
            // A. Run growth first (since you want to show "Next Turn's" potential)
            t.population += Mathf.RoundToInt(t.population * 0.05f);

            // B. Calculate Gold based on THIS new population
            float goldGenerated = t.GetGoldPerTurn();
            goldGenerated += t.population / 100f;

            // C. Store it
            t.localWealth = goldGenerated;

            // Note: We don't add to treasury here yet because the game hasn't "started"
        }

        // D. Update the hierarchy so Counties/Kingdoms show the sum of these new numbers
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
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
        foreach (var t in provinces)
        {
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
        foreach (var t in kingdoms)
        {
            // Update localWealth for the UI/Summary
            t.localWealth = t.subTerritories.Sum(sub => sub.localWealth);
        }
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

    public void KingOrderBuild(Territory targetTown, BuildingData building, bool forceVassalToPay)
    {
        Kingdom king = targetTown.ownerKingdom;

        if (forceVassalToPay)
        {
            targetTown.parentTerritory.personalTreasury -= building.cost;
            // Logic to decrease relationship/increase tyranny here
        }
        else
        {
            king.treasury -= building.cost;
            // Logic to increase relationship (Generosity)
        }

        targetTown.currentBuildings.Add(building);
        targetTown.RefreshUI();
    }
}