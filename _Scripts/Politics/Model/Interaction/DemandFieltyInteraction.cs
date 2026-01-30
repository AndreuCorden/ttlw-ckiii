using UnityEngine;

public class DemandFiletyInteraction : CharacterInteraction
{
    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        return (sender.prowess > 80 && receiver.prowess < 70) || (RelationshipManager.Instance.GetOpinion(sender,receiver) > 30);
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        Title title = sender.GetClosestTitle(receiver.GetHighestTitle());
        title.vassals.Add(receiver.GetHighestTitle());
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender,receiver,-20);
        RelationshipManager.Instance.ChangeOpinion(receiver,sender,-20);
        RelationshipManager.Instance.ChangeTrust(sender,receiver,-20);
        RelationshipManager.Instance.ChangeTrust(receiver,sender,-20);
        if(sender.prowess > (receiver.prowess + 30))
        {
            RelationshipManager.Instance.ChangeFear(sender,receiver,-20);
        }
    }
}