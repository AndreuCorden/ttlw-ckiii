using System.Collections.Generic;
using UnityEngine;

public class OfferOathInteraction : CharacterInteraction
{
    //The main issue here is deciding what kind of oath I'm giving. Looking for new territory, looking to give all of my previous territory (act of war probably).
    List<Title> offeredTitlesToVassilize;

    public OfferOathInteraction()
    {
        this.interactionName = "OfferOath";
    }

    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        int opinion;
        Relationship rel = RelationshipManager.Instance.GetRelationship(sender, receiver);
        if (sender.GetEntityId() < receiver.GetEntityId())
        {
            opinion = rel.charB.opinion;
        }
        else
        {
            opinion = rel.charA.opinion;
        }
        bool nl = HaveNoLiege();
        return (sender.prowess < receiver.prowess) && ((((opinion > 70 && !nl) || (opinion > 30 && nl)) && offeredTitlesToVassilize.Count > 0) || (opinion > 90 && offeredTitlesToVassilize.Count > 0));
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        if (HaveNoLiege())
        {
            foreach (Title title in offeredTitlesToVassilize)
            {
                Title newLiege = receiver.GetClosestTitle(title);
                newLiege.vassals.Add(title);
                title.liege = newLiege;
            }
        }
        else
        {
            foreach (Title title in offeredTitlesToVassilize)
            {
                Relationship rel1 = RelationshipManager.Instance.GetRelationship(sender, title.liege.holder);
                Relationship rel2 = RelationshipManager.Instance.GetRelationship(receiver, title.liege.holder);
                // Change opinions.
                Title newLiege = receiver.GetClosestTitle(title);
                newLiege.vassals.Add(title);
                title.liege = newLiege;
            }
        }
    }

    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        RelationshipManager.Instance.ChangeOpinion(sender, receiver, -10);
        RelationshipManager.Instance.ChangeOpinion(receiver, sender, 10);
        foreach (Title title in offeredTitlesToVassilize)
        {
            if (title.liege.holder != null)
            {
                RelationshipManager.Instance.ChangeOpinion(sender, title.liege.holder, -20);
            }
        }
    }

    private bool HaveNoLiege()
    {
        bool noLiege = true;
        foreach (Title title in offeredTitlesToVassilize)
        {
            if (title.liege != null)
            {
                noLiege = false;
            }
        }
        return noLiege;
    }
}