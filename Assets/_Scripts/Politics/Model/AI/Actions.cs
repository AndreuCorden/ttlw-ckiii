using UnityEngine;
using System.Collections.Generic;

public class Actions
{
    private static Actions _instance;
    public static Actions Instance
    {
        get
        {
            if (_instance == null) _instance = new Actions();
            return _instance;
        }
    }
    public enum AIGoal { ImproveStability, ExpandPower, ManageVassals, FormAlliances }

    public void DecideNextAction(CharacterData character)
    {
        // 1. Pick a goal based on their traits or situation
        AIGoal currentGoal = AIGoal.ManageVassals;
        if (character.influence < 20) currentGoal = AIGoal.ExpandPower;

        if (character.knownCharacters.Count < 5 || Random.value > 0.8f)
        {
            CharacterData stranger = FindPotentialContact(character);
            if (stranger != null)
            {
                SendEmissaryInteraction emissary = new SendEmissaryInteraction();
                emissary.sender = character;
                stranger.pendingInteractions.Add(emissary);
                return; // Used turn for discovery
            }
        }

        // 2. Get all known relationships to find a target
        List<Relationship> targets = RelationshipManager.Instance.GetAllRelationshipsFor(character);

        foreach (Relationship rel in targets)
        {
            CharacterData other = RelationshipManager.Instance.GetOtherCharacterInRelationship(rel, character);

            // 3. Score the interaction
            float score = EvaluateInteraction(character, other, rel, currentGoal);

            if (score > 75) // Threshold to actually send the interaction
            {
                SendInteraction(character, other, score, currentGoal);
                break; // Limit to one major action per turn
            }
        }
    }
    private float EvaluateInteraction(CharacterData character, CharacterData target, Relationship rel, AIGoal goal)
    {
        float score = 0;

        // Logic: If they hate me and I'm their liege, Sway is high priority
        if (rel.IsVassal(target) && RelationshipManager.Instance.GetOpinion(character, target) < 0)
        {
            score += 50;
            if (goal == AIGoal.ManageVassals) score += 30;
        }

        // Traits influence behavior
        if (character.HasTrait("Ambitious")) score += 20;

        return score;
    }

    public void SendInteraction(CharacterData character, CharacterData target, float score, AIGoal goal)
    {
        CharacterInteraction interaction = null;

        // Logic for choosing the SPECIFIC interaction class
        if (goal == AIGoal.ManageVassals)
        {
            if (score > 90 && character.influence > target.influence + 50)
                interaction = new RevokeInteraction();
            else if (score > 60)
                interaction = new SwayInteraction { chosenTopic = SwayTopic.Opinion };
            else
                interaction = new AntagonizeInteraction();
        }
        else if (goal == AIGoal.FormAlliances)
        {
            if (RelationshipManager.Instance.GetOpinion(character, target) > 50)
                interaction = new MarriageInteraction { proposedSpouse = character }; // Or a child
            else
                interaction = new SwayInteraction { chosenTopic = SwayTopic.Trust };
        }
        else if (goal == AIGoal.ExpandPower)
        {
            if (target.GetHighestRank() < character.GetHighestRank())
                interaction = new DemandFiletyInteraction();
            else
                interaction = new FabricateInteraction();
        }

        // Default Fallback
        if (interaction == null) interaction = new SwayInteraction { chosenTopic = SwayTopic.Influence };

        // Fill common data and send it
        interaction.sender = character;
        interaction.interactionName = interaction.GetType().Name;

        target.pendingInteractions.Add(interaction);
        Debug.Log($"{character.characterName} sent {interaction.interactionName} to {target.characterName}");
    }

    private CharacterData FindPotentialContact(CharacterData character)
    {
        // 1. Get everyone from the Registry
        var allPotentialPeople = CharacterRegistry.Instance.GetAllCharacters();

        // SAFETY: If the acting character has no land, they can't measure distance
        Title myTitle = character.GetHighestTitle();
        if (myTitle == null || myTitle.seatOfPower == null) return null;

        foreach (CharacterData stranger in allPotentialPeople.Values)
        {
            if (stranger == character) continue;
            if (character.KnowsCharacter(stranger)) continue;

            // SAFETY: Check the stranger's land too
            Title strangerTitle = stranger.GetHighestTitle();
            if (strangerTitle == null || strangerTitle.seatOfPower == null) continue;

            // 2. Now it is safe to calculate distance
            float distance = Vector2.Distance(
                myTitle.seatOfPower.transform.position,
                strangerTitle.seatOfPower.transform.position
            );

            if (distance < 500f)
            {
                return stranger;
            }
        }
        return null;
    }

    public void ExecuteInteractions(CharacterData character)
    {
        character.ExecuteInteractions();
    }
}