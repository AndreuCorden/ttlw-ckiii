using UnityEngine;
using System.Collections.Generic;

public class CharacterInteractionUI : MonoBehaviour
{
    public CharacterData playerCharacter; // Assign your player ScriptableObject
    public GameObject cardPrefab;        // The InteractionCardUI prefab
    public Transform listParent;         // A Vertical Layout Group container

    private List<GameObject> activeCards = new List<GameObject>();

    void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        if (playerCharacter == null) return;
        if (cardPrefab == null || listParent == null) {
        Debug.LogError("UI Card Prefab or List Parent is missing in the Inspector!");
        return;
    }
        // Clear old UI elements
        foreach (GameObject card in activeCards) Destroy(card);
        activeCards.Clear();

        // Create a card for every pending interaction
        foreach (CharacterInteraction interaction in playerCharacter.pendingInteractions)
        {
            GameObject newCard = Instantiate(cardPrefab, listParent);
            newCard.GetComponent<InteractionCardUI>().Setup(interaction, playerCharacter, this);
            activeCards.Add(newCard);
        }
    }

    public void RemoveInteraction(CharacterInteraction interaction, GameObject cardObj)
    {
        // Remove from the data list
        playerCharacter.pendingInteractions.Remove(interaction);
        
        // Remove from UI
        activeCards.Remove(cardObj);
        Destroy(cardObj);

        // If no more interactions, you might want to close the whole window
        if (playerCharacter.pendingInteractions.Count == 0)
        {
            // gameObject.SetActive(false);
        }
    }
}