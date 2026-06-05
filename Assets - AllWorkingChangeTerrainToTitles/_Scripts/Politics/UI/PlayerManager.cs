using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    [Header("Player Identity")]
    public CharacterData playerCharacter;
    public Family playerFamily;
    public Kingdom playerKingdom;
    private string pendingKingdomName;

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
        playerFamily = new Family(lastName, playerCharacter);
        playerCharacter.family = playerFamily;

        Debug.Log($"Created {firstName} {lastName}. Now select a territory on the map.");
    }

    public Territory AssignPlayerToTerritory(Territory clickedTile,TerritoryType currentMapMode)
    {
        Territory current = clickedTile;
        while (current.type != currentMapMode && current.parentTerritory != null)
        {
            current = current.parentTerritory;
        }
        // Transfer power to the player
        current.leader = playerCharacter;
        playerCharacter.governedTerritory = current;

        // Setup the Kingdom
        playerKingdom = new Kingdom();
        playerKingdom.SetUpKingdom(current);
        playerKingdom.kingdomName = pendingKingdomName; // Or custom name
        playerCharacter.family.familyColor = current.territoryColour;
        
        current.ownerKingdom = playerKingdom;
        Debug.Log($"Player is now the ruler of {current.territoryName}");
        
        return current;
    }

    public void RequestInvasionPermission(CharacterData targetVassal)
{
    CharacterData liege = playerCharacter.liege;

    // Logic: King denies if Authority is high and Stability is needed
    // King ignores/accepts if he is weak (Low Authority)
    if (liege.influence < 30) 
    {
        Debug.Log("The King is too weak to stop you. You proceed anyway.");
        StartWar(targetVassal);
    }
    else 
    {
        // Roll for permission based on relationship
        bool permissionGranted = Random.value > 0.5f;
        if (!permissionGranted) 
        {
            // If you invade anyway, you lose reputation with the family
            playerFamily.reputation -= 20;
            Debug.Log("The King denied you! Invading now will be seen as an act of defiance.");
        }
    }
}

    private void StartWar(CharacterData targetVassal)
    {
        Debug.Log($"Starting war against {targetVassal.characterName}!");
        // Implement war logic here
    }
}