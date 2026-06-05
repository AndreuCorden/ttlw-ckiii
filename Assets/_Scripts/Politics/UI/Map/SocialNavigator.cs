using System.Collections.Generic;
using UnityEngine;

public class SocialNavigator : MonoBehaviour
{
    public void FocusOnCharacter(CharacterData target)
    {
        if (target == null || target.heldTitles == null)
        {
            Debug.LogWarning("Focus failed: Target or Territory is null.");
            return;
        }

        // Attempt to find a real tile position
        Vector3 targetPos = GetAveragePosition(target.heldTitles[0].directDomain);

        // Ensure we keep the Camera's Z axis so we don't zoom into the map
        Vector3 newCamPos = new Vector3(targetPos.x, targetPos.y, -10f);

        Debug.Log($"Moving Camera to: {newCamPos} for character {target.characterName}");

        Camera.main.transform.position = newCamPos;

        // Trigger map mode
        MapManager mm = Object.FindAnyObjectByType<MapManager>();
        if (mm != null) mm.SetMapMode((int)target.heldTitles[0].rank);
    }

    public Vector3 GetAveragePosition(List<Territory> domain)
    {
        if (domain == null || domain.Count == 0)
            return Vector3.zero;

        Vector3 sumPosition = Vector3.zero;

        // 1. Accumulate all positions
        foreach (Territory t in domain)
        {
            sumPosition += t.transform.position;
        }

        // 2. Divide by the count to get the average
        Vector3 averagePosition = sumPosition / domain.Count;

        return averagePosition;
    }
}