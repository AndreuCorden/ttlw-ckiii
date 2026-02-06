using UnityEngine;

public class SwayInteraction : CharacterInteraction
{
    public SwayTopic chosenTopic;

    public SwayInteraction()
    {
        this.interactionName = "Sway";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        bool decision = false;
        switch (chosenTopic)
        {
            case SwayTopic.Influence:
                if (sender.influence >= receiver.influence)
                {
                    decision = true;
                }
                break;
            case SwayTopic.Prowess:
                if (sender.prowess >= receiver.prowess)
                {
                    decision = true;
                }
                break;
            case SwayTopic.Fear:
                if (RelationshipManager.Instance.GetFear(sender, receiver) >= 70)
                {
                    decision = true;
                }
                break;
            case SwayTopic.Opinion:
                if (RelationshipManager.Instance.GetOpinion(sender, receiver) >= 80)
                {
                    decision = true;
                }
                break;
            case SwayTopic.Trust:
                if (RelationshipManager.Instance.GetTrust(sender, receiver) >= 70)
                {
                    decision = true;
                }
                break;
        }
        return decision;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender, receiver,10);
        RelationshipManager.Instance.ChangeTrust(sender, receiver,10);
    }

    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender, receiver,-10);
        RelationshipManager.Instance.ChangeTrust(sender, receiver,-10);
    }
}