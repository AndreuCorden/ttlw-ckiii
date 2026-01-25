using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Get the Unit component to check if we are selected
        Unit unitInfo = GetComponent<Unit>();

        if (unitInfo != null && !unitInfo.isPlayerUnit)
        {
            return; // If it's an enemy, stop right here and ignore the rest of the code
        }

        if (unitInfo.isSelected && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}