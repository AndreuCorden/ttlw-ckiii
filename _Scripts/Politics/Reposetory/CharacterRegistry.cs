using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class CharacterRegistry : MonoBehaviour
{
    public static CharacterRegistry Instance;

    // This is our "Phonebook"
    private Dictionary<string, CharacterData> idToCharacter = new Dictionary<string, CharacterData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // IMPORTANT: We need to fill this dictionary!
        PopulateRegistry();
    }

    public void PopulateRegistry()
    {
        // This finds every CharacterData file currently loaded in memory
        CharacterData[] allChars = Resources.FindObjectsOfTypeAll<CharacterData>();
        foreach (var c in allChars)
        {
            // We use the InstanceID as the unique key
            string id = c.characterId;
            if (!idToCharacter.ContainsKey(id))
                idToCharacter.Add(id, c);
        }
    }

    public CharacterData GetCharacter(string id)
    {
        if (idToCharacter.TryGetValue(id, out CharacterData character))
            return character;
        
        return null;
    }

    public Dictionary<string, CharacterData> GetAllCharacters()
    {
        return idToCharacter;
    }
}