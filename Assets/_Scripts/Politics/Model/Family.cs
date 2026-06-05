using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewFamily", menuName = "Social/Family")]
public class Family : ScriptableObject
{
    public string familyName;
    public Color familyColor; // Often similar to the founder's color
    public List<CharacterData> members = new List<CharacterData>();
    public List<Family> alliedFamilies = new List<Family>();
    public CharacterData headOfFamily;

    public float reputation; // Overall family reputation

    public float influence;  // Political weight (separate from gold)

    public void Initialize(string name, CharacterData founder)
    {
        familyName = name;
        headOfFamily = founder;
        if (!members.Contains(founder)) members.Add(founder);
    }
}