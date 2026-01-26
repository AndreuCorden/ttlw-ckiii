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

        // Clean up
        GlobalGameManager.Instance.defender = null;
    }
}