using UnityEngine;

public class SendEmissaryInteraction : CharacterInteraction
{

    public SendEmissaryInteraction()
    {
        this.interactionName = "Send Emissery";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        return (sender.GetHighestRank() >= (receiver.GetHighestRank() - 1)) || (sender.prowess > (receiver.prowess + 20) && (sender.influence > (receiver.influence + 20)));
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        sender.knownCharacters.Add(receiver);
        receiver.knownCharacters.Add(sender);
        RelationshipManager.Instance.GetRelationship(sender,receiver);
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        Debug.Log("Your emissary was not accepted.");
    }
}