using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Building", menuName = "Politics/Building/GenericBuilding")]
public class BuildingData : ScriptableObject, IDescribable
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public int level;
    public BuildingData nextUpgrade; // Link to the next level
    public BuildingData previousLevel; // Link to the previous level

    [Header("Building Effects")]
    public List<BuildingEffect> effects = new List<BuildingEffect>();

    public string GetName()
    {
        return buildingName;
    }

    public string GetDescription()
    {
        string desc = $"Level: {level}\nCost: {cost} Gold\n";
        foreach (var effect in effects)
        {
            desc += effect.GetEffectDescription() + "\n";
        }
        return desc;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetGoldPerTurn()
    {
        int totalGold = 0;
        if (effects.Find(e => e is GoldEffect) is GoldEffect goldEffect)
        {
            totalGold += goldEffect.goldAmount;
        }
        return totalGold;
    }
}