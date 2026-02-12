using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteraction : MonoBehaviour
{
    public CharacterDisplay characterDisplay; // Drag your CharacterDisplay here
    public List<Territory> territoriesOwnedByDisplayedCharacter = new List<Territory>();

    public TownDisplay townUI; // Drag your TownDisplay here

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0)) // Left Click - Character
        {
            HandleMapClick(true);
            DisplayTerritories();
        }
        else if (Input.GetMouseButtonDown(1)) // Right Click - Town
        {
            HandleMapClick(false);
        }
    }

    private void HandleMapClick(bool isLeftClick)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Territory t = hit.collider.GetComponent<Territory>();
            if (t == null) return;

            if (isLeftClick)
            {
                // 1. Get the current map mode
                TitleRank currentMode = Object.FindAnyObjectByType<MapManager>().currentMapMode;

                // 2. Direct Pull: Get the specific title based on the map mode
                Title targetTitle = null;
                switch (currentMode)
                {
                    case TitleRank.Baron: targetTitle = t.owner; break;
                    case TitleRank.Count: targetTitle = t.county; break;
                    case TitleRank.Duke: targetTitle = t.duchy; break;
                    case TitleRank.King: targetTitle = t.kingdom; break;
                }

                // 3. Fallback: If the specific rank doesn't exist (e.g. tile is owned directly by a King)
                // we climb the tree until we find the highest available lord.
                if (targetTitle == null)
                {
                    targetTitle = t.owner; // Start at the bottom
                    while (targetTitle != null && targetTitle.rank < currentMode && targetTitle.liege != null)
                    {
                        targetTitle = targetTitle.liege;
                    }
                }

                if (targetTitle != null && targetTitle.holder != null)
                {
                    townUI.CloseDisplay();
                    characterDisplay.OpenCharacterDisplay(targetTitle.holder);
                }
                else
                {
                    Debug.LogWarning($"No holder found for {targetTitle?.name ?? "NULL Title"} at {t.name}");
                }
            }
            else // Right Click
            {
                characterDisplay.CloseDisplay();
                townUI.OpenTownDisplay(t);
            }
        }
    }

    private void DisplayTerritories()
    {
        foreach(Territory t in territoriesOwnedByDisplayedCharacter)
        {
            t.IsDisplayed(false);
        }
        territoriesOwnedByDisplayedCharacter.Clear();
        foreach(Title t in characterDisplay.characterToDisplay.heldTitles)
        {
            foreach(Territory territory in t.directDomain)
            {
                territory.IsDisplayed(true);
                territoriesOwnedByDisplayedCharacter.Add(territory);
            }
        }
    }
}