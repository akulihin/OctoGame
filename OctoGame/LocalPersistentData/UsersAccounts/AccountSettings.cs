using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OctoGame.LocalPersistentData.UsersAccounts
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    public class AccountSettings
    {

        /// <summary>
        /// General Information
        /// </summary>
        public string DiscordUserName { get; set; }
        public ulong DiscordId { get; set; }
        public string MyPrefix { get; set; }
        public string MyLanguage { get; set; }

        /// <summary>
        /// Your Octopus
        /// </summary>
        public string OctoName { get; set; }
        public uint OctoLvL { get; set; } //1 lvl ( 0 lvl 1lvl 2 lvl)
        public string OctoInfo { get; set; }
        public string OctoAvatar { get; set; }
        public List<ArtifactEntities> Inventory { get; set; }
        public List<ArtifactEntities> OctoItems { get; set; }
        public string AD_Tree { get; set; }
        public string DEF_Tree { get; set; }
        public string AG_Tree { get; set; }
        public string AP_Tree { get; set; }
        public string Passives { get; set; }

        /// <summary>
        /// Fight:
        /// </summary>
        
        public int Turn { get; set; }
        public double Round { get; set; }
        public ulong CurrentEnemy { get; set; }
        public int MoveListPage { get; set; } 
        public int PlayingStatus { get; set; }   // 0 = not playing | 1 = play


        public double Strength { get; set; } // 20
        // ReSharper disable once InconsistentNaming

        public double AD_Stats { get; set; } //0
        public double Base_AD_Stats { get; set; } //0
        public double Bonus_AD_Stats { get; set; } //0
        // ReSharper disable once InconsistentNaming
        public double AP_Stats { get; set; } //0
        // ReSharper disable once InconsistentNaming
        public double AG_Stats { get; set; } //0
        public double CritDmg { get; set; }
        public double CritChance { get; set; }
        public double DodgeChance { get; set; }
        public double Armor { get; set; } //1 LVL (1-6)
        public double Resist { get; set; } //1 LVL
        public double Health { get; set; } //100
        public double MaxHealth { get; set; }
        public double Stamina { get; set; } //200    
        public double MaxStamina { get; set; } //200    
        public double ArmPen { get; set; } // 0 LVL
        public double MagPen { get; set; } // 0 LVL      
        public double OnHit { get; set; }
        public bool IsCrit { get; set; }
        public string CurrentLogString { get; set; }
        public List<CooldownClass> PassiveList { get; set; }
        public List<CooldownClass> SkillCooldowns { get; set; }
        public List<DebuffClass> Debuff { get; set; }
        public List<CooldownClass> Buff { get; set; }
        public double PrecentBonusDmg { get; set; }
        public double NumberBonusDmg { get; set; }
        public List<DmgWithTimer> DamageOnTimer { get; set; }
        public List<DmgWithTimer> DebuffInTime { get; set; }
        public int Dodged { get; set; }
        public bool FirstHit { get; set; }
        public int MessageIdInList { get; set; }
        public double dmgDealedLastTime { get; set; }
        public double PhysShield { get; set; }
        public double MagShield { get; set; }
        public int HowManyTimesCrited { get; set; }
        public List<StatsForTimeClass> StatsForTime {get; set; }
        public double LifeStealPrec { get; set; }


        // 1 - AD
        // 2 - DEF
        // 3 - AGI
        // 4 - AP

        /// <summary>
        /// All Artifact Entities
        /// </summary>


        public class CooldownClass
        {
            public ulong skillId;
            public int cooldown;

            public CooldownClass(ulong skillId, int cooldown)
            {
                this.skillId = skillId;
                this.cooldown = cooldown*2;
            }
        }

        public class DebuffClass
        {
            public ulong skillId;
            public int cooldown;
            public bool activated;

            public DebuffClass(ulong skillId, int cooldown, bool activated)
            {
                this.skillId = skillId;
                this.cooldown = cooldown*2;
                this.activated = activated;
            }
        }


        [SuppressMessage("ReSharper", "InconsistentNaming")] 
        public class ArtifactEntities
        {
            public ulong artId;
            public double Health;
            public double AD_Stats;
            public double AP_Stats;
            public double Armor;
            public double Resist;
            public double AG_Stats;
            public double Stamina;
            public double Strenght;
            public double CritDmg;
            public double ArmPen;
            public double MagPen;
            public double OnHit;
        }

        public class DmgWithTimer
        {
            public double dmg;
            public int dmgType;
            public int timer;

            public DmgWithTimer(double dmg,  int dmgType, int timer)
            {
                this.dmg = dmg;
                this.dmgType = dmgType;
                this.timer = timer*2;
            }
        }

        public class StatsForTimeClass
        {
            public double AD_STATS;
            public int timer;

            public StatsForTimeClass(double AD_STATS, int timer)
            {
                this.AD_STATS = AD_STATS;
                this.timer = timer;
            }
        }
    }
}

/*
CREATE TABLE UserAccounts(DiscordUserName VARCHAR(50), userId decimal (20, 0), MyPrefix VARCHAR(50), MyLanguage VARCHAR(50), 
OctoName VARCHAR(50), OctoLvL bigint, OctoInfo VARCHAR(2000), OctoAvatar VARCHAR(100), AD_Tree VARCHAR(1000),
DEF_Tree VARCHAR(1000), AG_Tree VARCHAR(1000), AP_Tree VARCHAR(1000), Passives VARCHAR(1000),
Turn INTEGER, Round float, CurrentEnemy decimal (20, 0), MoveListPage INTEGER, PlayingStatus INTEGER,
Strength decimal (20, 0), Bonus_AD_Stats decimal (20, 0), Base_AD_Stats decimal (20, 0), AD_Stats decimal (20, 0), 
AP_Stats decimal (20, 0), AG_Stats decimal (20, 0), CritDmg decimal (20, 0), CritChance decimal (20, 0), 
DodgeChance decimal (20, 0), Armor decimal (20, 0), Resist decimal (20, 0), Health decimal (20, 0), 
MaxHealth decimal (20, 0), Stamina decimal (20, 0), MaxStamina decimal (20, 0), ArmPen decimal (20, 0), 
MagPen decimal (20, 0), OnHit decimal (20, 0), PrecentBonusDmg decimal (20, 0),  NumberBonusDmg decimal (20, 0),
IsCrit INTEGER, Dodged INTEGER, CurrentLogString VARCHAR(10000));
*/