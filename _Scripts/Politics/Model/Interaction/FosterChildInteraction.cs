using UnityEngine;

public class FosterChildInteraction : CharacterInteraction
{
    CharacterData child;
    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        bool goodChild = false;
        if (child.prowess > receiver.prowess*0.5 || child.influence > receiver.influence*0.5)
        {
            goodChild = true;
        }
        bool goodVassal = false;
        foreach (Title title in receiver.heldTitles)
        {
            foreach (Title title1 in title.vassals)
            {
                if (title1.holder == sender)
                {
                    if (title1.loyaltyToLiege < 20 || RelationshipManager.Instance.GetOpinion(sender,receiver) >= 75)
                    {
                        goodVassal = true;
                    }
                }
            }
        }
        return goodChild && goodVassal;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        receiver.fostered.Add(child);
        foreach (Title title in receiver.heldTitles)
        {
            foreach (Title title1 in title.vassals)
            {
                if (title1.holder == sender)
                {
                    title1.loyaltyToLiege += 20;
                }
            }
        }
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender,receiver,-10);
        RelationshipManager.Instance.ChangeOpinion(receiver,sender,-10);
        foreach (Title title in receiver.heldTitles)
        {
            foreach (Title title1 in title.vassals)
            {
                if (title1.holder == sender)
                {
                    title1.loyaltyToLiege -= 10;
                }
            }
        }
    }
}