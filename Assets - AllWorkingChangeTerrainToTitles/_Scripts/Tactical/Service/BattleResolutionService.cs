using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResolutionService : MonoBehaviour
{
    private bool battleOver = false;

    void Update()
    {
        if (battleOver) return;

        // Use a small timer so we don't check every single frame (better performance)
        if (Time.frameCount % 30 != 0) return; 

        Unit[] allUnits = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
        int playerUnits = 0;
        int enemyUnits = 0;

        foreach (Unit u in allUnits)
        {
            // Only count units that are actually alive
            if (u != null && u.currentHealth > 0)
            {
                if (u.isPlayerUnit) playerUnits++;
                else enemyUnits++;
            }
        }

        // Logic check
        if (enemyUnits == 0 && playerUnits > 0) EndBattle(true);
        else if (playerUnits == 0 && enemyUnits > 0) EndBattle(false);
    }

    void EndBattle(bool playerWon)
    {
        battleOver = true;
        Debug.Log(playerWon ? "VICTORY! Returning to Politics..." : "DEFEAT! Returning to Politics...");

        // Save the result to the Global Manager so Politics can react
        GlobalGameManager.Instance.lastBattleWon = playerWon;

        // IMPORTANT: Make sure your Politics scene is actually named "PoliticsScene" 
        // in your Build Settings (File > Build Settings)
        Invoke("ReturnToPolitics", 3f);
    }

    void ReturnToPolitics()
    {
        SceneManager.LoadScene("Politics");
    }
}