using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.GamePlayFramework;

namespace OctoGame.OctoGame.SpellHandling.Buffs
{
    public sealed class AllBuffs : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly UserAccounts _accounts;
        private readonly DealDmgToEnemy _dealDmgToEnemy;


        private static readonly ConcurrentDictionary<ulong, double> TemporaryStrength1089 =
            new ConcurrentDictionary<ulong, double>();
        private static readonly ConcurrentDictionary<ulong, double> TemporaryAttack1019 =
            new ConcurrentDictionary<ulong, double>();
        private static readonly ConcurrentDictionary<ulong, double> TemporaryBonusDmg1011 =
            new ConcurrentDictionary<ulong, double>();
        private static readonly ConcurrentDictionary<ulong, double> TemporaryBonusOnHitDmg1021 =
                new ConcurrentDictionary<ulong, double>();
            
        // private static List<ulong, double>


        public AllBuffs(UserAccounts accounts, DealDmgToEnemy dealDmgToEnemy)
        {
            _accounts = accounts;
            _dealDmgToEnemy = dealDmgToEnemy;
        }

        public async Task CheckForBuffs(AccountSettings account)
        {
            if (account.InstantBuff.Count > 0)
                for (var i = 0; i < account.InstantBuff.Count; i++)
                {
                    account.InstantBuff[i].forHowManyTurns--;
                    var enemy = _accounts.GetAccount(account.CurrentEnemy);
                    switch (account.InstantBuff[i].skillId)
                    {
                        case 4856757499:
                            break;
   

                        // (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад. 1005 
                        // -Пропускает ход1, может ходить на ход2, и тратит ход3 на удар
                        case 1005:
                            if (!account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                var dmg = account.AttackPower_Stats * 2.28;


                                await _dealDmgToEnemy.DmgHealthHandeling(0, dmg, 0, account,
                                    _accounts.GetAccount(account.CurrentEnemy));

                                account.IsMyTurn = false;
                                _accounts.GetAccount(account.CurrentEnemy).IsMyTurn = true;
                            }

                            break;

                        //1023 (ад ветка - ульта) Кровавая месть - дает 50% физ лайвстила на 5 ходов 

                        case 1023:
                            if (!account.InstantBuff[i].activated)
                            {
                                account.LifeStealPrec += 0.5;
                                account.InstantBuff[i].activated = true;
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                                account.LifeStealPrec -= 0.5;
                            break;

                        // 1019(ад ветка) Решительность - повышает ад на 50 % на следующий ход.

                        case 1019:
                            if (!account.InstantBuff[i].activated)
                            {
                                TemporaryAttack1019.GetOrAdd(account.Id,account.AttackPower_Stats * 0.5);
                                account.InstantBuff[i].activated = true;
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryAttack1019.TryGetValue(account.Id, out var extra);
                                account.AttackPower_Stats -= extra;
                                TemporaryAttack1019.TryRemove(account.Id, out _);
                                
                            }
                            break;

            //(ад ветка) All in - пропускает ход, увеличивает физический урон на один ход через 4 хода. на 10% за каждый пропущенный ход (макс 40%)
                        case 1011:
                            if (!account.InstantBuff[i].activated)
                            {
                                TemporaryBonusDmg1011.GetOrAdd(account.Id, 0);
                                account.InstantBuff[i].activated = true;
                            }

                            TemporaryBonusDmg1011.TryGetValue(account.Id, out var extra1011);

                            if (account.IsDodged && extra1011 < 0.4)
                            {
                                var dmgToGive = extra1011 + 0.1;

                                account.PrecentBonusDmg = account.PrecentBonusDmg - extra1011 + dmgToGive;

                                TemporaryBonusDmg1011.AddOrUpdate(account.Id, dmgToGive,
                                    (key, oldValue) => dmgToGive);
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryBonusDmg1011.TryGetValue(account.Id, out var extra);
                                account.PrecentBonusDmg -= extra;
                                TemporaryBonusDmg1011.TryRemove(account.Id, out _);
                                
                            }

                            break;

                        //1013 (ад ветка) Спартанское копье - активно кричит "Слабым здесь не место", кидая на следующий ход копье, нанося 100% от ад + 15% от вражеских потерянных хп.

                        //DONE
                        case 1013:
                            if (!account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                
                                var dmg = account.AttackPower_Stats * ((enemy.MaxHealth - enemy.Health) * 0.15 / 100 + 1);


                                await _dealDmgToEnemy.DmgHealthHandeling(0, dmg, 0, account,
                                    _accounts.GetAccount(account.CurrentEnemy));

                                account.IsMyTurn = false;
                                enemy.IsMyTurn = true;
                            }
                            break;

 //1021 (ад ветка) Кромсатель - наносит 3 удара 30%+50%+70% от ад.  пропуская следующих ход, но получая онхит на 5 ходов (20% уровня + вражеский уровень армора * 10% от уровня)

                        //TODO implement
                        case 1021:
                            if (!account.InstantBuff[i].activated)
                            {
                              await  _dealDmgToEnemy.DmgHealthHandeling(0, 0, 0, account, enemy);
                              await _dealDmgToEnemy.DmgHealthHandeling(0, 0, 0, account, enemy);
                              await _dealDmgToEnemy.DmgHealthHandeling(0, 0, 0, account, enemy);
                             account.InstantDeBuff.Add(new AccountSettings.InstantBuffClass(1021, 1, false));

                       
                                var onHitDamage =
                                 account.OctoLvL * 0.2 + enemy.PhysicalResistance * enemy.OctoLvL * 0.1;

                                account.OnHitDamage += onHitDamage;

                                TemporaryBonusOnHitDmg1021.AddOrUpdate(account.Id, onHitDamage,
                                    (key, oldValue) => onHitDamage);

                                account.InstantBuff[i].activated = true;
                            }
                          //  ada
                            if (!account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryBonusOnHitDmg1021.TryGetValue(account.Id, out var extra);
                                account.OnHitDamage += extra;
                                TemporaryBonusOnHitDmg1021.TryRemove(account.Id, out _);

                            }

                            break;


                        //1081 (танк ветка) Закал боем - бафф на два хода, который дает (25% от уровня) выносливости с каждым попаданием по врагу. 
                        //TODO implement
                        case 1081:

                            break;

                        //1083 (танк ветка) Замах Гиганта - пропускает 2 хода, следующий ход дамажит микс уроном и скейлится от твоей выносливости 1%*силу + 2хп/силу минус 1%lvl * силу (кд 12)
                        case 1083:

                            break;

                        //1089 (танк ветка) Истинный воин - повышает силу в 2 раза на следующие 2 хода (но  дополнительная силна не влияет на авд) 
                        case 1089:
                            if (!account.InstantBuff[i].activated)
                            {
                                TemporaryStrength1089.GetOrAdd(account.Id, account.Strength);
                                account.Strength = account.Strength * 2;
                                account.InstantBuff[i].activated = true;
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryStrength1089.TryGetValue(account.Id, out var extra);
                                account.Strength -= extra;
                                TemporaryStrength1089.TryRemove(account.Id, out _);
                            }

                            break;

                        //1091 (танк ветка) Тяжелая броня - пропускает ход, твой армор и резист перестанут бафать стамину на 3 хода, а бафнут урон со скилла на следующем ходу после пропуска, но снижает критшанс на этот удар в два раза. (%дамаг + %блока армора + %резиста)
                        case 1091:
                        //AdStast = AdStast *0.52 *0.52
                        // account.AttackPower_Stats ЭТО УРОН СЛЕДУЮЩЕГО СКИЛЛА! это не ад статы!
                        break;

                        //1093 (танк ветка) (prof) Бои без правил - пропускает 3 хода, наносит 3 хита. (150% от ад + (40% от уровня +150% от ап) + 26% от твоей выносливости) микс уроном.
                        case 1093:
                            break;




                    }

                    if (account.InstantBuff[i].forHowManyTurns <= 0)
                    {
                        account.InstantBuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.Id);
                    }
                }

            _accounts.SaveAccounts(account.Id);
            _accounts.SaveAccounts(account.CurrentEnemy);
            await Task.CompletedTask;
        }


