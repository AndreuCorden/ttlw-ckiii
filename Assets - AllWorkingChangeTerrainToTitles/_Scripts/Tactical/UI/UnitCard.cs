using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
    public Unit linkedUnit; // The 3D unit this card represents
    public TextMeshProUGUI unitNameText;
    public Image healthBarFill;
    public Image selectionHighlight; // A border that glows when selected
    public Button button;

    public void Setup(Unit unit)
    {
        linkedUnit = unit;
        unitNameText.text = unit.gameObject.name;

        // This checks if the button exists and adds the listener automatically
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); // Clear old ones to prevent double-clicks
            button.onClick.AddListener(OnCardClick);
        }
    }

    void Update()
    {
        if (linkedUnit == null)
        {
            Destroy(gameObject); // If the unit dies in battle, remove the card
            return;
        }

        // Update health bar on the card
        healthBarFill.fillAmount = linkedUnit.currentHealth / linkedUnit.maxHealth;

        // Show a border if this unit is selected in the 3D world
        selectionHighlight.enabled = linkedUnit.isSelected;
    }

    // This allows you to select the 3D unit by clicking the UI card
    public void OnCardClick()
    {
        // Tell a manager to select this unit
        BattleUIManager.Instance.SelectUnitFromCard(linkedUnit);
    }
}