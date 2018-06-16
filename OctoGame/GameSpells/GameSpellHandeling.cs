using System;
using OctoBot.Games.OctoGame.GameUsers;

namespace OctoBot.Games.OctoGame.GameSpells
{
    public class GameSpellHandeling
    {

        public static double ArmorHandeling(int armPen, int arm, double dmg)
        {
            double def = 0;
            if(arm - armPen == 1)
            {
                def = 0.24;
            }
            else if (arm - armPen == 2)
            {
                def = 0.44;
            }
            else if (arm - armPen == 3)
            {
                def = 0.6;
            }
            else if (arm - armPen == 4)
            {
                def = 0.72;
            }
            else if (arm - armPen == 5)
            {
                def = 0.8;
            }
            else if (arm - armPen == 6)
            {
                def = 0.84;
            }

            var final = (dmg - dmg * def);

            return final;
        }

        public static double ResistHandeling(int magPen, int magResist, double dmg)
        {
            double def = 0;
            if(magResist - magPen == 1)
            {
                def = 0.24;
            }
            else if (magResist - magPen == 2)
            {
                def = 0.44;
            }
            else if (magResist - magPen == 3)
            {
                def = 0.6;
            }
            else if (magResist - magPen == 4)
            {
                def = 0.72;
            }
            else if (magResist - magPen == 5)
            {
                def = 0.8;
            }
            else if (magResist - magPen == 6)
            {
                def = 0.84;
            }

            var final = (dmg - dmg * def);

            return final;
        }

        public static double CritHandeling(int agi, double dmg, GameAccountSettings account)
        {
            var rand = new Random();
            var randCrit = rand.Next(100);
         
            if (agi >= randCrit+1)
            {
                dmg = dmg * account.CurrentOctopusFighterCritDmg;
                return dmg;

            }

            return dmg;
        }
        public static double DodgeHandeling(int agi, double dmg, GameAccountSettings account)
        {
            var rand = new Random();
            var randDodge = rand.Next(100);
            if (agi > 1)
            {
                if (agi - 1 >= randDodge + 1)
                {
                    dmg = 0;
                    return dmg;

                }
                return dmg;
            }

            if (agi >= randDodge + 1)
            {
                dmg = 0;
                return dmg;

            }
            return dmg;
        }

        public static int DmgHealthHandeling(int dmgWhere, double dmg, GameAccountSettings account)
        {

              /*
               0 = Regular
               1 = To health
               2 = only to stamina
               */
            var status = 0;

            if (dmgWhere == 0)
            {
                if (account.CurrentEnemyStamina > 0)
                {
                    account.CurrentEnemyStamina -= Math.Ceiling(dmg);
                    GameUserAccounts.SaveAccounts();

                                       

                    if (account.CurrentEnemyStamina < 0)
                    {
                        account.CurrentEnemyHealth += account.CurrentEnemyStamina;
                        account.CurrentEnemyStamina = 0;
                        GameUserAccounts.SaveAccounts();
                       
                    }
                
                }

                else if (account.CurrentEnemyStamina <= 0)
                {

                    account.CurrentEnemyHealth -= Math.Ceiling(dmg);
                    GameUserAccounts.SaveAccounts();
                                      
                    if (account.CurrentEnemyHealth <= 0)
                    {
                        status = 1;
                        return status;
                    
                    }
                }

            } 
            else if (dmgWhere == 1)
            {
                account.CurrentEnemyHealth -= Math.Ceiling(dmg);
                GameUserAccounts.SaveAccounts();
                                      
                if (account.CurrentEnemyHealth <= 0)
                {
                    status = 1;
                    return status;

                }
               
            }
            else if (dmgWhere == 2)
            {
                account.CurrentEnemyStamina -= Math.Ceiling(dmg);
                if (account.CurrentEnemyStamina < 0)
                    account.CurrentEnemyStamina = 0;
                GameUserAccounts.SaveAccounts();
            }

            return status;
        }


        public static double AdSkills(ulong skillId, GameAccountSettings account)
        {
            double dmg = 0;
            
            
            if (skillId == 1001 || skillId == 1000)
            {
                //var skill = SpellUserAccounts.GetAccount(skillId);

               if (account.CurrentEnemyStamina <= 0)
                    dmg = 10009;
               else
               {
                   dmg = 5;
               }
                
               dmg = ArmorHandeling(account.CurrentOctopusFighterArmPen, account.CurrentEnemyArmor, dmg);

              
                return dmg;
            }
            
            return dmg;
        }


        public static double DefdSkills(ulong skillId, GameAccountSettings account)
        {
            double dmg = 0;
            return dmg;
        }

        public static double AgiSkills(ulong skillId, GameAccountSettings account)
        {
            double dmg = 0;
            return dmg;

        }

        public static double ApSkills(ulong skillId, GameAccountSettings account)
        {
            double dmg = 0;
            return dmg;
        }


    }
}







