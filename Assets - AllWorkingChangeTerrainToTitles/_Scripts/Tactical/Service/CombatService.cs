using UnityEngine;

public class CombatService 
{
    private static CombatService _instance;
    public static CombatService Instance => _instance ??= new CombatService();

    public void ExecuteAttack(Unit attacker, Unit target)
    {
        if (target == null || attacker.teamColor == target.teamColor) return;

        // Logic check: Can we attack yet?
        if (Time.time > attacker.lastAttackTime + attacker.attackSpeed)
        {
            target.TakeDamage(attacker.attackDamage);
            attacker.lastAttackTime = Time.time;
            Debug.Log($"{attacker.name} hit {target.name} for {attacker.attackDamage}");
            
            // Here you could trigger animations or sound effects
        }
    }
}