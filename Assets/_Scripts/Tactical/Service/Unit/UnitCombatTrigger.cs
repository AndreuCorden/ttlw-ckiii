using UnityEngine;

public class UnitCombatTrigger : MonoBehaviour
{
    private Unit myUnit;

    void Start() { myUnit = GetComponent<Unit>(); }

    // Swapping to TriggerStay for Kinematic-to-Kinematic detection
    void OnTriggerStay(Collider other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if (otherUnit != null)
        {
            CombatService.Instance.ExecuteAttack(myUnit, otherUnit);
        }
    }
}