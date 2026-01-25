using UnityEngine;
using UnityEngine.SceneManagement;

public class PoliticsController : MonoBehaviour
{
    public static PoliticsController Instance;

    private void Awake() => Instance = this;

    private void Start()
    {
        // Check if we just returned from a battle
        if (GlobalGameManager.Instance.defender != null)
        {
            ApplyBattleConsequences();
        }
    }

    public void StartBattle(CharacterData enemy)
    {
        GlobalGameManager.Instance.defender = enemy;
        SceneManager.LoadScene("Battle");
    }

    private void ApplyBattleConsequences()
    {
        CharacterData opponent = GlobalGameManager.Instance.defender;
        bool won = GlobalGameManager.Instance.lastBattleWon;

        if (won)
        {
            opponent.loyalty -= 20;
            GlobalGameManager.Instance.ChangeOpinion(opponent, -30);
        }
        else
        {
            opponent.loyalty += 10;
        }

        // Clean up
        GlobalGameManager.Instance.defender = null;
    }

    public void DeclareIndependence(Territory myTerritory)
    {
        // 1. Create a new Kingdom object for yourself
        Kingdom newKingdom = new Kingdom();
        newKingdom.SetUpKingdom(myTerritory);

        // 2. Recursively change ownership
        ApplyNewKingdom(myTerritory, newKingdom);

        // 3. Optional: Trigger a 'Diplomatic Penalty' with the old King
        Debug.Log($"{myTerritory.territoryName} has broken away!");
    }

    private void ApplyNewKingdom(Territory t, Kingdom k)
    {
        t.ownerKingdom = k;
        foreach (Territory sub in t.subTerritories)
        {
            ApplyNewKingdom(sub, k);
        }
    }
}