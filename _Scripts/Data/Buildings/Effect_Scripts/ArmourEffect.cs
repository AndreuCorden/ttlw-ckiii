using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Effects/Armour")]
public class ArmourEffect : BuildingEffect, IMilitaryEffect {
    public int armorAmount;
    public override string GetEffectDescription() => $"+{armorAmount} Armor";
    public int GetBuffAmount() => armorAmount;

    public void ApplyMilitaryBonus(ArmyUnitData unit)
    {
        // Apply the armor bonus to the unit
        unit.unitType.defensePower += armorAmount;
    }
}


