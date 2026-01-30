using UnityEngine;

public class GrantTitleInteraction : CharacterInteraction
{
    // Assuming that offeredTitle is already set up with the liege.
    public Title offeredTitle;
    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        bool accept = false;
        if (receiver.heldTitles == null && (sender.retinue.Contains(receiver) || sender.siblings.Contains(receiver) || sender.children.Contains(receiver))) accept = true;
        bool maybe = false;
        if (receiver.heldTitles != null)
        {
            foreach (Title title in receiver.heldTitles)
            {
                if (title.rank < offeredTitle.rank)
                {
                    Relationship rel = RelationshipManager.Instance.GetRelationship(sender, receiver);
                    if (sender.GetEntityId() < receiver.GetEntityId())
                    {
                        if (rel.charB.opinion > (receiver.prowess - 5))
                        {
                            maybe = true;
                        }
                    }
                    else
                    {
                        if (rel.charA.opinion > (receiver.prowess - 5))
                        {
                            maybe = true;
                        }
                    }
                }
            }
        }
        return accept || maybe;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        offeredTitle.holder = receiver;
        receiver.heldTitles.Add(offeredTitle);
        Decline(receiver);
    }

    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender, receiver);
        foreach (Title title in sender.heldTitles)
        {
            foreach (Title vassal in title.vassals)
            {
                if (vassal.holder == receiver)
                {
                    vassal.loyaltyToLiege += 20;
                }
            }
        }
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            rel.charB.opinion += 10;
        }
        else
        {
            rel.charA.opinion += 10;
        }
    }
}