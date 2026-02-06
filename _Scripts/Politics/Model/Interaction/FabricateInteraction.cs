using UnityEngine;

public class FabricateInteraction : CharacterInteraction
{
    public FabricateInteraction()
    {
        this.interactionName = "Fabricate lie.";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        return receiver.influence < (sender.influence + Random.Range(-10, 11));
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        ChangeCharacterRespect(receiver);
    }

    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        ChangeCharacterRespect(sender);
    }

    public void ChangeCharacterRespect(CharacterData characterData)
    {
        foreach (Title title in characterData.heldTitles)
        {
            RelationshipManager.Instance.ChangeOpinion(characterData, title.liege.holder, -10);
            RelationshipManager.Instance.ChangeTrust(characterData, title.liege.holder, -5);
            foreach (Title vassal in title.vassals)
            {
                RelationshipManager.Instance.ChangeOpinion(characterData, vassal.holder, -10);
                RelationshipManager.Instance.ChangeTrust(characterData, vassal.holder, -5);
                RelationshipManager.Instance.ChangeLoyalty(characterData, vassal.holder, -5);
            }
        }
    }
}