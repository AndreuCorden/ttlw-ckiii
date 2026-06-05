using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownHall", menuName = "Politics/Building/Town Hall")]
public class TownHallData : BuildingData 
{
    [Header("Town Hall Specifics")]
    public List<BuildingData> unlockedBuildings;
    public TownHallData prev;
    public TownHallData next;
}