using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming you are using TextMeshPro

public class CharacterSelectionUI : MonoBehaviour
{
    public GameObject uiPanel;          // The background panel
    public TextMeshProUGUI headerText;  // "House [Name]"
    public GameObject buttonPrefab;     // A simple button with a text child
    public Transform listContainer;     // A Vertical Layout Group container

    public SocialNavigator navigator;

    public void OpenCharacterMenu(CharacterData character)
    {
        Debug.Log($"Opening Menu for {character.characterName}. List count: {character.vassals.Count}");
        uiPanel.SetActive(true);
        headerText.text = "House of " + character.family.familyName;

        foreach (Transform child in listContainer) Destroy(child.gameObject);

        if (character.father != null)
            CreateButton("▲ FATHER: " + character.father.characterName, () => SelectCharacter(character.father));

        if (character.mother != null)
            CreateButton("▲ MOTHER: " + character.mother.characterName, () => SelectCharacter(character.mother));

        // 1. SOCIAL: SPOUSE
        if (character.spouse != null)
            CreateButton("♥ SPOUSE: " + character.spouse.characterName, () => SelectCharacter(character.spouse));

        // 2. SOCIAL: CHILDREN
        foreach (CharacterData child in character.children)
            CreateButton("○ CHILD: " + child.characterName, () => SelectCharacter(child));

        // 3. SOCIAL: SIBLINGS
        foreach (CharacterData sibling in character.siblings)
            CreateButton("◊ SIBLING: " + sibling.characterName, () => SelectCharacter(sibling));

        // 4. NAVIGATION: GO UP (Hierarchy)
        if (character.liege != null)
            CreateButton("↑ LIEGE: " + character.liege.characterName, () => SelectCharacter(character.liege));

        // 5. NAVIGATION: GO DOWN (Vassals/Land)
        foreach (CharacterData vassal in character.vassals)
            CreateButton("↓ VASSAL: " + vassal.characterName, () => SelectCharacter(vassal));

        // 6. SOCIAL: KNIGHTS
        foreach (CharacterData knight in character.knights)
            CreateButton(" KNIGHT: " + knight.characterName, () => SelectCharacter(knight));

        // 7. SOCIAL: PRIEST
        if (character.priest != null)
            CreateButton(" PRIEST: " + character.priest.characterName, () => SelectCharacter(character.priest));
    }

    void CreateButton(string label, System.Action onClickAction)
    {
        GameObject btnObj = Instantiate(buttonPrefab, listContainer);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = label;
        btnObj.GetComponent<Button>().onClick.AddListener(() => onClickAction.Invoke());
    }

    void SelectCharacter(CharacterData target)
    {
        navigator.FocusOnCharacter(target); // Moves camera
        Object.FindAnyObjectByType<CharacterDisplay>().characterToDisplay = target;
        Object.FindAnyObjectByType<CharacterDisplay>().UpdateUI(); // Updates display
        OpenCharacterMenu(target);          // Refreshes UI for the new person
    }

    public void CloseMenu()
    {
        uiPanel.SetActive(false);
    }
}