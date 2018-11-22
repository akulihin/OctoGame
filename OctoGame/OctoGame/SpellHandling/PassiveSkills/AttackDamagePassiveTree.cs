using System;
using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.PassiveSkills
{
    public class AttackDamagePassiveTree
    {
        public void AttackDamagePassiveSkills(AccountSettings account, AccountSettings enemy, int i)
        {
                switch (account.Buff[i].skillId)
                {
                    // (ад ветка) Выжидание - пассивно первая атака за бой будет усилена на 20% ||
                    case 1000:
                        account.PrecentBonusDmg = 0.2;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1000, 999));
                        break;

                    // (ад ветка) Меткость - пассивно если враг увернлся - то следующий ход он не может увернутся. (кд 8 ходов) 
                    case 1002 when enemy.Dodged >= 1:
                        enemy.DodgeChance = 0;

                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1002, 8));
                        break;

                    //пассивно увеличивает ад на 10% от вражеской текущей выносливости. 
                    case 1004:

                        account.Bonus_AD_Stats += 0.1 * enemy.Stamina;

                        break;

                    //(ад ветка) Без изъяна - пассивно дает 1 армор или резист, если на него не куплено ни одной вещи. 1006
                    case 1006 when account.OctoItems.Count == 0 || !account.OctoItems.Any(x => x.Armor >= 1) &&
                                   !account.OctoItems.Any(x => x.Resist >= 1):
                        account.Armor++;
                        account.Resist++;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1006, 999));
                        break;
                    //1008 (ад ветка) Battle trans - пассивно дает дамаг от потеряннх своих хп. 1% к ад за 5% потерянных хп. 
                    case 1008:
                        // 110
                        account.Bonus_AD_Stats += Math.Round(account.Health / account.MaxHealth / 5 * account.AD_Stats);
                        break;

                    //1010 (ад ветка) Мародер - пассивно каждый крит хилит 2% от максимальной стамины.
                    case 1010 when account.IsCrit:
                        account.Stamina += account.MaxStamina * 0.02;
                        break;

                    //1012 (ад ветка) Мастер фехтования - пассивно получает щит на стамину, который блокает физ урон (100% от ад) 
                    case 1012:
                        account.PhysShield = account.AD_Stats;
                        break;

                    //1014 (ад ветка) Безумец - пассивно повышает урон на 10%, но теряет 10% стамины.
                    case 1014:
                        account.Bonus_AD_Stats += account.Bonus_AD_Stats * 0.1;
                        account.MaxStamina = account.MaxStamina - account.MaxStamina * 0.1;
                        account.Stamina -= account.MaxStamina * 0.1;

                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1014, 999));
                        break;

                    //1016 (ад ветка) Ассассин - пассивно дает дамаг по лоу хп. 1% к урону за 5% потерянных вражеских хп. 
                    case 1016:
                        account.Bonus_AD_Stats += Math.Round(enemy.Health / enemy.MaxHealth / 5 * account.AD_Stats);
                        break;

                    //1018 (ад ветка) Мясорубка - пассивно каждый крит увеличивает АД на 1% до конца боя.
                    case 1018:
                        if (account.IsCrit)
                            account.Bonus_AD_Stats += account.AD_Stats * 0.01 * account.HowManyTimesCrited;
                        break;

                    //1020 (ад ветка) Храбрость - пассивно дает +10 к силе. 
                    case 1020:
                        account.Strength += 10;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1020, 999));
                        break;
                    //1022 (ад ветка) Бронебой - пассивно физический урон пробивает БЛОК из танк ветки, но наносит только 50% урона. Также повышает бафф урон скиллов на 25%
                    // ti ebu dal?
                    case 1022:
                        break;
                }
        }
    }
}