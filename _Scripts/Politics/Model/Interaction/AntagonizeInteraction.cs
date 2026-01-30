using UnityEngine;

public class AntagonizeInteraction : CharacterInteraction
{
    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        bool one = false;
        bool two = false;
        bool three = false;
        if ((sender.influence > receiver.influence) && (sender.prowess > receiver.prowess)) one = true;
        if ((sender.influence > receiver.influence) && (sender.prowess > receiver.prowess - 15)) two = true;
        if ((sender.influence > receiver.influence -15) && (sender.prowess > receiver.prowess)) three = true;
        return one || two || three;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender,receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            rel.charB.opinion -= 20;
            rel.charB.fear += 30;
            rel.charB.trust -= 20;
        }
        else
        {
            rel.charA.opinion -= 20;
            rel.charA.fear += 30;
            rel.charA.trust -= 20;
        }
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender,receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            rel.charB.opinion -= 30;
            rel.charB.trust -= 30;
        }
        else
        {
            rel.charA.opinion -= 30;
            rel.charA.trust -= 30;
        }
    }
}