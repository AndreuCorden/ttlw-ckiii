using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using System.Linq;


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
    public List<CharacterData> fostered = new List<CharacterData>();
    public List<CharacterInteraction> pendingInteractions = new List<CharacterInteraction>();

    public CharacterRole role; // Add an Enum for: Ruler, General, Priest, etc.

    [Header("Social Network")]
    public List<CharacterData> knownCharacters = new List<CharacterData>();

    // The "Main Boss" - usually the one who holds your highest-tier title's liege
    public CharacterData GetPrimaryLiege()
    {
        if (heldTitles.Count == 0) return null;

        // Logic: Find the highest rank title (King > Duke > Count)
        // and return that title's liege.
        Title highestTitle = null;
        foreach (Title title in heldTitles)
        {
            TitleRank titleRank = title.liege.holder.GetHighestRank();
            if (highestTitle == null || highestTitle.holder.GetHighestRank() < title.holder.GetHighestRank())
            {
                highestTitle = title;
            }
        }
        return highestTitle.liege?.holder;
    }

    public List<CharacterData> GetAllLieges()
    {
        List<CharacterData> allLieges = new List<CharacterData>();
        foreach (Title t in heldTitles)
        {
            if (t.liege != null && t.liege.holder != null)
            {
                if (!allLieges.Contains(t.liege.holder))
                    allLieges.Add(t.liege.holder);
            }
        }
        return allLieges;
    }

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

    public bool CanInteractWithPlayer(CharacterData target)
    {
        // 1. Are they in the same family?
        if (family == target.family) return true;

        // 2. Are they in a Feudal relationship?
        if (IsVassalOf(target) || IsLiegeOf(target)) return true;

        // 3. Are they in the same court? (Retinue/Council)
        if (target.retinue.Contains(this)) return true;

        // 4. Have they been introduced?
        if (knownCharacters.Contains(target)) return true;

        return false;
    }

    public bool IsVassalOf(CharacterData target)
    {
        bool isVassalOf = false;
        foreach (Title title in heldTitles)
        {
            foreach (Title vassal in title.vassals)
            {
                if (target == vassal.holder)
                {
                    isVassalOf = true;
                }
            }
        }
        return isVassalOf;
    }

    public bool IsLiegeOf(CharacterData target)
    {
        bool isLiegeOf = false;
        foreach (Title title in heldTitles)
        {
            if (title.liege.holder != null && target == title.liege.holder)
            {
                isLiegeOf = true;
            }
        }
        return isLiegeOf;
    }

    public float GetGold()
    {
        float totalGold = 0;
        foreach (Title title in heldTitles)
        {
            totalGold += title.personalTreasury;
        }
        return totalGold;
    }

    public TitleRank GetHighestRank()
    {
        TitleRank titleRank = 0;
        foreach (Title title in heldTitles)
        {
            if (title.rank > titleRank)
            {
                titleRank = title.rank;
            }
        }
        return titleRank;
    }

    public Title GetHighestTitle()
    {
        Title Htitle = null;
        foreach (Title title in heldTitles)
        {
            if (Htitle == null || title.rank > Htitle.rank)
            {
                Htitle = title;
            }
        }
        return Htitle;
    }

    public void ProcessTurn()
    {
        for (int i = pendingInteractions.Count - 1; i >= 0; i--)
        {
            var action = pendingInteractions[i];

            if (action.AI_Evaluate(this))
            {
                action.Execute(this);
                // Log for notification window
            }
            else
            {
                action.Decline(this);
            }

            pendingInteractions.RemoveAt(i);
        }
    }

    public bool CanOfferFealty(CharacterData potentialVassal, CharacterData potentialLiege)
    {
        // Compare the integer values of your TitleRank enum
        return (int)potentialLiege.GetHighestRank() > (int)potentialVassal.GetHighestRank();
    }

    public Title GetClosestTitle(Title vassalTitle)
    {
        Title closest = null;
        float minDistance = float.MaxValue;
        List<Title> orderedTitles = heldTitles.OrderBy(t => (int)t.rank).ToList();

        foreach (Title liegeTitle in orderedTitles)
        {
            // Only consider titles that are higher rank than the vassal's title
            if (((int)liegeTitle.rank == ((int)vassalTitle.rank + 1)) || ((int)liegeTitle.rank > (int)vassalTitle.rank && closest == null))
            {
                float dist = Vector2.Distance(vassalTitle.seatOfPower.transform.position, liegeTitle.seatOfPower.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = liegeTitle;
                }
            }
        }
        return closest;
    }

    public List<ArmyUnitData> army = new List<ArmyUnitData>();
}

public enum CharacterRole { Ruler, Knight, Priest, Merchant, Diplomat, Courtier, Family }
public enum SwayTopic { Opinion, Fear, Trust, Prowess, Influence }