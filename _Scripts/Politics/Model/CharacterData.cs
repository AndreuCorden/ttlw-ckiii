using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Social/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;

    [Header("Personality Traits")]
    public int prowess; // Combat skill
    public int age;
    public int influence;

    [Header("Dynamics")]
    public List<Trait> traits = new List<Trait>();
    public Trait religion;
    public Trait philosophy;

    [Header("Titles Held")]
    public List<Title> heldTitles = new List<Title>();

    [Header("Family Links")]
    public Family family; // You'll need to create the Family class below
    public CharacterData father;
    public CharacterData mother;
    public CharacterData spouse;
    public List<CharacterData> children = new List<CharacterData>();
    public List<CharacterData> siblings = new List<CharacterData>();
    public List<CharacterData> retinue = new List<CharacterData>();

    public CharacterRole role; // Add an Enum for: Ruler, General, Priest, etc.

    [Header("Social Network")]
    public List<CharacterData> knownCharacters = new List<CharacterData>();

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

    public bool CanInteractWithPlayer(CharacterData player)
    {
        // 1. Are they in the same family?
        if (family == player.family) return true;

        // 2. Are they in a Feudal relationship?
        if (IsVassalOf(player) || IsLiegeOf(player)) return true;

        // 3. Are they in the same court? (Retinue/Council)
        if (player.retinue.Contains(this)) return true;

        // 4. Have they been introduced?
        if (knownCharacters.Contains(player)) return true;

        return false;
    }

    public bool IsVassalOf(CharacterData player)
    {
        bool isVassalOf = false;
        foreach (Title title in heldTitles)
        {
            foreach (Title vassal in title.vassals)
            {
                if (player == vassal)
                {
                    isVassalOf = true;
                }
            }
        }
        return isVassalOf;
    }

    public bool IsLiegeOf(CharacterData player)
    {
        bool isLiegeOf = false;
        foreach (Title title in heldTitles)
        {
            if (player == title.liege)
            {
                isLiegeOf = true;
            }
        }
        return isLiegeOf;
    }

    public List<ArmyUnitData> army = new List<ArmyUnitData>();
}

public enum CharacterRole { Ruler, Knight, Priest, Merchant, Diplomat, Courtier, Family }