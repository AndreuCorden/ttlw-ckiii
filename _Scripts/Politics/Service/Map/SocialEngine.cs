using UnityEngine;
using System.Collections.Generic;

public class SocialEngine : MonoBehaviour
{
    public CharacterGenerator charGen; // Reference to your factory

    public void PopulateWorld(List<Territory> kingdoms, Territory playerCapital)
    {
        foreach (Territory kingdom in kingdoms)
        {

            // Check if the player is in this Kingdom's tree
            if (IsPlayerInHierarchy(kingdom, playerCapital))
            {
                // Handle kingdom where player exists
                ProcessKingdomWithPlayer(kingdom, playerCapital);
            }
            else
            {
                // 1. Create the King
                CharacterData king = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
                // 2. Create the Royal Family
                king.family = CreateNewFamily(king);
                kingdom.leader = king;
                king.role = CharacterRole.Ruler;
                king.governedTerritory = kingdom;
                kingdom.ownerKingdom = new Kingdom();

                // 3. Move down to Provinces
                foreach (Territory province in kingdom.subTerritories)
                {
                    ProcessTerritory(province, king, 0.3f); // 30% chance to be a relative
                }
            }

        }
    }

    void ProcessTerritory(Territory territory, CharacterData liege, float relativeChance)
    {
        CharacterData ruler;
        bool isRelative = Random.value < relativeChance;

        if (isRelative)
        {
            ruler = charGen.GenerateFamilyCharacter(CharacterRole.Ruler, liege);
        }
        else
        {
            ruler = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
            float h, s, v;
            Color.RGBToHSV(liege.factionColor, out h, out s, out v);
            s = Mathf.Clamp(s + Random.Range(-0.25f, 0.25f), 0.3f, 1f);
            v = Mathf.Clamp(v + Random.Range(-0.25f, 0.25f), 0.4f, 1f);
            ruler.factionColor = Color.HSVToRGB(h, s, v);
            ruler.family = CreateNewFamily(ruler);
        }

        // Create the Ruler for this level (Duke, Count, or Mayor)
        territory.leader = ruler;
        ruler.governedTerritory = territory;

        // Form the Feudal Bond
        ruler.liege = liege;
        liege.vassals.Add(ruler);

        // Keep going down the hierarchy
        foreach (Territory sub in territory.subTerritories)
        {
            // Further down, the chance of being a relative of the KING decreases
            ProcessTerritory(sub, ruler, 0.2f);
        }
    }

    Family CreateNewFamily(CharacterData founder)
    {
        string surname = charGen.namePool[Random.Range(0, charGen.namePool.Length)];
        return new Family(surname, founder);
    }

    private bool IsPlayerInHierarchy(Territory root, Territory playerCapital)
    {
        if (playerCapital == null) return false;

        // We climb UP from the player's capital to see if we hit the root (Kingdom)
        Territory current = playerCapital;
        while (current != null)
        {
            if (current == root) return true;
            current = current.parentTerritory;
        }
        return false;
    }

    private void ProcessKingdomWithPlayer(Territory kingdom, Territory playerCapital)
    {
        // 1. Check if the player IS the King (if they clicked the Kingdom container)
        if (kingdom.leader == null)
        {
            CharacterData king = charGen.GenerateRandomLeader(" of " + kingdom.territoryName);
            king.family = CreateNewFamily(king);
            kingdom.leader = king;
            king.governedTerritory = kingdom;
        }

        // Initialize Kingdom Data
        if (kingdom.ownerKingdom == null) kingdom.ownerKingdom = new Kingdom();

        // 2. Recurse down
        foreach (Territory province in kingdom.subTerritories)
        {
            ProcessTerritoryWithPlayerCheck(province, kingdom.leader, 0.3f, playerCapital);
        }
    }

    private void ProcessTerritoryWithPlayerCheck(Territory territory, CharacterData liege, float relativeChance, Territory playerCapital)
    {
        // If this IS the player's territory, we DON'T generate a leader.
        // We use the one already assigned in PlayerManager.
        if (territory == playerCapital)
        {
            Debug.Log($"SocialEngine: Found player at {territory.territoryName}. Linking to liege {liege.characterName}");

            // Link the player to the feudal hierarchy
            territory.leader.liege = liege;
            liege.vassals.Add(territory.leader);
        }
        else
        {
            // Otherwise, run your normal logic
            ProcessTerritory(territory, liege, relativeChance);
            return; // ProcessTerritory already handles sub-territories
        }

        // Continue down the tree even if we found the player, 
        // in case the player rules a County and needs Town vassals.
        foreach (Territory sub in territory.subTerritories)
        {
            ProcessTerritoryWithPlayerCheck(sub, territory.leader, 0.2f, playerCapital);
        }
    }
}