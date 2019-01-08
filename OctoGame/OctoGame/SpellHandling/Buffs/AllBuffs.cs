using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.GamePlayFramework;

namespace OctoGame.OctoGame.SpellHandling.Buffs
{
    public class AllBuffs
    {
        private readonly IUserAccounts _accounts;
        private readonly DealDmgToEnemy _dealDmgToEnemy;


        private static readonly ConcurrentDictionary<ulong, double> TemporaryStrength1089 =
            new ConcurrentDictionary<ulong, double>();
        private static readonly ConcurrentDictionary<ulong, double> TemporaryAttack1019 =
            new ConcurrentDictionary<ulong, double>();
        private static readonly ConcurrentDictionary<ulong, double> TemporaryBonusDmg1011 =
            new ConcurrentDictionary<ulong, double>();
        // private static List<ulong, double>


        public AllBuffs(IUserAccounts accounts, DealDmgToEnemy dealDmgToEnemy)
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

                    switch (account.InstantBuff[i].skillId)
                    {
                        case 4856757499:
                            break;
                        //1089 (танк ветка) Истинный воин - повышает силу в 2 раза на следующие 2 хода (но  дополнительная силна не влияет на авд) 
                        case 1089:
                            if (!account.InstantBuff[i].activated)
                            {
                                TemporaryStrength1089.GetOrAdd(account.DiscordId, account.Strength);
                                account.Strength = account.Strength * 2;
                                account.InstantBuff[i].activated = true;
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryStrength1089.TryGetValue(account.DiscordId, out var extra);
                                account.Strength -= extra;
                                TemporaryStrength1089.TryRemove(account.DiscordId, out _);
                            }

                            break;

                        // (ад ветка) Грязный прием - пропускает один ход, и через еще ход бьет 228% от ад. 1005 
                        // -Пропускает ход1, может ходить на ход2, и тратит ход3 на удар
                        case 1005:
                            if (!account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                var dmg = account.AttackPower_Stats * 2.28;


                                await _dealDmgToEnemy.DmgHealthHandeling(0, dmg, 0, account,
                                    _accounts.GetAccount(account.CurrentEnemy));

                                account.Turn = 1;
                                _accounts.GetAccount(account.CurrentEnemy).Turn = 0;
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
                                TemporaryAttack1019.GetOrAdd(account.DiscordId,account.AttackPower_Stats * 0.5);
                                account.InstantBuff[i].activated = true;
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryAttack1019.TryGetValue(account.DiscordId, out var extra);
                                account.AttackPower_Stats -= extra;
                                TemporaryAttack1019.TryRemove(account.DiscordId, out _);
                                
                            }
                            break;

            //(ад ветка) All in - пропускает ход, увеличивает физический урон на один ход через 4 хода. на 10% за каждый пропущенный ход (макс 40%)
                        case 1011:
                            if (!account.InstantBuff[i].activated)
                            {
                                TemporaryBonusDmg1011.GetOrAdd(account.DiscordId, 0);
                                account.InstantBuff[i].activated = true;
                            }

                            TemporaryBonusDmg1011.TryGetValue(account.DiscordId, out var extra1011);

                            if (account.IsDodged && extra1011 < 0.4)
                            {
                                var dmgToGive = extra1011 + 0.1;

                                account.PrecentBonusDmg = account.PrecentBonusDmg - extra1011 + dmgToGive;

                                TemporaryBonusDmg1011.AddOrUpdate(account.DiscordId, dmgToGive,
                                    (key, oldValue) => dmgToGive);
                            }

                            if (account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                TemporaryBonusDmg1011.TryGetValue(account.DiscordId, out var extra);
                                account.PrecentBonusDmg -= extra;
                                TemporaryBonusDmg1011.TryRemove(account.DiscordId, out _);
                                
                            }

                            break;

                        //1013 (ад ветка) Спартанское копье - активно кричит "Слабым здесь не место", кидая на следующий ход копье, нанося 100% от ад + 15% от вражеских потерянных хп.

                        //DONE
                        case 1013:
                            if (!account.InstantBuff[i].activated && account.InstantBuff[i].forHowManyTurns <= 0)
                            {
                                var e = _accounts.GetAccount(account.CurrentEnemy);
                                var dmg = account.AttackPower_Stats * ((e.MaxHealth - e.Health) * 0.15 / 100 + 1);


                                await _dealDmgToEnemy.DmgHealthHandeling(0, dmg, 0, account,
                                    _accounts.GetAccount(account.CurrentEnemy));

                                account.Turn = 1;
                                e.Turn = 0;
                            }
                            break;

                        //1021 (ад ветка) Кромсатель - наносит 3 удара 30%+50%+70% от ад.  пропуская следующих ход, но получая онхит на 5 ходов (20% уровня + вражеский уровень армора * 10% от уровня)

                        //TODO
                        case 1021:



                            break;
                    }

                    if (account.InstantBuff[i].forHowManyTurns <= 0)
                    {
                        account.InstantBuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.DiscordId);
                    }
                }

            _accounts.SaveAccounts(account.DiscordId);
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

            _accounts.SaveAccounts(account.DiscordId);
            await Task.CompletedTask;
        }


        public async Task CheckForDeBuffs(AccountSettings account)
        {
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
                    }

                    account.InstantDeBuff[i].activated = true;

                    if (account.InstantDeBuff[i].forHowManyTurns <= 0)
                    {
                        account.InstantDeBuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.DiscordId);
                    }
                }

            _accounts.SaveAccounts(account.DiscordId);
            await Task.CompletedTask;
        }

        public async Task CheckForDeBuffsToBeActivatedLater(AccountSettings account)
        {
            await Task.CompletedTask;
        }
    }
}