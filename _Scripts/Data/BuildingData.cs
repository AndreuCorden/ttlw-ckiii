using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Politics/Building")]
public class BuildingData : ScriptableObject, IDescribable
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public int level;
    public BuildingData nextUpgrade; // Link to the next level

    [Header("Stats")]
    public int goldGeneration;
    public int recruitmentBonus;

    public string GetName()
    {
        return buildingName;
    }

    public string GetDescription()
    {
        return $"Level: {level}\n" +
               $"Cost: {cost} Gold\n" +
               $"Gold Generation: {goldGeneration} per turn\n" +
               $"Recruitment Bonus: {recruitmentBonus}%";
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}