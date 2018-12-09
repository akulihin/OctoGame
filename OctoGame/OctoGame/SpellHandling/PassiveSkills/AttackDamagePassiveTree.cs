using System;
using System.Collections.Concurrent;
using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.PassiveSkills
{
    public class AttackDamagePassiveTree
    {
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1004 = new ConcurrentDictionary<ulong, double>();
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1008 = new ConcurrentDictionary<ulong, double>();
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1016 = new ConcurrentDictionary<ulong, double>();
        private readonly IUserAccounts _accounts;

        public AttackDamagePassiveTree(IUserAccounts accounts)
        {
            _accounts = accounts;
        }

        public void AttackDamagePassiveSkills(ulong skillId, AccountSettings account, AccountSettings enemy)
        {
                switch (skillId)
                {
                    // (ад ветка) Выжидание - пассивно первая атака за бой будет усилена на 20% ||
                    //Not done
                    case 1000:
                        account.PrecentBonusDmg = 0.2;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1000, 999));
                        break;

                    // (ад ветка) Меткость - пассивно если враг увернлся - то следующий ход он не может увернутся. (кд 8 ходов) 
                    //Not done
                    case 1002 when enemy.Dodged >= 1:
                        enemy.DodgeChance = 0;

                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1002, 8));
                        break;

                    //пассивно увеличивает ад на 10% от вражеской текущей выносливости. 
                    //WORKING!
                    case 1004:
                        var dmgValue1004 =   0.1 * enemy.Stamina;

                        if (TemporaryAd1004.TryGetValue(account.DiscordId, out double trying1004))
                        {
                            account.AD_Stats -= trying1004;
                        }
                        
                        account.AD_Stats += dmgValue1004;
                        TemporaryAd1004.AddOrUpdate(account.DiscordId, dmgValue1004, (key, oldValue) => dmgValue1004);

                        break;

                    //(ад ветка) Без изъяна - пассивно дает 1 армор или резист, если на него не куплено ни одной вещи. 1006
                    //not done
                    case 1006 when account.OctoItems.Count == 0 || !account.OctoItems.Any(x => x.Armor >= 1) &&
                                   !account.OctoItems.Any(x => x.Resist >= 1):
                        account.Armor++;
                        account.Resist++;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1006, 999));
                        break;
                    //1008 (ад ветка) Battle trans - пассивно дает дамаг от потеряннх своих хп. 1% к ад за 5% потерянных хп. 
                    //WORKING!
                    case 1008:
                        
                        var dmgValue1008 = Math.Round((1 - account.Health / account.MaxHealth) / 0.5 / 10 * account.AD_Stats);


                        if (TemporaryAd1008.TryGetValue(account.DiscordId, out double trying1008))
                        {
                            account.AD_Stats -= trying1008;
                        }
                        
                        account.AD_Stats += dmgValue1008;

                        TemporaryAd1008.AddOrUpdate(account.DiscordId, dmgValue1008, (key, oldValue) => dmgValue1008);
                        break;

                    //1010 (ад ветка) Мародер - пассивно каждый крит хилит 2% от максимальной стамины.
                           //not done
                    case 1010 when account.IsCrit:
                        account.Stamina += account.MaxStamina * 0.02;
                        break;

                    //1012 (ад ветка) Мастер фехтования - пассивно получает щит на стамину, который блокает физ урон (100% от ад) 
                    //WORKING!
                    case 1012:
                        account.PhysShield = account.AD_Stats;
                        break;

                    //1014 (ад ветка) Безумец - пассивно повышает урон на 10%, но теряет 10% стамины.
                    case 1014:
                        //TODO урон != AD, добавить переменную "DmgToDeal" и работать с ней
                   //         account.Bonus_AD_Stats += account.Bonus_AD_Stats * 0.1;
                    //    account.MaxStamina = account.MaxStamina - account.MaxStamina * 0.1;
                   //     account.Stamina -= account.MaxStamina * 0.1;

                //        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1014, 999));
                        break;

                    //1016 (ад ветка) Ассассин - пассивно дает дамаг по лоу хп. 1% к урону за 5% потерянных вражеских хп. 
                    case 1016:
                        //WORKING!

                        var dmgValue1016 = Math.Round((1 - enemy.Health / enemy.MaxHealth) / 0.5 / 10 * account.AD_Stats);

                        if (TemporaryAd1016.TryGetValue(account.DiscordId, out double trying1016))
                        {
                            account.AD_Stats -= trying1016;
                        }
                        
                        account.AD_Stats += dmgValue1016;

                        TemporaryAd1016.AddOrUpdate(account.DiscordId, dmgValue1016, (key, oldValue) => dmgValue1016);
                        break;

                    //1018 (ад ветка) Мясорубка - пассивно каждый крит увеличивает АД на 1% до конца боя.
                    case 1018:
                        //notDOne
                        //TODO change "HowManyTimesCrited" to local static variable (dictionary)
                        if (account.IsCrit)
                            account.AD_Stats += account.AD_Stats * 0.01 * account.HowManyTimesCrited;
                        break;

                    //1020 (ад ветка) Храбрость - пассивно дает +10 к силе. 
                    //WORKING!
                    case 1020:
                        account.Strength += 10;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1020, 999));
                        break;
                    //1022 (ад ветка) Бронебой - пассивно физический урон пробивает БЛОК из танк ветки, но наносит только 50% урона. Также повышает бафф урон скиллов на 25%
                    // ti ebu dal?
                    case 1022:
                        break;
                }
            _accounts.SaveAccounts(account.DiscordId);
            _accounts.SaveAccounts(enemy.DiscordId);
        }
    }
}