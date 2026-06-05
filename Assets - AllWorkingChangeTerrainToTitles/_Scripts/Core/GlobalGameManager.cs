using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GlobalGameManager : MonoBehaviour
{
    // This allows other scripts to access the Manager easily
    public static GlobalGameManager Instance { get; private set; }
    public List<Relationship> allRelationships = new List<Relationship>();

    public bool lastBattleWon = false;

    [Header("Battle Data")]
    public CharacterData playerData;
    public CharacterData defender;

    private void Awake()
    {
        // This is the "Singleton" pattern. It ensures only one Manager exists.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This makes the object survive scene changes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this function when you want to start a fight!
    public void StartBattle(CharacterData playerChar, CharacterData enemyChar)
    {
        playerData = playerChar;
        defender = enemyChar;
        SceneManager.LoadScene("BattleScene");
    }

    public int GetOpinion(CharacterData subject)
    {
        foreach (Relationship rel in allRelationships)
        {
            // Assuming characterA is always the player or we check both
            if (rel.characterB == subject) return rel.opinionScore;
        }
        return 0; // Neutral if not found
    }

    public void ChangeOpinion(CharacterData npc, int amount)
    {
        Relationship foundRel = null;

        // 1. Search for an existing relationship
        foreach (Relationship rel in allRelationships)
        {
            if (rel.characterB == npc)
            {
                foundRel = rel;
                break;
            }
        }

        // 2. If no relationship exists, create a new one
        if (foundRel == null)
        {
            foundRel = new Relationship();
            foundRel.characterB = npc;
            foundRel.opinionScore = 0; // Start at neutral
            allRelationships.Add(foundRel);
        }

        // 3. Apply the change and Clamp it so it stays between -100 and 100
        foundRel.opinionScore += amount;
        foundRel.opinionScore = Mathf.Clamp(foundRel.opinionScore, -100, 100);

        Debug.Log($"Opinion of {npc.characterName} changed by {amount}. New score: {foundRel.opinionScore}");
    }
}