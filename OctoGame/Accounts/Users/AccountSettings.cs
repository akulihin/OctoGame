using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OctoGame.Accounts.Users
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    public class AccountSettings
    {

        /// <summary>
        /// General Information
        /// </summary>
        public string UserName { get; set; }
        public ulong Id { get; set; }
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
        public double ArmPen { get; set; } // 0 LVL
        public double MagPen { get; set; } // 0 LVL      
        public double OnHit { get; set; }
        public int IsCrit { get; set; }
        public string CurrentLogString { get; set; }
        public List<Cooldown> PassiveList { get; set; }
        public List<Cooldown> SkillCooldowns { get; set; }
        public List<Cooldown> Debuff { get; set; }
        public List<Cooldown> Buff { get; set; }
        public double PrecentBonusDmg { get; set; }
        public double NumberBonusDmg { get; set; }
        public List<DmgWithTimer> DamageOnTimer { get; set; }
        public List<DmgWithTimer> DebuffInTime { get; set; }
        public int Dodged { get; set; }
       
        // 1 - AD
        // 2 - DEF
        // 3 - AGI
        // 4 - AP

        /// <summary>
        /// All Artifact Entities
        /// </summary>


        public class Cooldown
        {
            public ulong skillId;
            public int cooldown;

            public Cooldown(ulong skillId, int cooldown)
            {
                this.skillId = skillId;
                this.cooldown = cooldown;
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
            public int timer;

            public DmgWithTimer(double dmg, int timer)
            {
                this.dmg = dmg;
                this.timer = timer;
            }
        }
    }
}