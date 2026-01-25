using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Family
{
    public string familyName;
    public Color familyColor; // Often similar to the founder's color
    public List<CharacterData> members = new List<CharacterData>();
    public CharacterData headOfFamily;

    public float reputation; // Overall family reputation

    public float influence;  // Political weight (separate from gold)

    public Family(string name, CharacterData founder)
    {
        familyName = name;
        headOfFamily = founder;
        members.Add(founder);
        reputation = 50;
    }
}