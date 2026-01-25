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

    public void OpenDiplomacy(CharacterData character)
    {
        targetCharacter = character;
        nameText.text = character.characterName;

        // Update Opinion using the Manager
        int opinion = GlobalGameManager.Instance.GetOpinion(character);
        opinionText.text = "Opinion: " + opinion;

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
}