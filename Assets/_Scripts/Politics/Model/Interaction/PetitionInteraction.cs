using UnityEngine;

public class PetitionInteraction : CharacterInteraction
{
    public float goldAmount;

    public PetitionInteraction()
    {
        this.interactionName = "Petition GOld";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
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
        return (opinion > 40) && (RelationshipManager.Instance.GetLoyalty(sender,receiver) >= 50) && (goldAmount < receiver.treasury * 0.1);
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        foreach ( Title title in sender.heldTitles)
        {
            if (title.liege.holder == receiver)
            {
                title.holder.treasury += goldAmount;
                title.liege.holder.treasury -= goldAmount;
                RelationshipManager.Instance.ChangeLoyalty(sender,receiver,10);
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