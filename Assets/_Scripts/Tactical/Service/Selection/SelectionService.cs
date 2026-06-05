using UnityEngine;
using System.Collections.Generic;

public class SelectionService
{
    private static SelectionService _instance;
    public static SelectionService Instance => _instance ??= new SelectionService();

    private List<Unit> selectedUnits = new List<Unit>();

    public void HandleSelection(Unit unit, bool isMultiSelect)
    {
        if (!isMultiSelect) DeselectAll();

        if (unit != null && unit.isPlayerUnit)
        {
            unit.SetSelected(true);
            selectedUnits.Add(unit);
        }
    }

    public void DeselectAll()
    {
        // Instead of searching the whole scene (slow), we only clear what we tracked
        foreach (Unit u in selectedUnits)
        {
            if (u != null) u.SetSelected(false);
        }
        selectedUnits.Clear();

        // Safety backup: find any stragglers if needed
        Unit[] all = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit u in all) u.SetSelected(false);
    }
}