using UnityEngine;
using System.Collections.Generic;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    public GameObject unitCardPrefab;
    public Transform unitTrayParent; // The panel with the Horizontal Layout Group

    void Awake() { Instance = this; }

    // Call this from your BattleSpawner after all units are created
    public void CreateUnitCards()
{
    Unit[] allUnits = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
    
    foreach (Unit u in allUnits)
    {
        if (u.isPlayerUnit)
        {
            GameObject newCard = Instantiate(unitCardPrefab, unitTrayParent);
            // Just call Setup. The card handles the rest!
            newCard.GetComponent<UnitCard>().Setup(u);
        }
    }
}

    public void SelectUnitFromCard(Unit unitToSelect)
{
    SelectionManager selManager = Camera.main.GetComponent<SelectionManager>();
    
    // If NOT holding shift, clear others first
    if (!Input.GetKey(KeyCode.LeftShift))
    {
        selManager.DeselectAll();
    }
    
    unitToSelect.SetSelected(true);
}
}