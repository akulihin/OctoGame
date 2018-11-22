using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.BonusDmgHandling;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;

namespace OctoGame.OctoGame.SpellHandling.ActiveSkills
{
    public class AttackDamageActiveTree
    {
        private readonly IUserAccounts _accounts;
        private readonly ArmorReduction _armorReduction;
        private readonly MagicReduction _magicReduction;
        private readonly Dodge _dodge;
        private readonly Crit _crit;
   

        public AttackDamageActiveTree(IUserAccounts accounts, ArmorReduction armorReduction, MagicReduction magicReduction, Dodge dodge, Crit crit)
        {
            _accounts = accounts;
            _armorReduction = armorReduction;
            _magicReduction = magicReduction;
            _dodge = dodge;
            _crit = crit;
        }

        /*
1 столб
1000 (ад ветка) Выжидание - пассивно первая атака за бой будет усилена на 20%  
1001 (ад ветка) Убийца гигантов - дамажит по макс хп врага и скейлится 5% + 5% за каждые 20 силы (кд 10 ходов) формула ДАМАГ = (вражеское хп / 100 *5) * (сила/20 +1) + ад/100*(100 минус сила)
1002 (ад ветка) Меткость - пассивно если враг увернулся то следующие 2 хода он не может увернутся. (кд 8 ходов)
1003 (ад ветка) Острый топор - снижает вражеский армор на 2 уровня на 2 хода 
1004 (ад ветка) Твердый меч - пассивно увеличивает ад на 8% от вражеской текущей выносливости. 

1005 (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад.
1006 (ад ветка) Без изъяна - пассивно дает 1 армор или резист, если на него не куплено ни одной вещи.

1007 (ад ветка - ульта) Палач - добивает врага с 25% хп и ниже. (контрит перерождения) 
     
         */

        public double AttackDamageActiveSkills(ulong skillId, AccountSettings myAccount, AccountSettings enemyAccount, bool check)
        {
            double dmg = 0;
            switch (skillId)
            {
                //(ад ветка) Убийца гигантов = (вражеское хп / 100 *5) * (сила/20 +1) + ад/100*(100-сила)
                case 1001:
                    dmg = enemyAccount.Health / 100 * 5 * (myAccount.Strength / 20 + 1) +
                          myAccount.AD_Stats / 100 * (100 - myAccount.Strength);

                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 6));
                    break;

                // (ад ветка) Острый топор - снижает вражеский армор на 2 уровня на 2 хода, 4 хода КД  || 1003 //Done?
                case 1003:

                    if (!check)
                    {
                        enemyAccount.Debuff.Add(new AccountSettings.DebuffClass(skillId, 4, false));
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 4));
                    }

                    break;

                // (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад. 1005
                case 1005:

                    dmg = 2.28 * myAccount.AD_Stats;
                    if (!check)
                    {
                        if (myAccount.Buff.Any(x => x.skillId == 1000) && myAccount.FirstHit)
                        {
                            dmg = dmg * (1 + myAccount.PrecentBonusDmg);
                            myAccount.FirstHit = false;
                        }

                        enemyAccount.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 0, 1));
                        dmg = 0;
                    }
                                     
                    break;

                // (ад ветка - ульта) Палач - добивает врага с 25% хп и ниже. (контрит перерождения) 1007
                case 1007:
                    dmg = 0.25 >= enemyAccount.Health / enemyAccount.MaxHealth ? 99999999 : 1;
                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 10));
                    //finish game!!! ( I will implement gameEnd() later)
                    break;

                // (ад ветка) Кулак богатыря - дамажит 20% от ад за каждые 10 силы. (кд 5 ходов) 1009
                case 1009:
                    dmg = myAccount.AD_Stats * 0.2 * (myAccount.Strength / 10);
                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 5));
                    break;

                // (ад ветка-ульта) Замах ненависти - пропускает ход, следующих ходом наносит сокрушительный удар на 300% от ад и игнорирует 1 уровень физ защиты. 1015
                //    Дамаг считается за крит и не может быть увеличен крит уроном.
                case 1015:
                    dmg = myAccount.AD_Stats * 3;
                    if (!check)
                    {
                        if (myAccount.Buff.Any(x => x.skillId == 1000) && myAccount.FirstHit)
                        {
                            dmg = dmg * (1 + myAccount.PrecentBonusDmg);
                            myAccount.FirstHit = false;
                        }

                        enemyAccount.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 0, 1));
                        dmg = 0;
                    }

                    break;

                //1017 (ад ветка) Внезапный выпад - дамажит 120% от ад (нельзя увернуться) (кд 7 ходов) 
                case 1017:
                    dmg = myAccount.AD_Stats * 1.2;

                    if(!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 7));

                    break;

               // 1019(ад ветка) Решительность - повышает ад на 50 % на следующий ход.
                case 1019:
                    myAccount.Bonus_AD_Stats += myAccount.AD_Stats * 0.5;
                    myAccount.StatsForTime.Add(new AccountSettings.StatsForTimeClass(myAccount.AD_Stats * 0.5, 2));
                    break;


  //1021 (ад ветка) Кромсатель - наносит 3 удара 30%+50%+70% от ад.  пропуская следующих ход, но получая онхит на 5 ходов (20% уровня + вражеский уровень армора * 10% от уровня)
                    // hvatit ebu davat
                case 2021:

                    break;


                //1023 (ад ветка - ульта) Кровавая месть - дает 50% физ лайвстила на 5 ходов 
                case 2023:
                    myAccount.LifeStealPrec = 0.5;
                    break;
            }



            if (myAccount.Buff.Any(x => x.skillId == 1000) && myAccount.FirstHit)
                dmg = dmg * (1 + myAccount.PrecentBonusDmg);

            if (!check && dmg >= 1) myAccount.FirstHit = false;

            _accounts.SaveAccounts(myAccount.DiscordId);
            _accounts.SaveAccounts(enemyAccount.DiscordId);

            // Я два раза армор использую*?????? Проверь в DmgHandling пидор
               // арсор не то чем кажется 
            dmg = _armorReduction.ArmorHandling(myAccount.ArmPen, enemyAccount.Armor, dmg);
            return dmg;
        }
    }
}