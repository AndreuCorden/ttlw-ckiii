using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GlobalEconomyManager : MonoBehaviour
{
    public static GlobalEconomyManager Instance;

    void Awake() { Instance = this; }

    public void ProcessEconomy()
    {
        // 1. Get ONLY the Towns to do the heavy lifting
        List<Title> kingdoms = Object.FindObjectsByType<Title>(FindObjectsSortMode.None)
                                        .Where(t => t.rank == TitleRank.King).ToList();

        foreach (Title king in kingdoms)
        {
            float gold = 0;
            foreach (Territory land in king.directDomain)
            {
                gold += (int)land.GetGoldPerTurn();
            }
            foreach (Title vassal in king.vassals)
            {
                gold += RefreshHierarchyStats(vassal);
            }
            king.personalTreasury += gold;
        }
    }

    private float RefreshHierarchyStats(Title lord)
    {
        float gold = 0;
        foreach (Territory land in lord.directDomain)
        {
            gold += land.GetGoldPerTurn();
        }
        foreach (Title vassal in lord.vassals)
        {
            gold += RefreshHierarchyStats(vassal);
        }
        float taxedGold = gold*0.1f;
        lord.personalTreasury += gold - taxedGold;
        return taxedGold;
    }
}