[System.Serializable] // This allows it to show up in the Inspector
public class ArmyUnitData
{
    public UnitType unitType;
    public int currentSoldierCount;
    public int maxSoldierCount;

    public ArmyUnitData(UnitType type, int count)
    {
        unitType = type;
        currentSoldierCount = count;
        maxSoldierCount = count;
    }
}