using UnityEngine;
using System.Linq;

public class MilitaryEngine : MonoBehaviour
{
    // This is a static or singleton-style method so you can call it from anywhere
    public int GetModifiedDefense(ArmyUnitData unit, Territory currentLoc)
    {
        int baseDef = unit.unitType.defensePower;
        if (currentLoc != null && currentLoc.currentBuildings.Any(b => b.effects.Any(e => e is ArmourEffect)))
        {
            var milEffects = currentLoc.currentBuildings.SelectMany(b => b.effects).OfType<IMilitaryEffect>();

            foreach (var effect in milEffects)
            {
                // Apply the bonus directly
                effect.ApplyMilitaryBonus(unit);
            }
        }

        return baseDef;
    }
}

// 1. Expanded Medieval Unit List
// You can use these to create your UnitType ScriptableObjects.

// Infantry: The Core
// Peasant Levy: Armed with pitchforks/scythes. High numbers, zero morale, terrible armor.

// Spearmen: The "Standard" militia. Good at holding a line and repelling horses.

// Pikemen vs. Halberdiers:

// Pikemen: Use 15-20ft pikes. They are a "Defensive Wall." Impossible to charge from the front, but they can't move quickly or fight if the enemy gets "inside" the pike length.

// Halberdiers: The "Swiss Army Knife." The axe blade chops armor, the hook pulls knights off horses, and the point stabs. They are better in a messy melee than Pikemen.

// Men-at-Arms (Professional Infantry):

// Foot Knights: Knights who chose to fight on foot. Heaviest armor.

// Voulgiers / Billmen: Heavy infantry using "Billhooks." Great at "opening" plate armor.

// Macemen / Axemen: High "Armor Piercing" damage. Better against plate than swords.

// Missile: Ranged Support
// Shortbowmen: Local hunters. Fast fire rate, low range, poor against armor.

// Longbowmen: Cultural specialists (English style). Massive range and high damage, but require a specific building (Archery Range) to represent the years of training needed.

// Crossbowmen:

// Light Crossbow: Fast reload, decent piercing.

// Pavise Crossbowman: Carries a large shield (Pavise) on their back. They turn around to reload safely behind the shield.

// Arbalestiers: Heavy crossbows. Slowest fire rate, but can punch through the thickest plate.

// Cavalry: The Shock Force
// Hobelars: Light scouts on small ponies. Used for raiding, chasing fleeing enemies, and scouting.

// Mounted Archers: Skirmishers who fire and retreat.

// Squire / Sergeant: "Medium" cavalry. Armored but not as elite as a Knight.

// Knight: The heavy tank. Destructive charge, high cost, requires "Sword School" or "Stables."

// 2. The Medieval Building List
// Here are buildings categorized by their mechanical impact on your game.

// Economic & Infrastructure
// Open Fields / Farms: Increases population growth and seasonal gold (food surplus).

// Market / Trade Square: Increases gold generation based on the number of neighboring territories.

// Water Mill / Windmill: Multiplier for farm income.

// Port / Wharf: Even if the water is small, these generate high gold and allow "Naval Levies" (armed transport boats).

// Quarry / Mine: Required to build high-tier stone buildings (Castles/Cathedrals) or provides a discount on construction.

// Military & Training
// Blacksmith (Smithy): Allows the recruitment of basic armored units (Spearmen).

// Armoury: A "Regional" building. Increases Defense stat for all units recruited in the entire Province.

// Archery Range: Unlocks Longbowmen and Crossbowmen.

// Stables: Unlocks light/medium cavalry.

// Sword School (Fencing Master): Unlocks professional Men-at-Arms and increases the "Prowess" of the local Lord's family.

// Barracks: Increases the "Max Soldier Cap" for the territory.

// Social & Research
// Monastery / Religious Temple: Increases "Respect" and "Loyalty." Monks act as early "Doctors" (faster casualty recovery) and "Scribes" (History logs).

// Scriptorium (Scholarly Place): This is your "Research" hub. In medieval times, this was where books were copied. It unlocks better "Administration" (lower tax waste) or "Siege Tech."

// Tavern / Inn: Increases "Dread" (if you're a tyrant) or "Opinion" from travelers. It's a hub for hiring Mercenaries.

// Manor House / Palace: Increases the "Influence" of the character governing the territory.

// 3. Visualizing the Building Hierarchy
// Since you have a town/city system, you can limit certain buildings to certain tiers.

// 4. New Mechanic Idea: "Guilds"
// In late medieval times, cities weren't just about buildings; they were about Guilds.

// If you build a "Mason's Guild," buildings in that territory are 20% cheaper.

// If you build a "Blacksmith's Guild," your Men-at-Arms gain an extra "Masterwork" buff.

// How to Implement "Research"
// Since you mentioned a "scholarly place," you could have a GlobalResearch script.

// Scriptorium building adds +5 Research Points per turn.

// Project: "Better Steel" (Costs 500 Points).

// Result: Once finished, all UnitType objects in your game get +2 Attack.

// Settlement Type,Unlocked via Building,Unit Name,Description
// Small Town,Town Hall Lvl 1,Peasant Levy,"Low morale, high numbers, farm tools."
// Big Town,Town Hall Lvl 2,Spear Militia,Disciplined rural infantry with shields.
// Big Town,Archery Range,Rural Archers,"Fast-firing, light skirmishers."
// City,Militia Guild,Heavy Billmen,Wealthy artisans with heavy polearms.
// City,City Watch,Crossbowmen,"Defensive specialists, armor-piercing."
// City,Arsenal / Barracks,Men-at-Arms,"The true professional ""retinue"" units."