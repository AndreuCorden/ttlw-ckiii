using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    void Awake() => Instance = this;

    public void EndTurn()
    {
        Debug.Log("--- Starting End of Turn Processing ---");

        // 1. Process Economy & Population (Gold and Growth)
        GlobalEconomyManager.Instance.ProcessEconomy();

        // 2. Age the Characters
        AgeAllCharacters();

        // 3. Update UI
        // Trigger any UI refreshes here so the player sees the new numbers

        Debug.Log("--- Turn Complete ---");
    }

    private void AgeAllCharacters()
    {
        // Use FindObjectsByType for components, but for ScriptableObjects 
        // it's safer to track them in a list within your SocialEngine.
        // As a temporary fix, let's look for all Territories and age their leaders.

        Territory[] all = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None);
        HashSet<CharacterData> processed = new HashSet<CharacterData>();

        foreach (Territory t in all)
        {
            if (t.leader != null && !processed.Contains(t.leader))
            {
                t.leader.age += 1;
                processed.Add(t.leader);
            }
        }
    }
}