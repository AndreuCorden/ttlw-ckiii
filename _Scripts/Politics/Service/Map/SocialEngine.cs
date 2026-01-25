using UnityEngine;
using System.Collections.Generic;

public class SocialEngine : MonoBehaviour
{
    public CharacterGenerator charGen; // Reference to your factory

    private Territory playerTerritory;

    public void PopulateWorld(List<Territory> kingdoms, Territory playerCapital)
    {
        playerTerritory = playerCapital;
        foreach (Territory kingdom in kingdoms)
        {
            CharacterData king = playerCapital.leader;
            if (playerTerritory != null && playerTerritory != kingdom)
            {
                // 1. Create the King
                king = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
                kingdom.leader = king;
                king.role = CharacterRole.Ruler;
                king.governedTerritory = kingdom;
                kingdom.ownerKingdom = new Kingdom();
            }


            // 3. Move down to Provinces
            foreach (Territory province in kingdom.subTerritories)
            {
                ProcessTerritory(province, king, 0.3f); // 30% chance to be a relative
            }

        }
    }

    void ProcessTerritory(Territory territory, CharacterData liege, float relativeChance)
    {
        CharacterData ruler;
        bool isRelative = Random.value < relativeChance;

        if (playerTerritory != null && playerTerritory == territory)
        {
            ruler = playerTerritory.leader;
        }
        else if (isRelative)
        {
            ruler = charGen.GenerateFamilyCharacter(CharacterRole.Ruler, liege);
        }
        else
        {
            ruler = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
            ruler.family.familyColor = new Color(Random.value, Random.value, Random.value);
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
}