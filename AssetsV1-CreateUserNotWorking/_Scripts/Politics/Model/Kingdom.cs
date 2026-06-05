[System.Serializable]
public class Kingdom {
    public string kingdomName;
    public float treasury;
    public Territory capitalTerritory; // Reference to the "Kingdom_Container" object

    // This is a Property: It calculates the value whenever you ask for it.
    // No more manual updating needed!
    public int TotalPopulation {
        get { return capitalTerritory != null ? capitalTerritory.population : 0; }
    }

    public void SetUpKingdom(Territory capital) {
        capitalTerritory = capital;
        kingdomName = capital.territoryName + " Kingdom";
        treasury = 1000f;
    }
}