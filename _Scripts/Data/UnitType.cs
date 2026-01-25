using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitType", menuName = "Strategy/Unit Type")]
public class UnitType : ScriptableObject
{
    public string typeName;
    public float moveSpeed = 5f;
    public int attackPower = 10;
    public int defensePower = 5;
    public float attackRange = 2f;
    public GameObject unitPrefab; // The 3D model/sprite for this unit
    public float maxHealth = 100f;
}
