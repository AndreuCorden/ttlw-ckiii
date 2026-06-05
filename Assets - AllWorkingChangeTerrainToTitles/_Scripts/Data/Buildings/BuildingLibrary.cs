using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingLibrary", menuName = "Politics/Building Library")]
public class BuildingLibrary : ScriptableObject
{
    public List<BuildingData> townHallLevels; // Elements 0, 1, 2 = Lv 1, 2, 3
    public List<BuildingData> allPossibleBuildings; // List of Lv 1 versions of Farms, Markets, etc.
}