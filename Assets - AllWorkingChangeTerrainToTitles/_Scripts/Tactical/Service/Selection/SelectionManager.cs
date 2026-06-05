using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Declare hitUnit here by grabbing it from the object we touched
            Unit hitUnit = hit.collider.GetComponent<Unit>();

            // Pass it to our Service (Check if Shift is held for multi-select)
            bool isMultiSelect = Input.GetKey(KeyCode.LeftShift);
            SelectionService.Instance.HandleSelection(hitUnit, isMultiSelect);
        }
        else
        {
            // If we hit the sky or ground with no unit, deselect all
            SelectionService.Instance.DeselectAll();
        }
    }
    }

    public void DeselectAll()
    {
        // This is the new, non-deprecated way to find all units in the scene
        Unit[] allUnits = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);

        foreach (Unit u in allUnits)
        {
            u.SetSelected(false);
        }
    }
}