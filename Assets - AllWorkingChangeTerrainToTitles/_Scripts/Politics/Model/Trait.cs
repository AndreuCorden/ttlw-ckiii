using UnityEngine;

[CreateAssetMenu(fileName = "New Trait", menuName = "Social/Trait")]
public class Trait : ScriptableObject, IDescribable
{
    public string traitName;
    public TraitEnum traitType; // Link to your enum
    public ReligionEnum religionType; // Only used if this is a religion
    [TextArea] public string description;
    public Sprite icon;

    [Header("Stat Modifiers")]
    public int loyaltyMod;
    public int respectMod;
    public int dreadMod;
    public int prowessMod;

    // You can even add combat buffs here later!
    public float healthMultiplier = 1.0f;

    public string GetName()
    {
        return traitName;
    }

    public string GetDescription()
    {
        return $"Prowess: {(prowessMod >= 0 ? "+" : "")}{prowessMod}\n" +
               $"Loyalty: {(loyaltyMod >= 0 ? "+" : "")}{loyaltyMod}";
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}