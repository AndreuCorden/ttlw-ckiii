using UnityEngine;

public class RecruitmentManager : MonoBehaviour
{
    public void RecruitToRetinue(CharacterData character, UnitType unitType, Territory location)
    {
        // 1. Check if the character is at a territory they govern or own
        // 2. Check if that location has the required building
        if (CanRecruitUnit(unitType, location))
        {
            ArmyUnitData newUnit = new ArmyUnitData(unitType, 50); // Squad of 50
            character.army.Add(newUnit);
            Debug.Log($"{character.characterName} recruited {unitType.typeName} at {location.territoryName}");
        }
    }

    bool CanRecruitUnit(UnitType type, Territory t)
    {
        // Example: If unit is 'Men-at-Arms', check for 'Barracks' in constructedBuildings
        // This is where you'd implement your City/Town + Building logic
        return true;
    }

    public bool IsUnitUnlocked(UnitType unit, Territory location)
    {
        // Map units to their required buildings
        switch (unit.typeName)
        {
            case "Men-at-Arms":
                return location.HasBuilding("Barracks");
            case "Archers":
                return location.HasBuilding("Archery Range");
            case "Town Militia":
                return location.HasBuilding("Town Hall");
            default:
                return true; // Basic levies might not need buildings
        }
    }
}

//Unit Type,Recruitment Source,Role
//Pikemen / Halberdiers,City + Militia Guild,"Anti-Cavalry. High defense, slow move speed."
//Hobelars,Big Town + Stables,"Light Cavalry. High move speed, used for scouting/skirmishing."
//Longbowmen,Big Town + Archery Range,"High range, low defense. English/Welsh style specialists."
//Crossbowmen,City + Range,"High attack (piercing), slow reload speed."
//Knights,Character's Social Status,"The ""Tanks."" Highest prowess and defense, but very expensive."