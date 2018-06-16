using System.Collections.Generic;

namespace OctoBot.Games.OctoGame.GameUsers
{
    public class GameAccountSettings
    {

        public string UserName { get; set; }
        public ulong Id { get; set; }
        ///////////OctoGame////////////////
        public int Turn { get; set; }
        public int MoveListPage { get; set; }

        public int OctopusFightPlayingStatus {get; set; }

        public string OctopusFighterName{ get; set; }
        public string OctopusFighterInfo { get; set; }


        
        
         public int OctopusFighterStrength{ get; set; }  // 20
         public int OctopusFighterAd { get; set; }    //0
         public int OctopusFighterAp { get; set; }    //0
         public int OctopusFighterAgility { get; set; }  //0
         public double OctopusFighterCritDmg { get; set; } 
         public int OctopusFighterCritChance { get; set; } 
         public int OctopusFighterDodge{ get; set; } 
         public int OctopusFighterArmor { get; set; } //1 LVL
         public int OctopusFighterMagicResist { get; set; } //1 LVL
         public double OctopusFighterHealth { get; set; } //100
         public double OctopusFighterStamina { get; set; } //200    
         public int OctopusFighterArmPen { get; set; } // 0 LVL
         public int OctopusFighterMagPen { get; set; } // 0 LVL
         public int OctopusFighterLvl {get; set; } //1 lvl ( 0 lvl 1lvl 2 lvl)
         public double OctopusFighterLifeSteal {get; set; }
         
         public string OctopusFighterSkillSetAd { get; set; }
         public string OctopusFighterSkillSetDef { get; set; }
         public string OctopusFighterSkillSetAgi { get; set; }
         public string OctopusFighterSkillSetAp { get; set; }
         public List<OctopusFighterStats> OctopusFighterList { get; internal set; } = new List<OctopusFighterStats>();
         


        
        public int CurrentOctopusFighterStrength{ get; set; }  // 20
        public int CurrentOctopusFighterAd { get; set; }    //0
        public int CurrentOctopusFighterAp { get; set; }    //0
        public int CurrentOctopusFighterAgility { get; set; }  //0
        public double CurrentOctopusFighterCritDmg { get; set; } 
        public int  CurrentOctopusFighterCritChance { get; set; } 
        public int CurrentOctopusAbilityToCrit { get; set; } 
        public int CurrentOctopusFighterDodge{ get; set; } 
        public int CurrentOctopusFighterArmor { get; set; } //1 LVL
        public int CurrentOctopusFighterMagicResist { get; set; } //1 LVL
        public double CurrentOctopusFighterHealth { get; set; } //100
        public double CurrentOctopusFighterStamina { get; set; } //200    
        public int CurrentOctopusFighterArmPen { get; set; } // 0 LVL
        public int CurrentOctopusFighterMagPen { get; set; } // 0 LVL
        public int CurrentOctopusFighterLvl {get; set; } //1 lvl ( 0 lvl 1lvl 2 lvl)
        public double CurrentOctopusFighterLifeSteal {get; set; }

        public string CurrentOctopusFighterSkillSetAd { get; set; }
        public string CurrentOctopusFighterSkillSetDef { get; set; }
        public string CurrentOctopusFighterSkillSetAgi { get; set; }
        public string CurrentOctopusFighterSkillSetAp { get; set; }
        public List<OctopusFighterStats> CurrentOctopusFighterList { get; internal set; } = new List<OctopusFighterStats>();




        public string CurrentEnemyName{ get; set; }
        public int CurrentEnemyStrength{ get; set; }  
        public int CurrentEnemyAd { get; set; }    
        public int CurrentEnemyAp { get; set; }    
        public int CurrentEnemyAgility { get; set; } 
        public double CurrentEnemyCritDmg { get; set; } 
        public int CurrentEnemyDodge{ get; set; } 
        public int CurrentEnemyArmor { get; set; } 
        public int CurrentEnemyMagicResist { get; set; }
        public double CurrentEnemyHealth { get; set; } 
        public double CurrentEnemyStamina { get; set; }   
        public int CurrentEnemyArmPen { get; set; }
        public int CurrentEnemyMagPen { get; set; } 
        public int CurrentEnemyLvl {get; set; } 
        public double CurrentEnemyLifeSteal {get; set; }
       
        public string CurrentEnemySkillSetAd { get; set; }
        public string CurrentEnemySkillSetDef { get; set; }
        public string CurrentEnemySkillSetAgi { get; set; }
        public string CurrentEnemySkillSetAp { get; set; }
        
        public string CurrentLogString{ get; set; }
        public List<OctopusFighterStats>CurrentEnemyList { get; internal set; } = new List<OctopusFighterStats>();
        // 1 - AD
        // 2 - DEF
        // 3 - AGI
        // 4 - AP

       


      

        public struct OctopusFighterStats
        {
            public string Skills;
            public string Artifacts;
           

            public OctopusFighterStats(string skills, string artifacts)
            {
                Skills = skills;
                Artifacts = artifacts;
            }
        }


    }
}
