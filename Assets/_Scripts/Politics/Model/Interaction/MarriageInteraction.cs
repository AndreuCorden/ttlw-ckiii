using UnityEngine;

// --- MARRIAGE PROPOSAL ---
public class MarriageInteraction : CharacterInteraction
{
    public CharacterData proposedSpouse;

    public MarriageInteraction()
    {
        this.interactionName = "Marriage";
    }

    public override bool AI_Evaluate(CharacterData receiver)
    {
        // AI Logic: Check if they like the sender and if the family prestige is high
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender, receiver);
        int score = rel.charA.opinion; // simplified
        return score > 40;
    }

    public override void Execute(CharacterData receiver)
    {
        proposedSpouse.spouse = receiver;
        proposedSpouse.family.alliedFamilies.Add(receiver.family);
        receiver.spouse = proposedSpouse;
        receiver.family.alliedFamilies.Add(proposedSpouse.family);
    }

    public override void Decline(CharacterData receiver) => Debug.Log("Marriage Declined");
}