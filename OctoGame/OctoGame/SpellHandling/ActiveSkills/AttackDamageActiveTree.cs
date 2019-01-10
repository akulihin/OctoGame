
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;

namespace OctoGame.OctoGame.SpellHandling.ActiveSkills
{
    public sealed class AttackDamageActiveTree : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly UserAccounts _accounts;
        private readonly ArmorReduction _armorReduction;
  


        public AttackDamageActiveTree(UserAccounts accounts, ArmorReduction armorReduction)
        {
            _accounts = accounts;
            _armorReduction = armorReduction;
        }

        public double AttackDamageActiveSkills(ulong skillId, AccountSettings myAccount, AccountSettings enemyAccount,
            bool check)
        {
            double dmg = 0;
            switch (skillId)
            {
                //1096 (ад ветка) - 100% от АД

                //Done Checked
                case 1096:
                    dmg = myAccount.AttackPower_Stats;
                    break;
                //Атака(100% - Сила) + (5% от вражеских HP +5% за каждые 20 Силы). (Активируется только по HP)",

                //Done Checked
                case 1001:
                    var minus = myAccount.Strength;
                    if (minus > 100)
                        minus = 100;

                    dmg = ((100 - minus) /100 * myAccount.AttackPower_Stats) +
                          enemyAccount.Health * (myAccount.Strength / 20 * 5 / 100 + 0.05);

                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 6));
                    break;

                // (ад ветка) Острый топор - снижает вражеский армор на 2 уровня на 2 хода, 4 хода КД  || 1003

                // Done Checked
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
                    if (!check) myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 10));
           //TODO: I can use this with LAmar        private readonly Scope _upd;   _upd.GetInstance<UpdateFightPage>().UpdateIfWinOrContinue(1, myAccount.DiscordId, myAccount.MessageIdInList);
                

                    break;

                // (ад ветка) Кулак богатыря - дамажит 20% от ад за каждые 10 силы. (кд 5 ходов) 1009

                //DONE
                case 1009:
                    dmg = myAccount.AttackPower_Stats * 0.2 * (myAccount.Strength / 10);
                    if (!check)
                        myAccount.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 5));
                    break;

                //(ад ветка) All in - пропускает ход, увеличивает физический урон на один ход через 4 хода. на 10% за каждый пропущенный ход (макс 40%)

                //Done
                case 1011:
                    myAccount.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 4, false));
                    break;
//1013 (ад ветка) Спартанское копье - активно кричит "Слабым здесь не место", кидая на следующий ход копье, нанося 100% от ад + 15% от вражеских потерянных хп.

                //DONE
                case 1013:
                    myAccount.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 1, false));
                    break;
                // (ад ветка-ульта) Замах ненависти - пропускает ход, следующих ходом наносит сокрушительный удар на 300% от ад и игнорирует 1 уровень физ защиты. 1015
                //    Дамаг считается за крит и не может быть увеличен крит уроном.
                //TODO this
                case 1015:
                    dmg = myAccount.AttackPower_Stats * 3;
                    if (!check)
                    {
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
                        myAccount.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 2, false));
                    }

                    break;


                //1021 (ад ветка) Кромсатель - наносит 3 удара 30%+50%+70% от ад.  пропуская следующих ход, но получая онхит на 5 ходов (20% уровня + вражеский уровень армора * 10% от уровня)

                //TODO in CheckForBuffs()
                case 1021:
                    myAccount.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 1, false));
                    break;


                //1023 (ад ветка - ульта) Кровавая месть - дает 50% физ лайвстила на 5 ходов 

                //DONE
                case 1023:
                    if (!check)
                        enemyAccount.InstantDeBuff.Add(new AccountSettings.InstantBuffClass(skillId, 5, false));

                    break;
            }


            if (check)
                if(myAccount.PrecentBonusDmg > 0)
                    dmg =  dmg * (1 + myAccount.PrecentBonusDmg);
            if (check)
                dmg = _armorReduction.ArmorHandling(myAccount.PhysicalPenetration, enemyAccount.PhysicalResistance,
                    dmg);


            _accounts.SaveAccounts(myAccount.DiscordId);
            _accounts.SaveAccounts(enemyAccount.DiscordId);


            return dmg;

            //if(check) this is only to show the dmg for display
        }
    }
}