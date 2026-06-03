using System.Collections.Generic;

[System.Serializable]
public class CharacterSaveData {
    public string charID; // Use characterName + " " + familyName
    public string name;
    public int prowess;
    public int age;
    public int influence;
    public float treasury;
    public List<string> traitNames;
    public string spouseID;
    public List<ArmyUnitSaveData> army;
}

[System.Serializable]
public class RelationshipSaveData
{
    public string charAID;
    public string charBID;
    public int opinionAtoB;
    public int opinionBtoA;
    public int trustA;
    public int trustB;
    public int loyalty;
    public bool isAtWar;
    public bool isAllied;
    public FeudalStatus statusA;
    public FeudalStatus statusB;
}

[System.Serializable]
public class TerritorySaveData {
    public string territoryName;
    public int population;
    public string ownerTitleName; // To reconnect the owner
    public List<string> currentBuildingNames;
}

[System.Serializable]
public class ArmyUnitSaveData
{
    public string unitTypeName; // Save "Archers" instead of the ScriptableObject
    public int currentSoldierCount;
    public int maxSoldierCount;
}