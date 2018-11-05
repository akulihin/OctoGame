using System;
using System.Collections.Generic;
using OctoGame.Accounts.Users;

namespace OctoGame.Accounts.GameSpells
{
    public class GameSpellHandeling
    {
        private readonly IUserAccounts _accounts;

        public GameSpellHandeling(IUserAccounts accounts)
        {
            _accounts = accounts;
        }

        public double ArmorHandeling(double armPen, double arm, double dmg)
        {
            // TEST IT, maybe an error!!! ( unity test helps)
            double def = 0;
            if (Math.Ceiling(arm - armPen) == 1)
                def = 0.26;
            else if (Math.Ceiling(arm - armPen) == 2)
                def = 0.13;
            else if (Math.Ceiling(arm - armPen) == 3)
                def = 0.8;
            else if (Math.Ceiling(arm - armPen) == 4)
                def = 0.5;
            else if (Math.Ceiling(arm - armPen) == 5)
                def = 0.3;
            else if (Math.Ceiling(arm - armPen) == 6)
                def = 0.1;

            var final = dmg - dmg * def;

            return final;
        }

        public double ResistHandeling(int magPen, int magResist, double dmg)
        {
            double def = 0;
            if (magResist - magPen == 1)
                def = 0.26;
            else if (magResist - magPen == 2)
                def = 0.13;
            else if (magResist - magPen == 3)
                def = 0.8;
            else if (magResist - magPen == 4)
                def = 0.5;
            else if (magResist - magPen == 5)
                def = 0.3;
            else if (magResist - magPen == 6)
                def = 0.1;

            var final = dmg - dmg * def;

            return final;
        }

        public double CritHandeling(double agi, double dmg, AccountSettings account)
        {
            var rand = new Random();
            var randCrit = rand.Next(100);

            if (agi >= randCrit + 1)
            {
                dmg = dmg * account.CritDmg;
                return dmg;
            }

            return dmg;
        }

        public double DodgeHandeling(double agi, double dmg, AccountSettings account, AccountSettings enemy)
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
                enemy.Dodged = 1;
                _accounts.SaveAccounts(enemy.Id);
                return dmg;
            }

