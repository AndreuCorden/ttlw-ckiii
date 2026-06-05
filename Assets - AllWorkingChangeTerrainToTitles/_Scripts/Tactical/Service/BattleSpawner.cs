using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    void Start()
    {
        // 1. Logic: Spawn Defending Army
        if (GlobalGameManager.Instance.defender != null)
            InitializeArmy(GlobalGameManager.Instance.defender, new Vector3(0, 1, 20), Color.red, false);

        // 2. Logic: Spawn Player Army
        if (GlobalGameManager.Instance.playerData != null)
            InitializeArmy(GlobalGameManager.Instance.playerData, new Vector3(0, 1, -20), Color.blue, true);

        // 3. UI: Notify the UI that units are ready
        BattleUIManager.Instance.CreateUnitCards();
    }

    private void InitializeArmy(CharacterData data, Vector3 position, Color color, bool isPlayer)
    {
        float spacing = 5f;
        for (int i = 0; i < data.army.Count; i++)
        {
            Vector3 spawnPos = position + new Vector3(i * spacing - (data.army.Count * spacing / 2), 0, 0);
            CreateUnit(data.army[i], spawnPos, color, isPlayer);
        }
    }

    private void CreateUnit(ArmyUnitData stack, Vector3 pos, Color color, bool isPlayer)
    {
        GameObject go = Instantiate(stack.unitType.unitPrefab, pos, Quaternion.identity);

        // Setup Model
        Unit unit = go.GetComponent<Unit>();
        unit.currentHealth = stack.currentSoldierCount;
        unit.maxHealth = stack.maxSoldierCount;
        unit.teamColor = color;
        unit.isPlayerUnit = isPlayer;

        // Setup Visuals
        go.GetComponent<Renderer>().material.color = color;

        // Setup Logic/Service components
        if (!isPlayer)
        {
            go.AddComponent<EnemyAIService>();
        }
        else
        {
            // Only the player gets the right-click movement script
            go.AddComponent<UnitMovement>();
        }
    }
}