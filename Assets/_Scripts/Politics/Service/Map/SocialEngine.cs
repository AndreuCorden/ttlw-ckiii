using UnityEngine;
using System.Collections.Generic;

public class SocialEngine : MonoBehaviour
{
    public CharacterGenerator charGen; // Reference to your factory
    public Title playerTerritory;

    public void PopulateWorld(List<Title> kingdoms, Title playerTitle)
    {
        playerTerritory = playerTitle;
        foreach (Title kingdom in kingdoms)
        {
            CharacterData king = playerTitle.holder;
            if (playerTitle != null && playerTitle != kingdom)
            {
                // 1. Create the King
                king = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
                kingdom.holder = king;
                king.role = CharacterRole.Ruler;
                king.heldTitles.Add(kingdom);
            }


            // 3. Move down to Provinces
            foreach (Title province in kingdom.vassals)
            {
                ProcessTerritory(province, king, 0.3f); // 30% chance to be a relative
            }

        }
    }

    void ProcessTerritory(Title territory, CharacterData liege, float relativeChance)
    {
        CharacterData ruler;
        bool isRelative = Random.value < relativeChance;

        if (playerTerritory != null && playerTerritory == territory)
        {
            ruler = playerTerritory.holder;
        }
        else if (isRelative)
        {
            ruler = charGen.GenerateFamilyCharacter(CharacterRole.Ruler, liege);
            ruler.heldTitles.Add(territory);
        }
        else
        {
            ruler = charGen.GenerateRandomLeader(" of " + charGen.namePool[Random.Range(0, charGen.namePool.Length)]);
            ruler.family.familyColor = new Color(Random.value, Random.value, Random.value);
            ruler.heldTitles.Add(territory);
        }

        // Create the Ruler for this level (Duke, Count, or Mayor)
        territory.holder = ruler;
        ruler.role = CharacterRole.Ruler;
        RelationshipManager.Instance.CreateRelationship(liege,ruler,FeudalStatus.Liege);

        // Keep going down the hierarchy
        foreach (Title sub in territory.vassals)
        {
            // Further down, the chance of being a relative of the KING decreases
            ProcessTerritory(sub, ruler, 0.2f);
        }
    }
}