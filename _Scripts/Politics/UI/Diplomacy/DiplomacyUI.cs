using System.Linq;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyUI : MonoBehaviour
{
    public CharacterData targetCharacter;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI opinionText;
    public Transform traitIconContainer; // A panel with a Horizontal Layout Group
    public GameObject traitIconPrefab;    // A small UI Image prefab
    public GameObject UnknownView;
    public GameObject LiegeView;
    public GameObject VassalView;
    public GameObject KnownPersons;

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
        UnknownView.SetActive(false);
        LiegeView.SetActive(false);
        VassalView.SetActive(false);
        KnownPersons.SetActive(false);

        // Get the shared connection
        Relationship rel = RelationshipManager.Instance.GetRelationship(player, target);

        nameText.text = target.characterName;
        if (player.GetInstanceID() < target.GetInstanceID())
        {
            opinionText.text = $"Opinion of You: {rel.charB.opinion}";
        }
        else
        {
            opinionText.text = $"Opinion of You: {rel.charA.opinion}";
        }

        // Toggle UI based on if they have met
        if (rel == null)
        {
            Debug.Log("1");
            ShowUnknownDiplomacy(target);
        }
        else if (player.IsLiegeOf(target))
        {
            Debug.Log("2");
            LiegeViewObjects(target);
        }
        else if (player.IsVassalOf(target))
        {
            Debug.Log("3");
            VassalViewObjects(target);
        }
        else if (player.knownCharacters.Contains(target))
        {
            Debug.Log("4");
            KnownCharacterView(target);
        }
    }

    public void VassalViewObjects(CharacterData character)
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

        UnknownView.SetActive(false);
        LiegeView.SetActive(false);
        VassalView.SetActive(true);
        KnownPersons.SetActive(false);
    }

    public void LiegeViewObjects(CharacterData character)
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

        UnknownView.SetActive(false);
        LiegeView.SetActive(true);
        VassalView.SetActive(false);
        KnownPersons.SetActive(false);
    }

    public void KnownCharacterView(CharacterData character)
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

        UnknownView.SetActive(false);
        LiegeView.SetActive(false);
        VassalView.SetActive(false);
        KnownPersons.SetActive(true);
    }

    private void ShowUnknownDiplomacy(CharacterData character)
    {
        nameText.text = "Unknown Lord/Lady"; // Or just the name but hide traits
        opinionText.text = "Opinion: ???";

        UnknownView.SetActive(true);
        LiegeView.SetActive(false);
        VassalView.SetActive(false);
        KnownPersons.SetActive(false);
    }

    public void Antagonize()
    {
        AntagonizeInteraction Interaction = new AntagonizeInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
    }

    public void DemandFielty()
    {
        DemandFiletyInteraction Interaction = new DemandFiletyInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
    }

    public void Fabricate()
    {
        FabricateInteraction Interaction = new FabricateInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
    }

    public void FosterChild()
    {
        FosterChildInteraction Interaction = new FosterChildInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickChild() for -> Interaction.child = child;
    }

    public void GrantTitle()
    {
        GrantTitleInteraction Interaction = new GrantTitleInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickTitle() for -> Interaction.title = title;
    }

    public void Marriage()
    {
        MarriageInteraction Interaction = new MarriageInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickSpouse() for -> Interaction.spouse = spouse;
    }

    public void OfferOath()
    {
        OfferOathInteraction Interaction = new OfferOathInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickTitles()
    }

    public void Petition()
    {
        PetitionInteraction Interaction = new PetitionInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickGold()
    }

    public void Revoke()
    {
        RevokeInteraction Interaction = new RevokeInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
    }

    public void SendEmissary()
    {
        SendEmissaryInteraction Interaction = new SendEmissaryInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
    }

    public void Sway()
    {
        SwayInteraction Interaction = new SwayInteraction();
        Interaction.sender = PlayerManager.Instance.playerCharacter;
        targetCharacter.pendingInteractions.Add(Interaction);
        // PickTopic();
    }
}