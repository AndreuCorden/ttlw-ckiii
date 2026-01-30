using UnityEngine;

public class RevokeInteraction : CharacterInteraction
{
    // This is the logic the AI uses to decide Yes/No
    public override bool AI_Evaluate(CharacterData receiver)
    {
        return false;
    }

    // This is what actually happens if the answer is Yes
    public override void Execute(CharacterData receiver)
    {
        
    }
    
    // This is what happens if the answer is No
    public override void Decline(CharacterData receiver)
    {
        
    }
}