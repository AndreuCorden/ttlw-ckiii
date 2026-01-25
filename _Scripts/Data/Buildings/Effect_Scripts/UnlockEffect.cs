using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Buildings/Effects/Unlock")]
public class UnlockEffect : BuildingEffect
{
    public List<UnitType> units;
    public override string GetEffectDescription() => "Unlocks: " + string.Join(", ", units.Select(u => u.typeName));

    public List<UnitType> GetAllUnlockedUnits(Territory t)
    {
        List<UnitType> allUnits = new List<UnitType>();

        foreach (BuildingData bld in t.currentBuildings)
        {
            BuildingData check = bld;
            while (check != null)
            {
                // Find the UnlockEffect in this specific level
                var unlockEffect = check.effects.OfType<UnlockEffect>().FirstOrDefault();
                if (unlockEffect != null)
                {
                    foreach (var unit in unlockEffect.units)
                    {
                        if (!allUnits.Contains(unit)) allUnits.Add(unit);
                    }
                }
                // Move to previous level to see what it unlocked
                check = check.previousLevel;
            }
        }
        return allUnits;
    }
}