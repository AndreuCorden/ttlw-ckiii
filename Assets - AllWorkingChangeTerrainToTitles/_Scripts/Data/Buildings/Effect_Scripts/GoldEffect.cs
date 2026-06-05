using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Effects/Gold")]
public class GoldEffect : BuildingEffect, ICivicEffect {
    public int goldAmount;
    public override string GetEffectDescription() => $"+{goldAmount} Gold per turn";

    public void ApplyCivicBonus(Territory territory)
    {
        // Apply the civic bonus to the territory
        // This is a placeholder for future implementation
    }
}