[System.Serializable]
public class Relationship
{
    [System.Serializable]
    public struct CharRelationship
    {
        public int opinion;
        public int trust;
        public int fear;
        public FeudalStatus status;
        public int charID;
    };
    public CharRelationship charA;
    public CharRelationship charB;
    
    // You can add "flags" for medieval status
    public bool isAtWar = false;
    public bool isAllied = false;
    public bool owesFavor = false;
    public int loyalty;

    public bool IsVassal(CharacterData character)
    {
        if(charA.charID == character.GetInstanceID())
        {
            if(charA.status == FeudalStatus.Vassal)
            {
                return true;
            }
        }
        else
        {
            if(charB.status == FeudalStatus.Vassal)
            {
                return true;
            }
        }
        return false;
    }
}

public enum FeudalStatus { None, Liege, Vassal }