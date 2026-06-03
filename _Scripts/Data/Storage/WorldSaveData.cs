using System.Collections.Generic;

[System.Serializable]
public class WorldSaveData
{
    // The "Atlas" of your world
    public List<CharacterSaveData> characters = new List<CharacterSaveData>();
    public List<TerritorySaveData> territories = new List<TerritorySaveData>();
    public List<RelationshipSaveData> relationships = new List<RelationshipSaveData>();
    
    // Global Metadata
    public int currentYear;
    public string playerCharacterID;
}