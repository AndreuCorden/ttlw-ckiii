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

    public void RevokeTitle(CharacterData exVassal)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void PetitionLiege(CharacterData liege)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void GrantTitle(CharacterData vassal)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void OfferOath(CharacterData liege)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void DemandFilety(CharacterData target)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;
        Relationship rel = RelationshipManager.Instance.GetRelationship(player, target);

        if (player.GetInstanceID() < target.GetInstanceID())
        {
            // Logic check: Will they say yes?
            if (rel.charA.opinion > 50 && player.influence > target.influence)
            {
                // Set up the Liege/Vassal bond in the Title system
                ExecuteVassalization(player, target);
            }
        }
        else
        {
            // Logic check: Will they say yes?
            if (rel.charB.opinion > 50 && player.influence > target.influence)
            {
                // Set up the Liege/Vassal bond in the Title system
                ExecuteVassalization(player, target);
            }
        }
    }

    public void SendEmissary(CharacterData target)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;
        RelationshipManager.Instance.GetRelationship(player, target);
        player.knownCharacters.Add(target);
        target.knownCharacters.Add(player);
    }

    public void FormMarriage(CharacterData charA, CharacterData charB)
    {
        // 1. Set the Legal References
        charA.spouse = charB;
        charB.spouse = charA;

        // 2. Align the Dynasties (The "Join")
        // We don't merge them, we just ensure they share a "Connection" flag
        Relationship rel = RelationshipManager.Instance.GetRelationship(charA, charB);
        rel.isAllied = true;

        Debug.Log($"{charA.characterName} and {charB.characterName} are now wed.");
    }

    public void BreakMarriage(CharacterData charA, CharacterData charB)
    {
        // 1. Clear Legal References
        charA.spouse = null;
        charB.spouse = null;

        // 2. Break the Alliance
        Relationship rel = RelationshipManager.Instance.GetRelationship(charA, charB);
        rel.isAllied = false;

        // 3. The "Scandal" Penalty
        RelationshipManager.Instance.ChangeOpinion(charA, charB, -40);
        RelationshipManager.Instance.ChangeOpinion(charB, charA, -40);
    }

    public void FosterChild()
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void FabricateClaim(CharacterData targetTitle)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;
        // Costs Influence to "fake" a document
        if (player.influence >= 100)
        {
            player.influence -= 100;
            // Logic to add this title to a "Unpressed Claims" list
        }
    }

    public void Sway(CharacterData targetTitle)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;

    }

    public void Antagonize(CharacterData targetTitle)
    {
        CharacterData player = PlayerManager.Instance.playerCharacter;
        RelationshipManager.Instance.ChangeOpinion(player,targetTitle,-20);

    }
    public void ExecuteVassalization(CharacterData player, CharacterData target)
    {

    }
}