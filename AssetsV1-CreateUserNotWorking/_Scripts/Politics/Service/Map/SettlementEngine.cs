using UnityEngine;
using System.Collections.Generic;

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
                // If the Kingdom capital is inside this province, it's ALREADY the capital
                if (IsChildOf(province, kingdomCapital)) continue;

                Territory provinceCapital = GetRandomTownRecursive(province);
                MarkAsCapital(provinceCapital, province);

                // 4. For every other County, pick its own capital
                foreach (Territory county in province.subTerritories)
                {
                    if (IsChildOf(county, kingdomCapital) || IsChildOf(county, provinceCapital)) continue;

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
                t.population = (int) (capitalBonus * Random.Range(50, 500));
                break;
            case SettlementSize.Village:
                t.population = (int) (capitalBonus * Random.Range(500, 1000));
                break;
            case SettlementSize.BigVillage:
                t.population = (int) (capitalBonus * Random.Range(1000, 3000));
                break;
            case SettlementSize.SmallTown:
                t.population = (int) (capitalBonus * Random.Range(3000, 5000));
                break;
            case SettlementSize.MarketTown:
                t.population = (int) (capitalBonus * Random.Range(5000, 7000));
                break;
            case SettlementSize.Borough:
                t.population = (int) (capitalBonus * Random.Range(7000, 10000));
                break;
            case SettlementSize.BigTown:
                t.population = (int) (capitalBonus * Random.Range(10000, 15000));
                break;
            case SettlementSize.SmallCity:
                t.population = (int) (capitalBonus * Random.Range(15000, 50000));
                break;
            case SettlementSize.City:
                t.population = (int) (capitalBonus * Random.Range(50000, 200000));
                break;
            default:
                t.population = 0;
                break;
        }
    }
}