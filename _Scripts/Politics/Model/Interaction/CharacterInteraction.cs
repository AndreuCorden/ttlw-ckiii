using UnityEngine;

public enum InteractionType { Proposal, Demand, Gift, Hostile }

[System.Serializable]
public abstract class CharacterInteraction
{
    public CharacterData sender;
    public string interactionName;
    public string lastResultMessage;
    public InteractionType type;

    // This is the logic the AI uses to decide Yes/No
    public abstract bool AI_Evaluate(CharacterData receiver);

    // This is what actually happens if the answer is Yes
    public abstract void Execute(CharacterData receiver);
    
    // This is what happens if the answer is No
    public abstract void Decline(CharacterData receiver);
}