using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Social/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;

    [Header("Personality Traits")]
    public int prowess; // Combat skill
    public int age;
    public int influence;

    [Header("Political Standings")]
    public int loyalty = 50;  // 0-100
    public int respect = 50;  // How much people like you
    public int dread = 0;     // How much people fear you

    [Header("Relationships")]
    public bool isAlliedWithPlayer;
    public int opinionOfPlayer = 0; // -100 to 100

    [Header("Dynamics")]
    public List<Trait> traits = new List<Trait>();
    public Trait religion;
    public Trait philosophy;

    [Header("Faction Details")]
    public Color factionColor = Color.white;

    [Header("Social Connections")]
    public CharacterData liege;
    public List<CharacterData> vassals = new List<CharacterData>();
    public List<CharacterData> knights = new List<CharacterData>();
    public CharacterData priest;
    public Family family; // You'll need to create the Family class below

    [Header("Family Links")]
    public CharacterData father;
    public CharacterData mother;
    public CharacterData spouse;
    public List<CharacterData> children = new List<CharacterData>();
    public List<CharacterData> siblings = new List<CharacterData>();

    public CharacterRole role; // Add an Enum for: Ruler, General, Priest, etc.
    
    public Territory governedTerritory;

    // A helper function to get the "Total" of a stat including traits
    public int GetTotalProwess()
    {
        int total = prowess;
        foreach (Trait t in traits)
        {
            //total += t.prowessMod;
        }
        return total;
    }

    public bool HasTrait(string nameToCheck)
    {
        foreach (Trait t in traits)
        {
            if (t.traitName == nameToCheck) return true;
        }
        return false;
    }

    public System.Collections.Generic.List<ArmyUnitData> army = new List<ArmyUnitData>();
}

public enum CharacterRole { Ruler, General, Priest, Merchant, Diplomat, Courtier, Family }