using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [System.Obsolete]
    void Update()
    {
        if (isSelectingTerritory && Input.GetMouseButtonDown(0))
        {
            DetectTerritoryClick();
        }
    }

    [System.Obsolete]
    private void DetectTerritoryClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
            Territory clickedTerritory = hit.collider.GetComponent<Territory>();

            if (clickedTerritory != null && clickedTerritory.territoryType == TerritoryType.Land)
            {
                TitleRank currentMapMode = Object.FindAnyObjectByType<MapManager>().currentMapMode;
                // 1. Assign Player
                Title playerSelectedTitle = PlayerManager.Instance.AssignPlayerToTerritory(clickedTerritory,currentMapMode);

                // 2. Resume the rest of the world generation
                Object.FindAnyObjectByType<MapGenerator>().FinalizeWorldGeneration(playerSelectedTitle);

                // 3. Close UI
                selectionText.SetActive(false);
                isSelectingTerritory = false;
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