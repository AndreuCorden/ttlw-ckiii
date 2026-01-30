using UnityEngine;

public class PetitionInteraction : CharacterInteraction
{
    public float goldAmount;

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        int loyalty = 0;
        foreach ( Title title in sender.heldTitles)
        {
            if (title.liege.holder == receiver)
            {
                loyalty = title.liege.loyaltyToLiege;
            }
        }
        int opinion;
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender,receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            opinion = rel.charB.opinion;
        }
        else
        {
            opinion = rel.charA.opinion;
        }
        return (opinion > 40) && (loyalty >= 50) && (goldAmount < receiver.GetGold() * 0.1);
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        foreach ( Title title in sender.heldTitles)
        {
            if (title.liege.holder == receiver)
            {
                title.personalTreasury += goldAmount;
                title.liege.personalTreasury -= goldAmount;
                title.liege.loyaltyToLiege += 10;
            }
        }
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender,receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            rel.charB.opinion -= 5;
            rel.charA.opinion += 5;
        }
        else
        {
            rel.charB.opinion += 5;
            rel.charA.opinion -= 5;
        }
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender,receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            rel.charB.opinion -= 5;
            rel.charA.opinion -= 5;
        }
        else
        {
            rel.charB.opinion -= 5;
            rel.charA.opinion -= 5;
        }
    }
}