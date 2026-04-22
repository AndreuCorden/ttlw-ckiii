using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;

    public TMP_InputField kingdomNameInput;
    public GameObject creatorPanel;
    public GameObject selectionText; // The "Select Territory" text object

    [Header("State")]
    public bool isSelectingTerritory = false;

    public void OnClickContinue()
    {
        // 1. Send data to PlayerManager
        string fName = firstNameInput.text;
        string lName = lastNameInput.text;
        string kName = kingdomNameInput.text;

        if (string.IsNullOrEmpty(fName) || string.IsNullOrEmpty(lName) || string.IsNullOrEmpty(kName))
        {
            Debug.LogWarning("Please enter a name, family name, and kingdom name!");
            return;
        }

        PlayerManager.Instance.CreatePlayerIdentity(fName, lName, kName);
        // 2. Disable Creator UI, Enable Selection Prompt
        creatorPanel.SetActive(false);
        selectionText.SetActive(true);

        // 3. Enable the "Selection Mode"
        isSelectingTerritory = true;
    }

    void Update()
    {
        // Fix: Add the UI shield here too!
        if (isSelectingTerritory && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; // Ignore the click if we hit a UI button
            }
            DetectTerritoryClick();
        }
    }

    private void DetectTerritoryClick()
    {
        // Cleaner way to raycast in 2D without using obsolete math
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Territory clickedTerritory = hit.collider.GetComponent<Territory>();

            if (clickedTerritory != null && clickedTerritory.territoryType == TerritoryType.Land)
            {
                // Use the non-obsolete version for Unity 2023+
                MapManager mapManager = Object.FindFirstObjectByType<MapManager>();
                TitleRank currentMapMode = mapManager.currentMapMode;

                Title playerSelectedTitle = PlayerManager.Instance.AssignPlayerToTerritory(clickedTerritory, currentMapMode);

                // Safety Check: Ensure the player title actually exists before finalizing
                if (playerSelectedTitle != null)
                {
                    Object.FindFirstObjectByType<MapGenerator>().FinalizeWorldGeneration(playerSelectedTitle);

                    selectionText.SetActive(false);
                    isSelectingTerritory = false;
                }
                else
                {
                    Debug.LogError("Player title assignment failed! Check AssignPlayerToTerritory logic.");
                }
            }
        }
    }

    public void ShowCreator()
    {
        creatorPanel.SetActive(true);
        selectionText.SetActive(false);
        isSelectingTerritory = false;
    }
}