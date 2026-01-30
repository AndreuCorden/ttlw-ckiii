using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            relationship.charA.fear = 0;
            relationship.charA.opinion = 0;
            relationship.charA.trust = 0;
            relationship.charB.fear = 0;
            relationship.charB.opinion = 0;
            relationship.charB.trust = 0;
            relationshipDatabase.Add(key, relationship);
        }
        return relationshipDatabase[key];
    }

    public void ChangeOpinion(CharacterData actor, CharacterData target, int amount)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            rel.charA.opinion = Mathf.Clamp(rel.charA.opinion + amount, -100, 100);
        }
        else
        {
            rel.charB.opinion = Mathf.Clamp(rel.charB.opinion + amount, -100, 100);
        }
    }

    public void ChangeTrust(CharacterData actor, CharacterData target, int amount)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            rel.charA.trust = Mathf.Clamp(rel.charA.opinion + amount, -100, 100);
        }
        else
        {
            rel.charB.trust = Mathf.Clamp(rel.charB.opinion + amount, -100, 100);
        }
    }

    public void ChangeFear(CharacterData actor, CharacterData target, int amount)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            rel.charA.fear = Mathf.Clamp(rel.charA.opinion + amount, -100, 100);
        }
        else
        {
            rel.charB.fear = Mathf.Clamp(rel.charB.opinion + amount, -100, 100);
        }
    }

    public int GetOpinion(CharacterData actor, CharacterData target)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            return rel.charB.opinion;
        }
        else
        {
            return rel.charA.opinion;
        }
    }

    public int GetTrust(CharacterData actor, CharacterData target)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            return rel.charB.trust;
        }
        else
        {
            return rel.charA.trust;
        }
    }

    public int GetFear(CharacterData actor, CharacterData target)
    {
        Relationship rel = GetRelationship(actor, target);
        if (actor.GetInstanceID() < target.GetInstanceID())
        {
            return rel.charB.fear;
        }
        else
        {
            return rel.charA.fear;
        }
    }
}