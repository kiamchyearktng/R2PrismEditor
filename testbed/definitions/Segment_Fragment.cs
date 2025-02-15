using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prismeditor.definitions
{

    public class Segment(PropertyBag properties)
    {
        public static readonly ReadOnlyCollection<string> SegmentTypes = new(["Standard", "Fusion"]);
        public static readonly ReadOnlyCollection<string> LegsTypes = new(["Legendary"]);
        public static readonly ReadOnlyCollection<string> SegmentNames = new(["CriticalDamage", "WeakspotDamage", "RangedDamage", "MeleeDamage", "ModDamage", "SkillDamage", "ExplosiveDamage", "StatusDamage", "RangedCriticalChance", "MeleeCriticalChance", "ModCriticalChance", "SkillCriticalChance", "RangedFireRate", "FirearmChargeSpeed", "MeleeAttackSpeed", "HealthPercent", "HealthFlat", "HealthRegen", "HealingEfficacy", "GreyHealthRate", "StaminaPercent", "StaminaFlat", "DamageReduction", "ArmorPercent", "ArmorFlat", "ShieldPercent", "ShieldDuration", "EvadeSpeed", "EvadeDistance", "ReviveSpeed", "MovementSpeed", "WeaponSpread", "IdealRange", "ReloadSpeed", "SwapSpeed", "AmmoReserves", "ProjectileSpeed", "HeatReduction", "CastSpeed", "ModGeneration", "ModDuration", "SkillCooldown", "SkillDuration", "ConsumableSpeed", "ConsumableDuration"]);
        public static readonly ReadOnlyCollection<string> FusionNames = new(["WeakspotDamageCriticalDamage", "ModCriticalSkillCritical", "MeleeDamageMeleeSpeed", "MeleeCriticalEvadeSpeed", "MeleeSpeedStaminaFlat", "FireRateReloadSpeed", "RangedDamageIdealRange", "RangedCriticalAmmoReserves", "ModDamageModGeneration", "HealthPercentStaminaPercent", "DamageReductionArmorPercent", "ShieldEfficacyArmorFlat", "HealthPercentGreyHealthRate", "HealthRegenSkillCooldown", "MovementSpeedEvadeSpeed", "HealingEfficacyUseSpeed", "ModDurationSkillDuration", "WeaponSpreadSwapSpeed", "CastSpeedUseSpeed", "RangedDamageMeleeDamage", "ExplosiveDamageDamageReduction", "FirearmChargeSpeedHeatReduction", "ReviveSpeedHealingEfficacy"]);
        public static readonly ReadOnlyCollection<string> LegsNames = new(["MasterKiller", "Immovable", "FullHearted", "Allegiance", "Outlaw", "Soulmate", "HeavyDrinker", "BoundlessEnergy", "PowerTrip", "FleetFooted", "LuckOfTheDevil", "Sadistic", "Unbridled", "DefensiveMeasures", "GodTear", "SpeedDemon", "Reverberation", "Hyperactive", "PrimeTime", "Exhausted", "Overpowered", "Gigantic", "Traitor", "DarkOmen", "Altruistic", "Unbreakable", "InsultToInjury", "Brutality", "WreckingBall", "SteelPlating", "CriticalSituation", "SharpShooter", "JackOfAllTrades", "Vaccinated", "PeakConditioning", "Physician", "Impervious", "Spectrum", "ArtfulDodger", "PowerFantasy", "SizeMatters", "Bodyguard"]);

        internal PropertyBag properties = properties;

        public string Name {
            get { return (properties["RowName"].Value as FName)!.Name; }
            set { (properties["RowName"].Value as FName)!.Name = value; }
        }

        public int Level {
            get { return properties["Level"].Get<int>(); }
            // no set
        }
    }

    public class Fragment(PropertyBag properties)
    {
        internal PropertyBag properties = properties;

        public string Name {
            get { return (properties["RowName"].Value as FName)!.Name; }
            set { (properties["RowName"].Value as FName)!.Name = value; }
        }

        public int Level {
            get { return properties["FedLevel"].Get<int>(); }
            set { properties["FedLevel"].Value = value; }
        }
    }
}
