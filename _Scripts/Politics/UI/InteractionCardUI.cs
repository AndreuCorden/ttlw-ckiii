using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionCardUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public Button acceptButton;
    public Button declineButton;
    public Button closeButton;

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
        closeButton.gameObject.SetActive(false);
    }

    void OnAccept()
    {
        currentInteraction.Execute(player);
        ShowResult(true);
    }

    void OnDecline()
    {
        currentInteraction.Decline(player);
        ShowResult(false);
    }

    void ShowResult(bool accepted)
    {
        closeButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        closeButton.onClick.AddListener(Close);
        if (accepted)
        {
            descriptionText.text = "Test: Interaction accepted!";
        }
        else
        {
            descriptionText.text = "Test: Interaction declined!";
        }
    }

    void Close()
    {
        parentUI.RemoveInteraction(currentInteraction, this.gameObject);
    }
}