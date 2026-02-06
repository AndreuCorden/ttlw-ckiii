using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionCardUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public Button acceptButton;
    public Button declineButton;

    private CharacterInteraction currentInteraction;
    private CharacterData player;
    private CharacterInteractionUI parentUI;

    public void Setup(CharacterInteraction interaction, CharacterData playerChar, CharacterInteractionUI mainUI)
    {
        currentInteraction = interaction;
        player = playerChar;
        parentUI = mainUI;

        descriptionText.text = $"{interaction.sender.characterName} wants to: {interaction.interactionName}";

        acceptButton.onClick.AddListener(OnAccept);
        declineButton.onClick.AddListener(OnDecline);
    }

    void OnAccept()
    {
        currentInteraction.Execute(player);
        Close();
    }

    void OnDecline()
    {
        currentInteraction.Decline(player);
        Close();
    }

    void Close()
    {
        parentUI.RemoveInteraction(currentInteraction, this.gameObject);
    }
}