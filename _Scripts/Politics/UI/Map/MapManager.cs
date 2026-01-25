using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public List<Territory> allTerritories = new List<Territory>();
    public TerritoryType currentMapMode = TerritoryType.Town;

    public CharacterData character;

    void Start()
    {
    }

    public void SetMapMode(int typeIndex)
    {
        currentMapMode = (TerritoryType)typeIndex;
        Debug.Log("Switching Map Mode to: " + currentMapMode);
        UpdateMapVisuals();
    }

    public void ShowMapLayer(int typeIndex)
    {
        TerritoryType selectedType = (TerritoryType)typeIndex;

        foreach (Territory t in allTerritories)
        {
            // Simple logic: if it's the right layer, show it. If not, dim it.
            if (t.type == selectedType)
            {
                t.GetComponent<SpriteRenderer>().color = Color.white; // High visibility
            }
            else
            {
                t.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Dimmed
            }
        }
    }

    public void UpdateMapVisuals()
    {
        Territory[] allTiles = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);

        foreach (Territory t in allTiles)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr == null) continue;
            if (t.type == TerritoryType.Water)
            {
                sr.color = Color.blue;
            }
            else
            {
                // Climb the tree to find the leader for the specific mode
                sr.color = GetColorForMode(t, currentMapMode);
            }
        }
    }

    private Color GetColorForMode(Territory t, TerritoryType mode)
    {
        // If we found the right level, return that color
        if (t.type == mode)
            return t.territoryColour;

        // Otherwise, keep looking up the family tree
        else if (t.parentTerritory != null)
            return GetColorForMode(t.parentTerritory, mode);

        // Fallback.
        return Color.gray;
    }
}