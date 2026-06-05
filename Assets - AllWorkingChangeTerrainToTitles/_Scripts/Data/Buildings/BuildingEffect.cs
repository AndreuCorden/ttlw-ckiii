
using UnityEngine;

public abstract class BuildingEffect : ScriptableObject 
{
    public abstract string GetEffectDescription();
    // You can add logic here like virtual void OnTurnPass(Territory t)
}