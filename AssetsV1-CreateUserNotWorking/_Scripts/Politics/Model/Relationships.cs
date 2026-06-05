using UnityEngine;

[System.Serializable]
public class Relationship
{
    public CharacterData characterA;
    public CharacterData characterB;
    public int opinionScore; 
    public bool isAllied;
    public bool hasTradeTreaty;
}