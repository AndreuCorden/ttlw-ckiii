using UnityEngine;

public class FosterChildInteraction : CharacterInteraction
{
    public CharacterData child;

    public FosterChildInteraction()
    {
        this.interactionName = "Foster Child";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        bool goodChild = false;
        if (child.prowess > receiver.prowess * 0.5 || child.influence > receiver.influence * 0.5)
        {
            goodChild = true;
        }
        bool goodVassal = false;
        if (RelationshipManager.Instance.GetLoyalty(sender, receiver) < 20 || RelationshipManager.Instance.GetOpinion(sender, receiver) >= 75)
        {
            goodVassal = true;
        }
        return goodChild && goodVassal;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        receiver.fostered.Add(child);
        RelationshipManager.Instance.ChangeLoyalty(sender, receiver, 20);
    }

    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender, receiver, -10);
        RelationshipManager.Instance.ChangeOpinion(receiver, sender, -10);
        RelationshipManager.Instance.ChangeLoyalty(sender, receiver, -10);
    }
}