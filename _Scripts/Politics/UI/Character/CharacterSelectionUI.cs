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
        foreach (Title liege in character.heldTitles)
            if ( liege.liege != null) CreateButton("↑ LIEGE: " + liege.liege.holder.characterName, () => SelectCharacter(liege.liege.holder));

        // // 5. NAVIGATION: GO DOWN (Vassals/Land)
        foreach (Title title in character.heldTitles)
            foreach (Title vassal in title.vassals)
                CreateButton("↓ VASSAL: " + vassal.holder.characterName, () => SelectCharacter(vassal.holder));

        // // 6. SOCIAL: KNIGHTS
        foreach (CharacterData retinue in character.retinue)
            CreateButton($"{retinue.role}: " + retinue.characterName, () => SelectCharacter(retinue));

        // // 7. SOCIAL: PRIEST
        // if (character.priest != null)
        //     CreateButton(" PRIEST: " + character.priest.characterName, () => SelectCharacter(character.priest));
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