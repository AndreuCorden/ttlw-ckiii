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
}