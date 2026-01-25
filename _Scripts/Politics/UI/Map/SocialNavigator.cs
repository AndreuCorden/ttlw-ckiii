using UnityEngine;

public class SocialNavigator : MonoBehaviour
{
    public void FocusOnCharacter(CharacterData target)
    {
        if (target == null || target.governedTerritory == null)
        {
            Debug.LogWarning("Focus failed: Target or Territory is null.");
            return;
        }

        // Attempt to find a real tile position
        Vector3 targetPos = GetPhysicalLocation(target.governedTerritory);

        // Ensure we keep the Camera's Z axis so we don't zoom into the map
        Vector3 newCamPos = new Vector3(targetPos.x, targetPos.y, -10f);

        Debug.Log($"Moving Camera to: {newCamPos} for character {target.characterName}");

        Camera.main.transform.position = newCamPos;

        // Trigger map mode
        MapManager mm = Object.FindAnyObjectByType<MapManager>();
        if (mm != null) mm.SetMapMode((int)target.governedTerritory.type);
    }
    
    private Vector3 GetPhysicalLocation(Territory t)
    {
        // 1. If this is a physical tile, return its position
        if (t.type == TerritoryType.Town) return t.transform.position;

        // 2. Search all nested children for the first Territory with a SpriteRenderer
        Territory physicalChild = t.GetComponentInChildren<Territory>(false);

        // We loop through children to find a 'Town' specifically
        Territory[] allChildren = t.GetComponentsInChildren<Territory>();
        foreach (Territory child in allChildren)
        {
            if (child.type == TerritoryType.Town) return child.transform.position;
        }

        return t.transform.position;
    }
}