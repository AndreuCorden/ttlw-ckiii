using UnityEngine;
using UnityEngine.AI;

public class EnemyAIService : MonoBehaviour
{
    private NavMeshAgent agent;
    private Unit myUnit;
    private float searchTimer = 1.0f; // Only look for targets once per second (saves FPS)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myUnit = GetComponent<Unit>();
    }

    void Update()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0)
        {
            FindAndChaseTarget();
            searchTimer = 1.0f;
        }
    }

    void FindAndChaseTarget()
    {
        // Find all units
        Unit[] allUnits = Object.FindObjectsByType<Unit>(FindObjectsSortMode.None);
        Unit closestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (Unit u in allUnits)
        {
            // If it's a player unit and it's alive
            if (u.isPlayerUnit && u.currentHealth > 0)
            {
                float dist = Vector3.Distance(transform.position, u.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = u;
                }
            }
        }

        if (closestTarget != null)
        {
            agent.SetDestination(closestTarget.transform.position);
        }
    }
}