            return dmg;
        }

        public int DmgHealthHandeling(int dmgWhere, double dmg, AccountSettings myAccount, AccountSettings enemyAccount)
        {
            /*
             0 = Regular
             1 = To health
             2 = only to stamina
             */

            var status = 0;
            var userId = myAccount.Id;
            switch (dmgWhere)
            {
                case 0 when enemyAccount.Stamina > 0:
                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);


                    if (enemyAccount.Stamina < 0)
                    {
                        enemyAccount.Health += enemyAccount.Stamina;
                        enemyAccount.Stamina = 0;
                        _accounts.SaveAccounts(userId);
                    }

                    break;
                case 0:
                    if (enemyAccount.Stamina <= 0)
                    {
                        enemyAccount.Health -= Math.Ceiling(dmg);
                        _accounts.SaveAccounts(userId);

                    }

                    break;
                case 1:
                    enemyAccount.Health -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);
                    break;
                case 2:
                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    if (enemyAccount.Stamina < 0)
                        enemyAccount.Stamina = 0;
                    _accounts.SaveAccounts(userId);
                    break;
            }

            if (enemyAccount.Health <= 0)
            {
                enemyAccount.Health = 0;
                status = 1;
            }
            return status;
        }


        /*
         (ад ветка) Battle trans - дает дамаг от потеряннх своих хп. 1% к ад за 5% потерянных хп. 1008

(ад ветка) Мародер - пассивно каждый крит хилит 2% стамины.  1010
(ад ветка) All in - пропускает ход, увеливает 1 физ удар через 4 хода на 10% за каждый пропущенный ход (макс 40%) 1011
(ад ветка) Мастер фехтования - пассивно получает щит на стамину, который блокает физ урон (100% от ад)  1012

PROF - (ад ветка) Спартанское копье - активно кричит "Слабым здесь не место", кидая на следующий ход копье, нанося 100% от ад + 15% от вражеских потерянных хп. 1013
PROF - (ад ветка) Безумец - повышает урон на 10%, но теряет 10% стамины. 1014


         
         */



        public double AdSkills(ulong skillId, AccountSettings myAccount, AccountSettings enemyAccount, bool check)
        {
            double dmg = 0;
            switch (skillId)
            {
                //(ад ветка) Убийца гигантов = (вражеское хп / 100 *5) * (сила/20 +1) + ад/100*(100-сила)
                case 1001: 
                    dmg = enemyAccount.Health / 100 * 5 * (myAccount.Strength / 20 + 1) +
                          myAccount.AD_Stats / 100 * (100 - myAccount.Strength);
                    if( myAccount.SkillCooldowns == null )
                        myAccount.SkillCooldowns = new List<AccountSettings.Cooldown>();
                    if (!check)
                      myAccount.SkillCooldowns.Add( new AccountSettings.Cooldown(skillId, 8));
                    break;

                // (ад ветка) Острый топор - снижает вражеский армор на 2 уровня на 2 хода  || 1003 //Done?
                case 1003: 
                    if (enemyAccount.Debuff == null)
                        enemyAccount.Debuff = new List<AccountSettings.Cooldown>();
                    if (!check)
                        enemyAccount.Debuff.Add(new AccountSettings.Cooldown(skillId, 2));
                    break;

                // (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад. 1005
                case 1005: 
                    if (enemyAccount.DamageOnTimer == null)
                        enemyAccount.DamageOnTimer = new List<AccountSettings.DmgWithTimer>();
                    if (!check)
                        enemyAccount.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(2.28 * myAccount.AD_Stats, 2));
                    dmg = 2.28 * myAccount.AD_Stats;
                    break;

                // (ад ветка - ульта) Палач - добивает врага с 25% хп и ниже. (контрит перерождения) 1007
                case 1007: 
                    dmg = 0.25 >= enemyAccount.Health / enemyAccount.MaxHealth ? 99999999 : 1;
                    
                    //finish game!!! ( I will implement gameEnd() later)
                    break;

                // (ад ветка) Кулак богатыря - дамажит 20% от ад за каждые 10 силы. (кд 5 ходов) 1009
                case 1009:
                    dmg = (myAccount.AD_Stats * 0.2) * (myAccount.Strength / 10);
                    break;

                // (ад ветка-ульта) Замах ненависти - пропускает ход, следующих ходом наносит сокрушительный удар на 300% от ад и игнорирует 1 уровень физ защиты. 1015
                //    Дамаг считается за крит и не может быть увеличен крит уроном.
                case 1015:
                    dmg = myAccount.AD_Stats * 3;
                    if(myAccount.DamageOnTimer == null) myAccount.DamageOnTimer = new List<AccountSettings.DmgWithTimer>();
                        
                    if (myAccount.DebuffInTime == null) myAccount.DebuffInTime = new List<AccountSettings.DmgWithTimer>();
                    //  myAccount.Debuff.Add(); //поменяй дебаффы пидор, а то тан нихуя нет, ты что дурак??? юбля.... какой же ты тупой, прошлый я.
                    if (!check)
                        myAccount.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 1));
                    break;
                }

            _accounts.SaveAccounts(myAccount.Id);
            _accounts.SaveAccounts(enemyAccount.Id);
            return dmg;
        }

        public double DefdSkills(ulong skillId, AccountSettings account)
        {
            double dmg = 0;
            return dmg;
        }

        public double AgiSkills(ulong skillId, AccountSettings account)
        {
            double dmg = 0;
            return dmg;
        }

        public double ApSkills(ulong skillId, AccountSettings account)
        {
            double dmg = 0;
            return dmg;
        }

        public void ActivatePassive(AccountSettings account, ulong passiveId)
        {
            switch (passiveId)
            {
                case 1000:

                    break;
            }
        }
    }
}