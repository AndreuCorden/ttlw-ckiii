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
        public string charID;
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
        if (charA.charID == character.characterId)
        {
            if (charA.status == FeudalStatus.Vassal)
            {
                return true;
            }
        }
        else
        {
            if (charB.status == FeudalStatus.Vassal)
            {
                return true;
            }
        }
        return false;
    }

    public RelationshipSaveData Export()
    {
        return new RelationshipSaveData
        {
            charAID = charA.charID.ToString(), // Change this to a String ID later!
            charBID = charB.charID.ToString(),
            opinionAtoB = charA.opinion,
            opinionBtoA = charB.opinion,
            trustA = charA.trust,
            trustB = charB.trust,
            isAtWar = this.isAtWar,
            isAllied = this.isAllied,
            loyalty = this.loyalty,
            statusA = charA.status,
            statusB = charB.status
        };
    }
}

public enum FeudalStatus { None, Liege, Vassal }