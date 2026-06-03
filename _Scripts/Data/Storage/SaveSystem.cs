using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour {
    
    public void SaveGame() {
        WorldSaveData masterSave = new WorldSaveData();

        // 1. Save Characters
        foreach(var charData in CharacterRegistry.Instance.GetAllCharacters().Values) {
            masterSave.characters.Add(new CharacterSaveData {
                charID = charData.characterName, // Should ideally be a unique string
                age = charData.age,
                treasury = charData.treasury
                // ... add other stats here
            });
        }

        // 2. Save Territories
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);
        foreach(var t in allTerritories) {
            masterSave.territories.Add(new TerritorySaveData {
                territoryName = t.territoryName,
                population = t.population,
                ownerTitleName = t.owner != null ? t.owner.titleName : ""
            });
        }

        // 3. Convert to JSON and Save to Disk
        string json = JsonUtility.ToJson(masterSave, true);
        string path = Path.Combine(Application.persistentDataPath, "SaveGame01.json");
        File.WriteAllText(path, json);

        Debug.Log("Game Saved to: " + path);
    }
}