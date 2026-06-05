using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteraction : MonoBehaviour
{
    public CharacterDisplay characterDisplay; // Drag your CharacterDisplay here
    public MapManager mapManager; // Drag your MapManager here

    public TownDisplay townUI; // Drag your TownDisplay here

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Stop the code here so we don't click the map through the UI
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                Territory t = hit.collider.GetComponent<Territory>();
                if (t != null)
                {
                    CharacterData leaderToDisplay = null;

                    // Look at MapManager to see what level we are viewing
                    int currentMode = (int)Object.FindAnyObjectByType<MapManager>().currentMapMode;

                    if (currentMode == 0) // Town Mode
                        leaderToDisplay = t.leader;
                    else if (currentMode == 1) // County Mode
                        leaderToDisplay = t.parentTerritory?.leader;
                    else if (currentMode == 2) // Province Mode
                        leaderToDisplay = t.parentTerritory?.parentTerritory?.leader;
                    else if (currentMode == 5) // Kingdom Mode
                        leaderToDisplay = t.parentTerritory?.parentTerritory?.parentTerritory?.leader;

                    if (leaderToDisplay != null)
                        townUI.CloseDisplay();
                    characterDisplay.OpenCharacterDisplay(leaderToDisplay);
                }
            }
        }
        // Inside MapInteraction.Update()
        if (Input.GetMouseButtonDown(1)) // 1 is Right Click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                Territory t = hit.collider.GetComponent<Territory>();
                if (t != null)
                {
                    // If we clicked a container, we find the first town tile inside it
                    if (t.type != TerritoryType.Town)
                    {
                        t = GetFirstTown(t);
                    }

                    if (t != null)
                    {
                        characterDisplay.CloseDisplay();
                        townUI.OpenTownDisplay(t);
                    }
                }
            }
        }
    }
    // Helper to ensure right-click finds a town even if clicking a large border
    private Territory GetFirstTown(Territory container)
    {
        if (container.type == TerritoryType.Town) return container;
        if (container.subTerritories.Count > 0) return GetFirstTown(container.subTerritories[0]);
        return null;
    }
}