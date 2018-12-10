using System;
using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;

namespace OctoGame.OctoGame.SpellHandling.ActiveSkills
{
    public class DefenceActiveTree
    {
        private readonly IUserAccounts _accounts;
        private readonly ArmorReduction _armorReduction;

        public DefenceActiveTree(IUserAccounts accounts, ArmorReduction armorReduction)
        {
            _accounts = accounts;
            _armorReduction = armorReduction;
        }
        
        public double DefSkills(ulong skillId, AccountSettings account, AccountSettings enemy, bool check)
        {
            double dmg = 0;

            switch (skillId)
            {
                //1073 (танк ветка) Напор массой - дает в ебло, наносит микс урон равный (15% от твоей выносливости + 50% от уровня) (кд 4 хода)
                case 1073:

                    // прописать микс урон
                    dmg = account.Stamina * 0.15 + account.OctoLvL * 0.5;
                    if (!check)
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 4));

                    break;

                //1075 (танк ветка) Колючие доспехи - следующие 3 хода враг будет получать урон при атаке равный 10% от твоей выносливости за попадание. 
                case 1075:
                    dmg = account.Stamina * 0.1;
                    if (!check)
                        account.PoisonDamage.Add(new AccountSettings.Poison(skillId, dmg, 3, 0));
                    //TODO poisen dmg!!!!!!!!!!!!!!!!!!!
                    break;
                //1077 (танк ветка) (prof) Надежный щит - пропуская ход, ставит блок, блокируя одно физ попадание врага.  
                //                              (без лимита по времени) (можно использовать только пока активна стамина, кд 5 ходов)
                case 1077:
                    if (!check)
                    {
                        if (account.Stamina > 1)
                            account.BlockShield.Add(new AccountSettings.FullDmgBlock(0, 1, 9999));

                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(skillId, 5));
                    }

                    break;
                //1079 (танк ветка - ульта) Get cancer - после 20и ходов отхиливает фул выносливость +1% за каждые 20 уровней, снимает все дебафы 
                case 1079:
                    if (!check)
                        account.BuffToBeActivatedLater.Add(new AccountSettings.OnTimeBuffClass(skillId, 20, false));
                    dmg = 0;
                    break;

                //1081 (танк ветка) Закал боем - бафф на два хода, который дает (25% от уровня) выносливости с каждым попаданием по врагу. 
                case 1081:
                    if (!check)
                        account.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 2, false));
                    //TODO implement this buff in def pass tree
                    break;
                //1083 (танк ветка) Замах Гиганта - пропускает 2 хода, следующий ход дамажит микс уроном и скейлится от твоей выносливости 1%*силу + 2хп/силу минус 1%lvl * силу (кд 12)
                case 1083:

                    dmg = account.Stamina * 0.01 * account.Stamina + 2 * account.Health / account.Strength -
                          account.OctoLvL * 0.01 * account.Strength;

                    if (!check)
                    {
                        if (account.InstantBuff.Any(x => x.skillId == 1000) && account.IsFirstHit)
                        {
                            dmg = dmg * (1 + account.PrecentBonusDmg);
                            account.IsFirstHit = false;
                        }

                        account.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 3, 2));
                        dmg = 0;
                    }

                    break;
                // 1085 (танк ветка) (prof) Оборона - пропускает 4 хода, получая на них на 50% урона меньше. Считается за блок и пробивается полностью мастеркой Бронебой. по окончанию получает 20% от уровня в ад и 25% в ап.

                case 1085:
                    break;
//1087 (танк ветка - ульта) Берсеркер - снимает твой армор и резист и прибавляет его к ад и ап (деф * 20 % от уровня) до конца боя 
                case 1087:

                    if (!check)
                    {
                        account.AttackPower_Stats += (account.PhysicalResistance + account.MagicalResistance) * account.OctoLvL * 0.2;
                        account.MagicPower_Stats += (account.PhysicalResistance + account.MagicalResistance) * account.OctoLvL * 0.2;
                        account.PhysicalResistance = 0;
                        account.MagicalResistance = 0;
                    }

                    break;

                //1089 (танк ветка) Истинный воин - повышает силу в 2 раза на следующие 2 хода (но уменьшает скейл ад от силы в 2 раза) 
                case 1089:
                    if (!check)
                    {
                        account.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId, 2, false));
                        //TODO implement it in pass def tree
                    }
                    break;

                //1091 (танк ветка) Тяжелая броня - пропускает ход, твой армор и резист перестанут бафать стамину на 3 хода, а бафнут урон со скилла на следующем ходу после пропуска, но снижает критшанс на этот удар в два раза. (%дамаг + %блока армора + %резиста)
                case 1091:
                    //AdStast = AdStast *0.52 *0.52


                    // account.AttackPower_Stats ЭТО УРОН СЛЕДУЮЩЕГО СКИЛЛА! это не ад статы!
                    dmg = account.AttackPower_Stats *
                          (_armorReduction.GetArmorPercentDependsOnLvl(Convert.ToInt32(account.PhysicalResistance) + _armorReduction.GetArmorPercentDependsOnLvl(Convert.ToInt32(account.MagicalResistance))));
                    
                    break;

                //1093 (танк ветка) (prof) Бои без правил - пропускает 3 хода, наносит 3 хита. (150% от ад + (40% от уровня +150% от ап) + 26% от твоей выносливости) микс уроном.
                case 1093:
                    if (!check)
                    {
                        dmg = account.AttackPower_Stats*1.5 + (account.OctoLvL * 0.4 + account.MagicPower_Stats * 1.5) + account.Stamina * 0.26;
                        account.DamageOnTimer.Add(new AccountSettings.DmgWithTimer(dmg, 3, 3));
                    }
                    break;
            
                    //1095(танк ветка - ульта) Доспехи Бессмертной Гидры - после активации 6% от хп переходит в выносливость с каждым ходом, пока выносливость не будет пробита
                case 1095:
               if (!check)
                {
                    account.InstantBuff.Add(new AccountSettings.InstantBuffClass(skillId,9999, false));
                }
                    break;
                // 1099 (танк ветка) (без кд) Кулачный бой -  бьет врага микс уроном, скейлится (1% вражеской стамины или хп за 20 силы + 15% от своей выносливости +20% от ад +25% от ап)
                case 1099:
                    //TODO implemet mixed dmg in deal dmg module
                    var scale = enemy.Stamina > 1
                        ? enemy.Stamina
                        : enemy.Health;

                    dmg = scale * (1 + account.Strength * 0.01 / 20 / 100) + account.Stamina * 0.15 +
                          account.AttackPower_Stats * 0.2 + account.MagicPower_Stats * 0.25;
                    break;
            }

            _accounts.SaveAccounts(account.DiscordId);
            _accounts.SaveAccounts(enemy.DiscordId);

            return dmg;
        }
    }
}