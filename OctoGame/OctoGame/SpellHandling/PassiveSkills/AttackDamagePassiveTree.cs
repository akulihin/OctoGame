using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.PassiveSkills
{
    public sealed class AttackDamagePassiveTree : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1004 = new ConcurrentDictionary<ulong, double>();
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1008 = new ConcurrentDictionary<ulong, double>();
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1016 = new ConcurrentDictionary<ulong, double>();
        private  static readonly ConcurrentDictionary<ulong, double> TemporaryAd1018 = new ConcurrentDictionary<ulong, double>();
        private readonly UserAccounts _accounts;

        public AttackDamagePassiveTree(UserAccounts accounts)
        {
            _accounts = accounts;
        }

        public void AttackDamagePassiveSkills(ulong skillId, AccountSettings account, AccountSettings enemy)
        {
          
                switch (skillId)
                {
                    // (ад ветка) Выжидание - пассивно первая атака за бой будет усилена на 20% ||

                    //Done
                    case 1000:
                        account.PrecentBonusDmg = 0.2;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1000, 999));
                        break;

                    // (ад ветка) Меткость - пассивно если враг увернлся - то следующий ход он не может увернутся. (кд 8 ходов) 

                    //TODO
                    case 1002 when enemy.IsDodged:
                        enemy.DodgeChance = 0;

                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1002, 8));
                        break;

                    //пассивно увеличивает ад на 10% от вражеской текущей выносливости. 

                    //DONE
                    case 1004:
                        var dmgValue1004 =   0.1 * enemy.Stamina;

                        if (TemporaryAd1004.TryGetValue(account.Id, out double trying1004))
                        {
                            account.AttackPower_Stats -= trying1004;
                        }
                        
                        account.AttackPower_Stats += dmgValue1004;
                        TemporaryAd1004.AddOrUpdate(account.Id, dmgValue1004, (key, oldValue) => dmgValue1004);

                        break;

                    //(ад ветка) Без изъяна - пассивно дает 1 армор или резист, если на него не куплено ни одной вещи. 1006

                    //DONE
                    case 1006 when account.OctoItems.Count == 0 || !account.OctoItems.Any(x => x.Armor >= 1) &&
                                   !account.OctoItems.Any(x => x.Resist >= 1):
                        account.PhysicalResistance++;
                        account.MagicalResistance++;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1006, 999));
                        break;

                    //1008 (ад ветка) Battle trans - пассивно дает дамаг от потеряннх своих хп. 1% к ад за 5% потерянных хп. 

                    //Done
                    case 1008:
                        
                        var dmgValue1008 = Math.Round((1 - account.Health / account.MaxHealth) / 0.5 / 10 * account.AttackPower_Stats);


                        if (TemporaryAd1008.TryGetValue(account.Id, out double trying1008))
                        {
                            account.AttackPower_Stats -= trying1008;
                        }
                        
                        account.AttackPower_Stats += dmgValue1008;

                        TemporaryAd1008.AddOrUpdate(account.Id, dmgValue1008, (key, oldValue) => dmgValue1008);
                        break;

                    //1010 (ад ветка) Мародер - пассивно каждый крит хилит 2% от максимальной стамины.

                    //DONE
                    case 1010 when account.IsCrit:
                       
                        account.Stamina += account.MaxStamina * 0.02;
                        break;

                    //1012 (ад ветка) Мастер фехтования - пассивно получает щит на стамину, который блокает физ урон (100% от ад) 

                    //Done
                    case 1012:
                        account.PhysShield = account.AttackPower_Stats;
                        break;

                    //1014 (ад ветка) Безумец - пассивно повышает урон на 10%, но теряет 10% стамины. ( урорн всем скиллам)
                    //TODO урон != AD, добавить переменную "DmgToDeal" и работать с ней
                    case 1014:
                        
                        break;

                    //1016 (ад ветка) Ассассин - пассивно дает дамаг по лоу хп. 1% к урону за 5% потерянных вражеских хп. 

                    //Done
                    case 1016:
                     

                        var dmgValue1016 = Math.Round((1 - enemy.Health / enemy.MaxHealth) / 0.5 / 10 * account.AttackPower_Stats);

                        if (TemporaryAd1016.TryGetValue(account.Id, out double trying1016))
                        {
                            account.AttackPower_Stats -= trying1016;
                        }
                        
                        account.AttackPower_Stats += dmgValue1016;

                        TemporaryAd1016.AddOrUpdate(account.Id, dmgValue1016, (key, oldValue) => dmgValue1016);
                        break;

                    //1018 (ад ветка) Мясорубка - пассивно каждый крит увеличивает АД на 1% до конца боя.

                    //DONE
                    case 1018:
                      
                        var dmgValue1018 = account.AttackPower_Stats * 0.01 * account.HowManyTimesCrited;

                        if (TemporaryAd1018.TryGetValue(account.Id, out double trying1018))
                        {
                            account.AttackPower_Stats -= trying1018;
                        }

                        account.AttackPower_Stats += dmgValue1018;
                        TemporaryAd1018.AddOrUpdate(account.Id, dmgValue1018, (key, oldValue) => dmgValue1018);

                        break;

                    //1020 (ад ветка) Храбрость - пассивно дает +10 к силе. 

                    //Done
                    case 1020:
                        account.Strength += 10;
                        account.SkillCooldowns.Add(new AccountSettings.CooldownClass(1020, 999));
                        break;

                    //1022 (ад ветка) Бронебой - пассивно физический урон пробивает БЛОК из танк ветки, но наносит только 50% урона. Также повышает бафф урон скиллов на 25%

                    // TODO: dat' ebu
                    case 1022:
                        break;
                }
            _accounts.SaveAccounts(account.Id);
            _accounts.SaveAccounts(enemy.DiscordId);
        }
    }
}