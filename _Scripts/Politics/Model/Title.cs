using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;

public class Title : MonoBehaviour
{
    public string titleName;
    public TitleRank rank; // Mayor, Count, Duke, King
    public CharacterData holder;
    public Color colour;

    // The land this Title holder manages DIRECTLY (their Demesne)
    // For a Mayor, this is 1 Town. For a King, this is the Capital Province.
    public List<Territory> directDomain = new List<Territory>();
    public Territory seatOfPower; // The main Territory associated with this Title

    // The people who report to this Title
    public List<Title> vassals = new List<Title>();
    public Title liege;
    public int personalPopulation = 0;
    public int totalPopulation = 0;

    public void CalculateTreasury()
    {
        float treasury = seatOfPower.GetGoldPerTurn();
        foreach(Territory territory in directDomain)
        {
            treasury += territory.GetGoldPerTurn();
        }
        foreach (var vassal in vassals) treasury += vassal.CalculateTaxForLiege();
        holder.treasury = treasury;
    }

    public float CalculateTaxForLiege()
    {
        // Whatever logic you want: e.g., 20% of their total income
        CalculateTreasury();
        float tax = holder.treasury * 0.2f;
        holder.treasury -= tax;
        return tax;
    }

    public int GetPopulation()
    {
        return directDomain.Sum(t => t.population);
    }

    public List<Territory> FullRealmTiles
    {
        get
        {
            // Start with the land we own personally
            List<Territory> realm = new List<Territory>(directDomain);

            // Add the land of every vassal (and their vassals, and so on...)
            foreach (Title vassal in vassals)
            {
                realm.AddRange(vassal.FullRealmTiles);
            }

            return realm;
        }
    }
}

public enum TitleRank { Baron, Count, Duke, King }