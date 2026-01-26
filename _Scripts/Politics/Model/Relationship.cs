[System.Serializable]
public class Relationship
{
    public int opinion = 0;   // -100 to 100
    public int trust = 0;     // 0 to 100
    public int fear = 0;      // 0 to 100
    
    // You can add "flags" for medieval status
    public bool isAtWar = false;
    public bool isAllied = false;
    public bool owesFavor = false;
}