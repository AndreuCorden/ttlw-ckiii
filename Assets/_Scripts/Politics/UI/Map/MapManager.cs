using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public List<Territory> allTerritories = new List<Territory>();
    public TitleRank currentMapMode = TitleRank.King;

    void Start()
    {
    }

    public void SetMapMode(int typeIndex)
    {
        currentMapMode = (TitleRank)typeIndex;
        Debug.Log("Switching Map Mode to: " + currentMapMode);
        UpdateMapVisuals();
    }
    public void UpdateMapVisuals()
    {
        Territory[] allTerritories = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);

        foreach (Territory territory in allTerritories)
        {
            SpriteRenderer sr = territory.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            // Default to gray/white if something is truly wrong
            Color targetColor = Color.white;

            switch (currentMapMode)
            {
                case TitleRank.Baron:
                    // Show the direct owner's color (the Lord who actually holds this tile)
                    if (territory.owner != null) targetColor = territory.owner.colour;
                    break;

                case TitleRank.Count:
                    // If it's part of a county, use that. 
                    // If the Count/Duke seized it directly, fall back to the owner's color.
                    if (territory.county != null) targetColor = territory.county.colour;
                    else if (territory.owner != null) targetColor = territory.owner.colour;
                    else Debug.Log($"{territory} and {territory.owner}");
                    break;

                case TitleRank.Duke:
                    if (territory.duchy != null) targetColor = territory.duchy.colour;
                    else if (territory.owner != null) targetColor = territory.owner.colour;
                    break;

                case TitleRank.King:
                    if (territory.kingdom != null) targetColor = territory.kingdom.colour;
                    break;
            }

            sr.color = targetColor;
        }
    }
}