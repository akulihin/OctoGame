using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;

namespace OctoGame.OctoGame.SpellHandling.ActiveSkills
{
    public class AttackDamageActiveTree
    {
        private readonly IUserAccounts _accounts;
        private readonly ArmorReduction _armorReduction;


        public AttackDamageActiveTree(IUserAccounts accounts, ArmorReduction armorReduction)
        {
            _accounts = accounts;
            _armorReduction = armorReduction;
      
        }

        public double AttackDamageActiveSkills(ulong skillId, AccountSettings myAccount, AccountSettings enemyAccount, bool check)
        {
            double dmg = 0;
            switch (skillId)
            {
                //1096 (ад ветка) - 100% от АД

                //Done
                case 1096:
                    dmg = myAccount.AttackPower_Stats;
                    break;
                //(ад ветка) Убийца гигантов = (вражеское хп / 100 *5) * (сила/20 +1) + ад/100*(100-сила)

                //Done
                case 1001:
                    dmg = enemyAccount.Health / 100 * 5 * (myAccount.Strength / 20 + 1) +
                          myAccount.AttackPower_Stats / 100 * (100 - myAccount.Strength);

                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 6));
                    break;

                // (ад ветка) Острый топор - снижает вражеский армор на 2 уровня на 2 хода, 4 хода КД  || 1003

                // Done
                case 1003:

                    if (!check)
                    {
                        enemyAccount.InstantDeBuff.Add(new AccountSettings.InstantBuffClass(skillId, 2, false));
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 4));
                    }

                    break;

                // (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад. 1005           
                // -Пропускает ход1, может ходить на ход2, и тратит ход3 на удар

                //Done
                case 1005:

                    dmg = 2.28 * myAccount.AttackPower_Stats;

                    if (!check)
                    {

                        enemyAccount.InstantDeBuff.Add(new AccountSettings.InstantBuffClass(skillId, 1, false));
                        dmg = 0;
                    }

                    break;

                // (ад ветка - ульта) Палач - добивает врага с 25% хп и ниже. (контрит перерождения) 1007

                //DONE
                case 1007:
                    dmg = 0.25 >= enemyAccount.Health / enemyAccount.MaxHealth ? 99999999.00 : 1;
                    if (!check)
                    {
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 10));
                    }


                    break;

                // (ад ветка) Кулак богатыря - дамажит 20% от ад за каждые 10 силы. (кд 5 ходов) 1009

                //DONE
                case 1009:
                    dmg = myAccount.AttackPower_Stats * 0.2 * (myAccount.Strength / 10);
                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 5));
                    break;

                // (ад ветка-ульта) Замах ненависти - пропускает ход, следующих ходом наносит сокрушительный удар на 300% от ад и игнорирует 1 уровень физ защиты. 1015
                //    Дамаг считается за крит и не может быть увеличен крит уроном.

                //TODO this
                case 1015:
                    dmg = myAccount.AttackPower_Stats * 3;
                    if (!check)
                    {
                        if (myAccount.InstantBuff.Any(x => x.skillId == 1000) && myAccount.IsFirstHit)
                        {
                            dmg = dmg * (1 + myAccount.PrecentBonusDmg);
                            myAccount.IsFirstHit = false;
                        }

                        enemyAccount.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 0, 1));
                        dmg = 0;
                    }
                    // -Здесь должна быть +1 армор пенетры на удар. так же этот удар не критует, но считается критом для игры (есть там пара скиллов на этой механнике)
                    break;

                //1017 (ад ветка) Внезапный выпад - дамажит 120% от ад (нельзя увернуться) (кд 7 ходов) 

                //DONE
                case 1017:
                    dmg = myAccount.AttackPower_Stats * 1.2;

                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 7));
                    // -Нельзя увернутся от этого удара
                    break;

                // 1019(ад ветка) Решительность - повышает ад на 50 % на следующий ход.

                //DONE
                case 1019:
                    if (!check)
                    {
                        myAccount.AttackPower_Stats += myAccount.AttackPower_Stats * 0.5;
                        myAccount.StatsForTime.Add(
                            new AccountSettings.StatsForTimeClass(myAccount.AttackPower_Stats * 0.5, 2));
                    }

                    break;


                //1021 (ад ветка) Кромсатель - наносит 3 удара 30%+50%+70% от ад.  пропуская следующих ход, но получая онхит на 5 ходов (20% уровня + вражеский уровень армора * 10% от уровня)

                //TODO this
                case 1021:

                    break;


                //1023 (ад ветка - ульта) Кровавая месть - дает 50% физ лайвстила на 5 ходов 

                //DONE
                case 1023:
                    if (!check)
                        enemyAccount.InstantDeBuff.Add(new AccountSettings.InstantBuffClass(skillId, 5, false));
  
                    break;

            }



            if (check)
                if (myAccount.InstantBuff.Any(x => x.skillId == 1000) && myAccount.IsFirstHit)
                    dmg = dmg * (1 + myAccount.PrecentBonusDmg);
            if (check)
                dmg = _armorReduction.ArmorHandling(myAccount.PhysicalPenetration, enemyAccount.PhysicalResistance, dmg);


            _accounts.SaveAccounts(myAccount.DiscordId);
            _accounts.SaveAccounts(enemyAccount.DiscordId);


            return dmg;

            //if(check) this is only to show the dmg for display
        }
    }
}
