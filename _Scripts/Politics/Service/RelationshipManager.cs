using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance;

    // The key is a string: "CharacterAID_CharacterBID"
    private Dictionary<string, Relationship> relationshipDatabase = new Dictionary<string, Relationship>();
    private Dictionary<string, List<Relationship>> characterRelationships = new Dictionary<string, List<Relationship>>();

    void Awake() => Instance = this;

    private string GetKey(CharacterData a, CharacterData b)
    {
        // String.Compare ensures the key is always "Alpha_Beta" and never "Beta_Alpha"
        return string.Compare(a.characterId, b.characterId) < 0
            ? $"{a.characterId}_{b.characterId}"
            : $"{b.characterId}_{a.characterId}";
    }

    public Relationship CreateRelationship(CharacterData a, CharacterData b, FeudalStatus fs)
    {
        string key = GetKey(a, b);
        Relationship relationship;
        if (!relationshipDatabase.ContainsKey(key))
        {
            relationship = new Relationship();
            relationship.charA.fear = 0;
            relationship.charA.opinion = 0;
            relationship.charA.trust = 0;
            relationship.charB.fear = 0;
            relationship.charB.opinion = 0;
            relationship.charB.trust = 0;
            if (a.GetInstanceID() < b.GetInstanceID())
            {
                relationship.charA.charID = a.characterId;
                relationship.charB.charID = b.characterId;
            }
            else
            {
                relationship.charA.charID = b.characterId;
                relationship.charB.charID = a.characterId;
            }
            List<Relationship> r = new List<Relationship>
            {
                relationship
            };
            AddToCharacterCache(a.characterId, relationship);
            AddToCharacterCache(b.characterId, relationship);
        }
        else
        {
            relationship = relationshipDatabase[key];
        }
        switch (fs)
        {
            case FeudalStatus.None:
                relationship.charA.status = FeudalStatus.None;
                relationship.charB.status = FeudalStatus.None;
                break;
            case FeudalStatus.Vassal:
                if (a.GetInstanceID() < b.GetInstanceID())
                {
                    relationship.charA.status = FeudalStatus.Vassal;
                    relationship.charB.status = FeudalStatus.Liege;
                }
                else
                {
                    relationship.charB.status = FeudalStatus.Liege;
                    relationship.charA.status = FeudalStatus.Vassal;
                }
                break;
            case FeudalStatus.Liege:
                if (a.GetInstanceID() < b.GetInstanceID())
                {
                    relationship.charB.status = FeudalStatus.Liege;
                    relationship.charA.status = FeudalStatus.Vassal;
                }
                else
                {
                    relationship.charA.status = FeudalStatus.Vassal;
                    relationship.charB.status = FeudalStatus.Liege;
                }
                break;
        }
        relationshipDatabase.Add(key, relationship);
        return relationshipDatabase[key];
    }

    private void AddToCharacterCache(string charID, Relationship rel)
    {
        // 1. If the character isn't in the dictionary yet, give them a new list
        if (!characterRelationships.ContainsKey(charID))
        {
            characterRelationships[charID] = new List<Relationship>();
        }

        // 2. Only add the relationship if it isn't already in their personal list
        if (!characterRelationships[charID].Contains(rel))
        {
            characterRelationships[charID].Add(rel);
        }
    }

    public Relationship GetRelationship(CharacterData a, CharacterData b)
    {
        string key = GetKey(a, b);
        if (!relationshipDatabase.ContainsKey(key))
        {
            return CreateRelationship(a, b, FeudalStatus.None);
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

    public void ChangeLoyalty(CharacterData actor, CharacterData target, int amount)
    {
        Relationship rel = GetRelationship(actor, target);
        rel.loyalty = Mathf.Clamp(rel.loyalty + amount, -100, 100);
    }

    public int GetLoyalty(CharacterData actor, CharacterData target)
    {
        Relationship rel = GetRelationship(actor, target);
        return rel.loyalty;
    }

    public List<Relationship> GetAllRelationshipsFor(CharacterData character)
    {
        string id = character.characterId;
        if (!characterRelationships.ContainsKey(id))
        {
            // THIS IS YOUR SMOKING GUN
            Debug.LogWarning($"[Relationship Gap] {character.characterName} {character.name} {character.role} {character.GetHighestTitle()} (ID: {id}) exists in the world but has NO entry in characterRelationships dictionary.");
            return new List<Relationship>();
        }
        return characterRelationships[character.characterId];
    }

    public CharacterData GetOtherCharacterInRelationship(Relationship r, CharacterData character)
    {
        string myID = character.characterId;

        // 1. Identify which ID in the relationship isn't mine
        // (Assuming you added charA_ID and charB_ID to your Relationship class)
        string otherID = (r.charA.charID == myID) ? r.charB.charID : r.charA.charID;

        // 2. Ask the registry for the actual object
        return CharacterRegistry.Instance.GetCharacter(otherID);
    }
}