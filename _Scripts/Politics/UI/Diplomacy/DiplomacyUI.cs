using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyUI : MonoBehaviour
{
    public CharacterData targetCharacter;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI opinionText;
    public Transform traitIconContainer; // A panel with a Horizontal Layout Group
    public GameObject traitIconPrefab;    // A small UI Image prefab

    public void OnGiftButtonClick()
    {
        if (targetCharacter == null) return;

        // Call the Service logic
        DiplomacyManager.Instance.GiveGift(targetCharacter);

        // Refresh the UI to show the new Opinion score
        OpenDiplomacy(targetCharacter);
    }

    public void CloseWindow()
    {
        // This turns off the panel
        gameObject.SetActive(false);

        CharacterDisplay mainDisplay = Object.FindFirstObjectByType<CharacterDisplay>();
        if (mainDisplay != null)
        {
            mainDisplay.UpdateUI();
        }
    }

    public void OpenDiplomacy(CharacterData target)
    {
        targetCharacter = target;
        CharacterData player = PlayerManager.Instance.playerCharacter;

        // Get the shared connection
        Relationship rel = RelationshipManager.Instance.GetRelationship(player, target);

        nameText.text = target.characterName;
        opinionText.text = $"Opinion of You: {rel.opinion}";

        // Toggle UI based on if they have met
        if (rel == null)
        {
            ShowUnknownDiplomacy(target);
        }
        else
        {
            ShowFullDiplomacy(target);
        }
    }

    public void ShowFullDiplomacy(CharacterData character)
    {
        targetCharacter = character;
        nameText.text = character.characterName;

        // Clear and Spawn Trait Icons
        foreach (Transform child in traitIconContainer) Destroy(child.gameObject);

        foreach (Trait t in character.traits)
        {
            GameObject icon = Instantiate(traitIconPrefab, traitIconContainer);

            // Reset the local scale and position to ensure the Layout Group takes over
            icon.transform.localPosition = Vector3.zero;
            icon.transform.localScale = Vector3.one;

            HoverTooltipTrigger script = icon.GetComponent<HoverTooltipTrigger>();
            if (script != null) script.Setup(t);

            LayoutRebuilder.ForceRebuildLayoutImmediate(traitIconContainer as RectTransform);
        }
    }

    private void ShowUnknownDiplomacy(CharacterData character)
    {
        nameText.text = "Unknown Lord/Lady"; // Or just the name but hide traits
        opinionText.text = "Opinion: ???";

        // Hide standard buttons, show "Send Envoy" button
        // giftButton.gameObject.SetActive(false);
        //envoyButton.gameObject.SetActive(true);
    }
}