        /*
         * if spell will be activated only in 20 turns, and it gives a stats buff for 5 turns, make it 25 turns.
         * 20 => activate shit,
         * 0 => remove shit and delete buff
         */
        public async Task CheckForBuffsToBeActivatedLater(AccountSettings account)
        {
            if (account.BuffToBeActivatedLater.Count > 0)
                for (var i = 0; i < account.BuffToBeActivatedLater.Count; i++)
                {
                    account.BuffToBeActivatedLater[i].afterHowManyTurns--;

                    switch (account.BuffToBeActivatedLater[i].skillId)
                    {
                        //1079 (танк ветка - ульта) Get cancer - после 20и ходов отхиливает фул выносливость +1% за каждые 20 уровней, снимает все дебафы 
                        case 1079:
                            if (account.BuffToBeActivatedLater[i].afterHowManyTurns <= 0)
                            {
                                // ReSharper disable once PossibleLossOfFraction
                                account.Stamina = account.MaxStamina + account.OctoLvL / 20 * account.MaxStamina / 100;
                                account.InstantDeBuff = new List<AccountSettings.InstantBuffClass>();
                                account.DeBuffToBeActivatedLater = new List<AccountSettings.OnTimeBuffClass>();
                            }

                            break;
                    }
                }

            _accounts.SaveAccounts(account.Id);
            await Task.CompletedTask;
        }


        public async Task CheckForDeBuffs(AccountSettings account)
        {
            var enemyAccount = _accounts.GetAccount(account.CurrentEnemy);
            if (account.InstantDeBuff.Count > 0)
                for (var i = 0; i < account.InstantDeBuff.Count; i++)
                {
                    account.InstantDeBuff[i].forHowManyTurns--;
              

                    switch (account.InstantDeBuff[i].skillId)
                    {
                        case 1003:
                            if (!account.InstantDeBuff[i].activated)
                                account.PhysicalResistance -= 2;
                            if (account.InstantDeBuff[i].forHowManyTurns <= 0)
                                account.PhysicalResistance += 2;
                            break;


                        //1075 (танк ветка) Колючие доспехи - следующие 3 хода враг будет получать урон при атаке равный 10% от твоей выносливости за попадание. 
                        //TODO implement
                        case 1075:

                            break;

                        case 1021:
                            account.IsMyTurn = false;
                            enemyAccount.IsMyTurn = true;

                            break;
                    }

                    account.InstantDeBuff[i].activated = true;

                    if (account.InstantDeBuff[i].forHowManyTurns <= 0)
                    {
                        account.InstantDeBuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.Id);
                    }
                }

            _accounts.SaveAccounts(account.Id);
            await Task.CompletedTask;
        }

        public async Task CheckForDeBuffsToBeActivatedLater(AccountSettings account)
        {
            await Task.CompletedTask;
        }
    }
}