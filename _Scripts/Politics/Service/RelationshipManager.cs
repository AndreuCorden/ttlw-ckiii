using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance;

    // The key is a string: "CharacterAID_CharacterBID"
    private Dictionary<string, Relationship> relationshipDatabase = new Dictionary<string, Relationship>();

    void Awake() => Instance = this;

    private string GetKey(CharacterData a, CharacterData b)
    {
        // Sort IDs or names alphabetically so that (A, B) and (B, A) result in the same key
        return a.GetInstanceID() < b.GetInstanceID()
            ? $"{a.GetInstanceID()}_{b.GetInstanceID()}"
            : $"{b.GetInstanceID()}_{a.GetInstanceID()}";
    }

    public Relationship GetRelationship(CharacterData a, CharacterData b)
    {
        string key = GetKey(a, b);
        if (!relationshipDatabase.ContainsKey(key))
        {
            Relationship relationship = new Relationship();
            relationship.fear = 0;
            relationship.opinion = 50;
            relationship.trust = 50;
            relationshipDatabase.Add(key, relationship);
        }
        return relationshipDatabase[key];
    }

    public void ChangeOpinion(CharacterData actor, CharacterData target, int amount)
    {
        Relationship rel = GetRelationship(actor, target);
        rel.opinion = Mathf.Clamp(rel.opinion + amount, -100, 100);
    }
}