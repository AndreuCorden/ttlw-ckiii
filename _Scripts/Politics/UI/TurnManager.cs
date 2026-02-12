using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    void Awake() => Instance = this;

    public void EndTurn()
    {
        Debug.Log("--- Starting End of Turn Processing ---");

        // 2. Age the Characters
        AgeAllCharacters();

        // Change Populations
        ProcessPopulation();

        // Calculate Kings wealth
        ProcessWealth();

        foreach (CharacterData character in CharacterRegistry.Instance.GetAllCharacters().Values)
        {
            if (character != PlayerManager.Instance.playerCharacter)
            {
                Actions.Instance.DecideNextAction(character);
            }
        }

        foreach (CharacterData character in CharacterRegistry.Instance.GetAllCharacters().Values)
        {
            if (character != PlayerManager.Instance.playerCharacter)
            {
                Actions.Instance.ExecuteInteractions(character);
            }
        }

        // 3. Update UI
        // Trigger any UI refreshes here so the player sees the new numbers

        CharacterInteractionUI ui = Object.FindAnyObjectByType<CharacterInteractionUI>(FindObjectsInactive.Include);

        if (ui != null)
        {
            // 2. Feed the data
            ui.playerCharacter = PlayerManager.Instance.playerCharacter;

            // 3. FORCE the GameObject to be active (This triggers OnEnable)
            ui.gameObject.SetActive(true);

            // 4. Double check the list is refreshed
            ui.RefreshList();

            Debug.Log($"UI opened with {ui.playerCharacter.pendingInteractions.Count} interactions.");
        }

        Debug.Log("--- Turn Complete ---");
    }

    private void AgeAllCharacters()
    {
        // Use FindObjectsByType for components, but for ScriptableObjects 
        // it's safer to track them in a list within your SocialEngine.
        // As a temporary fix, let's look for all Territories and age their leaders.

        Title[] all = Object.FindObjectsByType<Title>(FindObjectsSortMode.None);
        HashSet<CharacterData> processed = new HashSet<CharacterData>();

        foreach (Title t in all)
        {
            if (t.holder != null && !processed.Contains(t.holder))
            {
                t.holder.age += 1;
                processed.Add(t.holder);
            }
        }
    }

    public void ProcessPopulation()
    {
        // Find all Title objects where the rank is King
        List<Title> kingdoms = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
            .Where(t => t.rank == TitleRank.King)
            .ToList();

        // Find all Territory objects (usually the land tiles)
        List<Territory> allLand = Object.FindObjectsByType<Territory>(FindObjectsSortMode.None)
            .ToList();
        foreach (Title king in kingdoms)
        {
            king.personalPopulation = 0;
            king.totalPopulation = 0;
            foreach (Title lord in king.vassals)
            {
                RestartPopulationStats(lord);
            }
        }
        foreach (Territory land in allLand)
        {
            land.population += (int)(land.population * 0.05);
            land.county.totalPopulation += land.population;
            land.duchy.totalPopulation += land.population;
            land.kingdom.totalPopulation += land.population;
        }
        foreach (Title king in kingdoms)
        {
            int pop = 0;
            foreach (Territory land in king.directDomain)
            {
                pop += land.population;
            }
            king.personalPopulation = pop;
            foreach (Title lord in king.vassals)
            {
                RefreshPopulationStats(lord);
            }
        }
    }

    private void RefreshPopulationStats(Title lord)
    {
        int pop = 0;
        foreach (Territory land in lord.directDomain)
        {
            pop += land.population;
        }
        foreach (Title vassal in lord.vassals)
        {
            RefreshPopulationStats(vassal);
        }
        lord.personalPopulation = pop;
    }
    private void RestartPopulationStats(Title lord)
    {
        lord.personalPopulation = 0;
        lord.totalPopulation = 0;
        foreach (Title vassal in lord.vassals)
        {
            RestartPopulationStats(vassal);
        }
    }

    public void ProcessWealth()
    {
        List<Title> kingdoms = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
            .Where(t => t.rank == TitleRank.King)
            .ToList();

        foreach(Title kingdom in kingdoms)
        {
            kingdom.CalculateTreasury();
        }
    }
}