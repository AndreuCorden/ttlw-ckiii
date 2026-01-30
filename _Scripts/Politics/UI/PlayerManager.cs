using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Player Identity")]
    public CharacterData playerCharacter;
    public Family playerFamily;
    private string pendingKingdomName;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Role;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Prowess;
    public TextMeshProUGUI Influence;

    public GameObject parameterView;

    void Awake() => Instance = this;

    // Called by the UI "Start Game" button
    public void CreatePlayerIdentity(string firstName, string lastName, string kingdomName)
    {
        // 1. Create the Character
        playerCharacter = ScriptableObject.CreateInstance<CharacterData>();
        playerCharacter.characterName = firstName;
        playerCharacter.age = 20;
        pendingKingdomName = kingdomName;

        // 2. Create the Family
        Family playerFamily = ScriptableObject.CreateInstance<Family>();
        playerFamily.Initialize(lastName, playerCharacter);
        playerCharacter.family = playerFamily;

        Object.FindAnyObjectByType<GlobalGameManager>().playerData = playerCharacter;

        Debug.Log($"Created {firstName} {lastName}. Now select a territory on the map.");
    }

    public Title AssignPlayerToTerritory(Territory clickedTile, TitleRank currentMapMode)
    {
        Title titleToAssign = null;

        switch (currentMapMode)
        {
            case TitleRank.Baron:
                // Find the specific title sitting on this tile. 
                // We look at the vassals of the County because Barons are Count's vassals.
                titleToAssign = clickedTile.county.vassals.Find(v => v.rank == TitleRank.Baron && v.directDomain.Contains(clickedTile));

                // If it's not a Barony (e.g. it's the King's personal tile), 
                // then the player can't "be" the Baron there because no Barony exists.
                break;

            case TitleRank.Count:
                titleToAssign = clickedTile.county;
                break;

            case TitleRank.Duke:
                titleToAssign = clickedTile.duchy;
                break;

            case TitleRank.King:
                titleToAssign = clickedTile.kingdom;
                break;
        }

        if (titleToAssign != null)
        {
            titleToAssign.holder = playerCharacter;
            titleToAssign.titleName = pendingKingdomName;
            playerCharacter.heldTitles.Add(titleToAssign);
        }

        return titleToAssign;
    }

    public void SetActiveParameterView()
    {
        parameterView.SetActive(true);
    }

    public void UpdateCharacterParameters()
    {
        Name.text = playerCharacter.characterName;
        Role.text = playerCharacter.GetHighestRank().ToString();
        Gold.text = playerCharacter.GetGold().ToString();
        Prowess.text = playerCharacter.prowess.ToString();
        Influence.text = playerCharacter.influence.ToString();
    }
}