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

    public void GiveGift(CharacterData target)
    {
        int baseOpinion = 10;

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
    }

    public void SendInsult(CharacterData characterToDisplay)
    {
    }

    public void StartContactMission(CharacterData sender, CharacterData target)
    {
        // Costs Influence or Gold to send a diplomat
        if (sender.influence >= 20)
        {
            sender.influence -= 20;
            // In a real game, this might take 10 days to complete
            CompleteContact(sender, target);
        }
    }

    private void CompleteContact(CharacterData sender, CharacterData target)
    {
        if (!target.knownCharacters.Contains(sender))
            target.knownCharacters.Add(sender);
            
        if (!sender.knownCharacters.Contains(target))
            sender.knownCharacters.Add(target);

        Debug.Log($"Connections formed between {sender.characterName} and {target.characterName}");
    }
}