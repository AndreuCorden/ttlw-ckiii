using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyManager
{
    // Change 'object' to 'DiplomacyManager'
    private static DiplomacyManager _instance;
    public static DiplomacyManager Instance
    {
        get
        {
            if (_instance == null) _instance = new DiplomacyManager();
            return _instance;
        }
    }

    // Constructor is private so no one else can 'new' it up
    private DiplomacyManager() { }

    public void ProposeAlliance(CharacterData target)
    {
        // Get the opinion from the Global Manager
        int opinion = GlobalGameManager.Instance.GetOpinion(target);

        if (opinion > 50 && !target.isAlliedWithPlayer)
        {
            target.isAlliedWithPlayer = true;
            Debug.Log("Alliance Formed!");
        }
        else
        {
            Debug.Log("They refused the alliance.");
        }
    }

    public void GiveGift(CharacterData target)
    {
        int baseOpinion = 10;
        int loyaltyBonus = 10;

        // Use Enums for clean logic
        foreach (Trait t in target.traits)
        {
            if (t.traitType == TraitEnum.Greedy) baseOpinion += 10;
            if (t.traitType == TraitEnum.Ambitious) baseOpinion -= 5;
        }

        if (target.religion != null && target.religion.religionType == ReligionEnum.SunWorship)
        {
            baseOpinion += 5; // Religious bonus
        }

        target.loyalty += loyaltyBonus;
        if (target.loyalty > 100) target.loyalty = 100;

        GlobalGameManager.Instance.ChangeOpinion(target, baseOpinion);
    }

    public void SendInsult(CharacterData characterToDisplay)
    {
        characterToDisplay.loyalty -= 20;
        if (characterToDisplay.loyalty < 0) characterToDisplay.loyalty = 0;
    }
}