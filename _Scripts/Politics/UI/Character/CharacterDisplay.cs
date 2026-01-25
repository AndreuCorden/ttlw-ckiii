using UnityEngine;
using TMPro; // Use this if you created TextMeshPro objects

public class CharacterDisplay : MonoBehaviour
{
    public CharacterData characterToDisplay;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI armyText;
    public GameObject battleButton; // Drag the BattleButton here in Inspector

    public DiplomacyUI diplomacyWindow; // Drag your DiplomacyPanel here in the Inspector
    public CharacterSelectionUI selectionUI;

    void Start()
    {
        if (characterToDisplay != null)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        nameText.text = characterToDisplay.characterName;
        statsText.text = $"Prowess: {characterToDisplay.prowess} | Loyalty: {characterToDisplay.loyalty}%";

        // If loyalty is 0 or less, show the battle button and change text color
        if (characterToDisplay.loyalty <= 0)
        {
            battleButton.SetActive(true);
            statsText.text = "STATE: IN REBELLION!";
            statsText.color = Color.red;
        }
        else
        {
            battleButton.SetActive(false);
            statsText.color = Color.white;
        }

        // Count total soldiers in the army
        int totalSoldiers = 0;
        foreach (var unit in characterToDisplay.army)
        {
            totalSoldiers += unit.currentSoldierCount;
        }

        armyText.text = $"Total Soldiers: {totalSoldiers}";
    }

    void Update()
    {
        // Check if the Escape key is pressed while the display is active
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDisplay();
        }
    }

    public void OpenCharacterDisplay(CharacterData character)
    {
        if (character == null)
        {
            Debug.LogError("CharacterDisplay received a NULL character!");
            return;
        }
        characterToDisplay = character;
        UpdateUI();
        gameObject.SetActive(true);
        if (selectionUI.isActiveAndEnabled)
        {
            OnFamilyClick();
        }
    }

    public void OnGiftClick() { DiplomacyManager.Instance.GiveGift(characterToDisplay); UpdateUI(); }

    public void OnInsultClick() { DiplomacyManager.Instance.SendInsult(characterToDisplay); UpdateUI(); }

    public void OnBattleClick() => PoliticsController.Instance.StartBattle(characterToDisplay);

    public void OnOpenDiplomacy()
    {
        if (diplomacyWindow != null)
        {
            diplomacyWindow.gameObject.SetActive(true);
            diplomacyWindow.OpenDiplomacy(characterToDisplay);
        }
    }

    public void CloseDisplay()
    {
        gameObject.SetActive(false);
    }

    public void OnFamilyClick()
    {
        selectionUI.OpenCharacterMenu(characterToDisplay);
    }
}