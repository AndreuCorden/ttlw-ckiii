[System.Serializable]
public class Relationship
{
    [System.Serializable]
    public struct CharRelationship
    {
        public int opinion;
        public int trust;
        public int fear;
    };
    public CharRelationship charA;
    public CharRelationship charB;
    
    // You can add "flags" for medieval status
    public bool isAtWar = false;
    public bool isAllied = false;
    public bool owesFavor = false;
}