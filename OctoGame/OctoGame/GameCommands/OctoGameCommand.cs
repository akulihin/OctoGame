using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.LoggingSystemJson;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.GamePlayFramework;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame.OctoGame.GameCommands
{
    public class OctoGameCommand : ModuleBaseCustom
    {
        private readonly UserAccounts _accounts;
        private readonly SpellUserAccounts _spellAccounts;
        private readonly LoggingSystem _loggingSystem;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;
        private readonly GameFramework _gameFramework;
        private readonly DiscordShardedClient _client;
        private readonly OctoGameUpdateMess _octoGameUpdateMess;

        public OctoGameCommand(UserAccounts accounts, SpellUserAccounts spellAccounts, Global global,
            AwaitForUserMessage awaitForUserMessage, LoggingSystem loggingSystem,
            GameFramework gameFramework, DiscordShardedClient client, OctoGameUpdateMess octoGameUpdateMess)
        {
            _accounts = accounts;
            _spellAccounts = spellAccounts;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;


            _loggingSystem = loggingSystem;
            _gameFramework = gameFramework;
            _client = client;
            _octoGameUpdateMess = octoGameUpdateMess;
        }


        [Command("CreateOcto", RunMode = RunMode.Async)]
        [Alias("UpdateOcto", "OctoCreate")]
        [Summary("Creates your very own Octopus!")]
        public async Task CreateOctopusFighter()
        {
            var account = _accounts.GetAccount(Context.User);
            var response = "бу";

            if (account.OctoInfo != null)
            {
                await SendMessAsync(
                    "У тебя уже есть осьминожка! Если ты хочешь ПОЛНОСТЬЮ обновить информацию осьминожки напиши **да**");
                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 6000);
                response = res.Content;
            }

            if (account.OctoInfo == null || response == "да")
            {
                account.OctoInfo = null;


                await SendMessAsync( "Введи имя своего осьминожка, у тебя 1 минута.");
                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctoInfo += res.Content + "|";
                account.OctoName = res.Content;

                await SendMessAsync( "Введи цвет своего осьминожка, у тебя 1 минута.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctoInfo += res.Content + "|";


                await SendMessAsync( "Введи характер своего осьминожка, у тебя 2 минуты.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctoInfo += res.Content + "|";

                await SendMessAsync( "Введи Лор своего осьминожка, у тебя 2 минуты.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctoInfo += res.Content + "|";


                account.Strength = 20;
                account.AttackPower_Stats = 0;
                account.MagicPower_Stats = 0;
                account.Agility_Stats = 0;
                account.CriticalDamage = 1.5;
                account.DodgeChance = 0;
                account.PhysicalResistance = 1;
                account.MagicalResistance = 1;
                account.Health = 100;
                account.Stamina = 200;
                account.OctoLvL = 1;
                account.PhysicalPenetration = 0;
                account.MagicalPenetration = 0;

                _accounts.SaveAccounts(Context.User);
                await SendMessAsync( "Готово! Ты создал или обновил информацию о своем осьминоге!");
            }
        }

        [Command("OctoInfo")]
        [Alias("InfoOcto", "OctoInfo", "myOcto")]
        [Summary("Shows octopus info, yours or someone else' ")]
        public async Task CheckOctopusFighter(IGuildUser user = null)
        {
            AccountSettings account;
            if (user == null)
                account = _accounts.GetAccount(Context.User);
            else
                account = _accounts.GetAccount(user);
            _accounts.SaveAccounts(Context.User);
            if (account.OctoInfo == null)
            {
                await SendMessAsync(
                    "У тебя нет осьминожки все ещё! Чтобы создать, впиши команду **CreateOcto**");
                return;
            }

            var octoInfoArray = account.OctoInfo.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            if (octoInfoArray[0] == null)
                octoInfoArray[0] = "Тут ничего нет. Что-то пошло не так, когда ты создавал Осьминога";
            if (octoInfoArray[1] == null)
                octoInfoArray[1] = "Тут ничего нет. Что-то пошло не так, когда ты создавал Осьминога";
            if (octoInfoArray[2] == null)
                octoInfoArray[2] = "Тут ничего нет. Что-то пошло не так, когда ты создавал Осьминога";
            if (octoInfoArray[3] == null)
                octoInfoArray[3] = "Тут ничего нет. Что-то пошло не так, когда ты создавал Осьминога";
            var embed = new EmbedBuilder();
            embed.WithAuthor(Context.User);
            embed.WithColor(Color.Blue);
            embed.AddField("Твой осьминожка!",
                $"**Имя:** {octoInfoArray[0]}\n**Цвет:** {octoInfoArray[1]}\n**Характер** {octoInfoArray[2]}\n**Лор:** {octoInfoArray[3]}");

            await SendMessAsync( embed);
        }

        [Command("endGame")]
        [Summary("Emergency only! Finish previous fight if there any problems.")]
        public async Task EndOctoGameCommand()
        {
            var account = _accounts.GetAccount(Context.User);
            account.PlayingStatus = 0;
            _accounts.SaveAccounts(Context.User);
            await SendMessAsync( "Мы закончили твою игру, можешь начинать другую");
        }


        //Math.Ceiling(dmg)
        public AccountSettings SetupOctopusStats(ulong userId)
        {
            var account = _accounts.GetAccount(userId);

            account.MoveListPage = 1;      
            account.CurrentEnemy = 1;
            account.Health = Math.Ceiling(100.0); //  ONLY ITEMS + SKILLS
            account.MaxHealth = account.Health;
            account.Stamina = Math.Ceiling(100 + 3 * Convert.ToDouble(account.OctoLvL - 1));
            account.MaxStamina = account.Stamina;
            account.Strength = Math.Ceiling(20.0); // ONLY ITEMS + SKILLS

            account.AttackPower_Stats = Math.Ceiling(account.Strength + account.Strength * (0.2 * account.OctoLvL)); // + ITEMS + SKILLS
            account.MagicPower_Stats = Math.Ceiling(10 + 0.1 * account.OctoLvL); // +  ITEMS + SKILLS
            account.Agility_Stats = Math.Ceiling(1.0); // ONLY ITEMS + SKILLS
            account.CriticalDamage = Math.Ceiling(150.0); // 250 MAX ONLY ITEMS + SKILLS
            account.CriticalChance = Math.Ceiling(account.Agility_Stats); // + SKILLS
            account.DodgeChance = Math.Ceiling(account.Agility_Stats - 1); // + SKILLS
            account.PhysicalResistance = Math.Ceiling(1.0); // 1-6 MAX + ITEMS + SKILLS
            account.MagicalResistance = Math.Ceiling(1.0); // 1-6 MAX + ITEMS + SKILLS
            account.PhysicalPenetration = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.MagicalPenetration = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.OnHitDamage =
                Math.Ceiling((account.OctoLvL / 80 + 1) *
                             (account.Agility_Stats / 4 + 1)); // lvl/100 * (1(agility/2)) + ITEMS + SKILLS
            account.CurrentLogString = "";
            
           

  
          //  account.AttackPower_Stats = account.Base_AD_Stats + account.Bonus_AD_Stats;

            account.IsFirstHit = true;
            account.dmgDealtLastTime = 0;
            account.PhysShield = 0;
            account.MagShield = 0;
            account.HowManyTimesCrited = 0;
            account.LifeStealPrec = 0;
            account.BlockShield = new List<AccountSettings.FullDmgBlock>();
            account.InstantBuff = new List<AccountSettings.InstantBuffClass>();
            account.InstantDeBuff = new List<AccountSettings.InstantBuffClass>();
            account.BuffToBeActivatedLater = new List<AccountSettings.OnTimeBuffClass>();
            account.DeBuffToBeActivatedLater = new List<AccountSettings.OnTimeBuffClass>();
            account.PassiveList = new List<AccountSettings.CooldownClass>();
            account.DamageOnTimer = new List<AccountSettings.DmgWithTimer>();
            account.PoisonDamage = new List<AccountSettings.Poison>();
            account.OctoItems = new List<AccountSettings.ArtifactEntities>();
            account.SkillCooldowns = new List<AccountSettings.CooldownClass>();
            account.Inventory = new List<AccountSettings.ArtifactEntities>();

            if (account.AllPassives == null)
                account.AllPassives = "";
            var passives = account.AllPassives.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var passive in passives)
                account.PassiveList.Add(new AccountSettings.CooldownClass(Convert.ToUInt64(passive), 9999));

            _accounts.SaveAccounts(userId);

            return account;
        }

        [Command("Fight")]
        [Summary("A demo for a fight vs Shark")]
        public async Task CreateFight(SocketUser enemyUser = null)
        {

            var account = SetupOctopusStats(Context.User.Id);
            var enemy = SetupOctopusStats(enemyUser?.Id ?? account.CurrentEnemy);


            // tis is a test of logging system. it does work btw
            _loggingSystem.CreateNewLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCurrentFightLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCompletedFight(account.DiscordId, enemy.DiscordId);

            if (account.PlayingStatus >= 1)
            {
                //  await ReplyAsync("you are already playing");
                // return;
            }

            if (account.Attack_Tree == null && account.Defensive_Tree == null &&
                account.Agility_Tree == null && account.Magic_Tree == null)
            {
                await SendMessAsync( "You dont have any moves. You can get them using **boole** command");
                return;
            }

            if (account.OctoInfo != null)
            {
                account.PlayingStatus = 1;
                _accounts.SaveAccounts(Context.User);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.WithFooter("Enemy choice");
                embed.WithColor(Color.DarkGreen);
                embed.AddField("Choose enemy lvl : ",
                    $"{new Emoji("1⃣")} Enemy {account.OctoLvL - 1} LvL\n" +
                    $"{new Emoji("2⃣")} Enemy {account.OctoLvL} LvL\n" +
                    $"{new Emoji("3⃣")} Enemy {account.OctoLvL + 1} LvL\n" +
                    $"{new Emoji("❌")} - End Fight.");


                var message = await Context.Channel.SendMessageAsync("", false, embed.Build());

                await message.AddReactionAsync(new Emoji("1⃣"));
                await message.AddReactionAsync(new Emoji("2⃣"));
                await message.AddReactionAsync(new Emoji("3⃣"));
                await message.AddReactionAsync(new Emoji("❌"));

                var newOctoGame = _global.CreateNewGame(message, null, Context.User, null);

                foreach (var c in  _global.OctopusGameMessIdList)
                {
                    if (c.Any(x => x.PlayerDiscordAccount.Id == Context.User.Id || x.PlayerDiscordAccount.Id == enemy.DiscordId))
                    {
                        _global.OctopusGameMessIdList.Remove(c);
                        break;
                    }
                }

                _global.OctopusGameMessIdList.Add(newOctoGame);
                _global.OctoGamePlaying += 1;
                account.PlayingStatus = 1;
                account.MessageIdInList = _global.OctopusGameMessIdList.Count - 1;


                    //twice for a reason
                await _gameFramework.CheckForPassivesAndUpdateStats(account, enemy);
                await _gameFramework.CheckForPassivesAndUpdateStats(enemy, account);
                await _gameFramework.CheckForPassivesAndUpdateStats(account, enemy);
                await _gameFramework.CheckForPassivesAndUpdateStats(enemy, account);

                _accounts.SaveAccounts(Context.User);
            }
            else
            {
                await SendMessAsync( "У тебя нет осьминога! создай его командой **CreateOcto**");
            }

            _accounts.SaveAccounts(Context.User);
            if (enemyUser != null) _accounts.SaveAccounts(enemyUser.Id);
        }



        [Command("Fightv")]
        [Summary("A demo for a fight vs a player")]
        public async Task CreateFightV(SocketUser enemyUser)
        {

            var account = SetupOctopusStats(Context.User.Id);
            var enemy = SetupOctopusStats(enemyUser.Id);

            account.CurrentEnemy = enemyUser.Id;
            enemy.CurrentEnemy = Context.User.Id;


            // tis is a test of logging system. it does work btw
            _loggingSystem.CreateNewLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCurrentFightLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCompletedFight(account.DiscordId, enemy.DiscordId);


            if (account.PlayingStatus >= 1 || enemy.PlayingStatus >=1 )
            {
                //  await ReplyAsync("you are already playing");
                // return;
            }

            if (account.Attack_Tree == null && account.Defensive_Tree == null &&
                account.Agility_Tree == null && account.Magic_Tree == null)
            {
                await SendMessAsync( "You dont have any moves. You can get them using **boole** command");
                return;
            }

            if (account.OctoInfo != null)
            {
                account.PlayingStatus = 1;
                _accounts.SaveAccounts(Context.User);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.WithFooter("Enemy choice");
                embed.WithColor(Color.DarkGreen);
                embed.AddField("Choose enemy lvl : ",
                    $"Who are you?");


                var messageToPlayer1 = _client.GetUser(Context.User.Id).GetOrCreateDMChannelAsync().Result.SendMessageAsync("", false, embed.Build());
                var messageToPlayer2 =  _client.GetUser(enemyUser.Id).GetOrCreateDMChannelAsync().Result.SendMessageAsync("", false, embed.Build());

                var newOctoGame = _global.CreateNewGame(messageToPlayer1.Result, messageToPlayer2.Result, Context.User, enemyUser);


                foreach (var c in  _global.OctopusGameMessIdList)
                {
                    if (c.Any(x => x.PlayerDiscordAccount.Id == Context.User.Id || x.PlayerDiscordAccount.Id == enemy.DiscordId))
                    {
                        _global.OctopusGameMessIdList.Remove(c);
                        break;
                    }
                }

                _global.OctopusGameMessIdList.Add(newOctoGame);
                _global.OctoGamePlaying += 1;
                account.PlayingStatus = 1;
                account.MessageIdInList = _global.OctopusGameMessIdList.Count - 1;
                enemy.MessageIdInList = _global.OctopusGameMessIdList.Count - 1;


                    //twice for a reason
                await _gameFramework.CheckForPassivesAndUpdateStats(account, enemy);
                await _gameFramework.CheckForPassivesAndUpdateStats(enemy, account);
                await _gameFramework.CheckForPassivesAndUpdateStats(account, enemy);
                await _gameFramework.CheckForPassivesAndUpdateStats(enemy, account);


                var someList = new List<ulong>
                {
                    Context.User.Id,
                    enemyUser.Id
                };

                Parallel.For(0, someList.Count, async i =>
                {
                    if(someList[i] == Context.User.Id)
                        await _octoGameUpdateMess.WaitMess(Context.User.Id,
                            messageToPlayer1.Result as RestUserMessage);
                    else
                        await _octoGameUpdateMess.WaitMess(enemyUser.Id,
                            messageToPlayer2.Result as RestUserMessage);
                });


            }
            else
            {
                await SendMessAsync( "У тебя нет осьминога! создай его командой **CreateOcto**");
            }

            _accounts.SaveAccounts(Context.User);
            _accounts.SaveAccounts(enemyUser.Id);
        }


        [Command("CreateSkill", RunMode = RunMode.Async)]
        [Alias("CSS")]
        [Summary("OLD. Adds a skill to the DataBase, only for staff")]
        public async Task CreateSkill(ulong skillId)
        {
            var skill = _spellAccounts.GetAccount(skillId);

            if (skill.SpellNameEn != null)
            {
                var embed1 = new EmbedBuilder();
                embed1.WithAuthor(Context.User);
                embed1.AddField("Этот скилл иди уже существует",
                    $"{skill.SpellNameEn}\nID: {skill.SpellId}\nTree: {skill.SpellTreeNum}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}\n" +
                    "Если хочешь полностью его изменить, напиши **да** (1 минута)");

                await SendMessAsync( embed1);

                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);

                if (res.Content == "да")
                {
                    await SendMessAsync( $"Ты изменяешь скилл {skill.SpellNameEn}");
                }
                else
                {
                    await SendMessAsync( $"никаких апдейтов. (ты сказал {res.Content})");
                    return;
                }
            }

            await SendMessAsync( "Введи Назваие скилла, у тебя 5 минута.");
            var response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellNameEn = response.ToString();

            var embed = new EmbedBuilder();
            embed.AddField("Введи Номер дерева скилла, у тебя 5 минута", "1 - AD\n2 - DEF\n3 - AGI\n4 - AP");

            await SendMessAsync( embed);


            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellTreeNum = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync(
                "Введи Русское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellDescriptionRu = response.ToString();

            await Context.Channel.SendMessageAsync(
                "Введи Английское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellDescriptionEn = response.ToString();

            var embedAc = new EmbedBuilder();
            embedAc.AddField("Введи Активка или Пассивка, у тебя 5 минута", "0 - Пассив\n1 - Актив");
            await Context.Channel.SendMessageAsync("", false, embedAc.Build());
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellType = Convert.ToInt32(response.ToString());
            /*
            await Context.Channel.SendMessageAsync("Введи Формулу описание скилла, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.DiscordId, Context.Channel.DiscordId, 3000000 );
            skill.SpellFormula = response.ToString();
            */
            var embedCd = new EmbedBuilder();
            embedCd.AddField("Введи КД скилла, у тебя 5 минут",
                "1)Если есть - в ходах\n2)Если КД = 1 раз в игру, пиши 9999\n3)Если КД нету вообще, пиши 0");
            await Context.Channel.SendMessageAsync("", false, embedCd.Build());
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellCd = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync("Тип урона (AD or AP), у тебя 5 минут.");
            await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            // skill.SpellDmgType = response.ToString();

            /*
            await Context.Channel.SendMessageAsync("Введи Пойзен (прокает он хит), у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.DiscordId, Context.Channel.DiscordId, 3000000 );
            skill.Poison = response.ToString();
           
            await Context.Channel.SendMessageAsync("Введи ОнХит, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.DiscordId, Context.Channel.DiscordId, 3000000 );
            skill.Onhit = response.ToString();

            await Context.Channel.SendMessageAsync("Введи Бафф, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.DiscordId, Context.Channel.DiscordId, 3000000 );
            skill.InstantBuff = response.ToString();

            await Context.Channel.SendMessageAsync("Введи ДЕбафф, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.DiscordId, Context.Channel.DiscordId, 3000000 );
            skill.DeBuff = response.ToString();
            */
            _spellAccounts.SaveAccounts();
            await SendMessAsync( "Готово!");
        }


        [Command("CS", RunMode = RunMode.Async)]
        [Summary("NEW. Adds a skill to the DataBase, only for staff")]
        public async Task CreateSkillDev(ulong skillId, [Remainder] string skillNameRu)
        {
            var skill = _spellAccounts.GetAccount(skillId);

            var messSplit = skillNameRu.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);

            if (messSplit.Length >= 3)
            {
                await SendMessAsync(" only one `-` can be in a string (between `SpellNameRu` and `SpellDescriptionRu`)");
                return;
            }

            var secondSplit = messSplit[0].Split(new[] {')'}, StringSplitOptions.RemoveEmptyEntries);

            skill.SpellNameRu = secondSplit[1];
            skill.SpellDescriptionRu = messSplit[1];
            skill.SpellFormula = "TODO";
            skill.SpellDescriptionEn = "TODO";

            var firstMess = await ReplyAsync("SpellNameEn");
            var response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellNameEn = response.ToString();
            await Context.Channel.DeleteMessageAsync(response);


            if (messSplit[1].Contains("пассивно"))
            {
                skill.SpellType = 0;
            }
            else
            {
                await firstMess.ModifyAsync(msg =>
                {
                    msg.Content = "SpellType\n" +
                                  "* 0 = passive\n" +
                                  "* 1 = active\n" +
                                  "* 2 = Ultimate";
                });
                response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
                skill.SpellType = Convert.ToInt32(response.ToString());
                await Context.Channel.DeleteMessageAsync(response);
            }

            if (messSplit[0].Contains("маг ветка"))
            {
                skill.SpellTreeNum = 4;
            }
            else if (messSplit[0].Contains("ловкость ветка"))
            {
                skill.SpellTreeNum = 3;
            }
            else if (messSplit[0].Contains("танк ветка"))
            {
                skill.SpellTreeNum = 2;
            }
            else if (messSplit[0].Contains("ад ветка"))
            {
                skill.SpellTreeNum = 1;
            }
            else
            {
                await firstMess.ModifyAsync(msg =>
                {
                    msg.Content = "SpellTreeNum\n" +
                                  "* 0 = General\n" +
                                  "* 1 = AD\n" +
                                  "* 2 = DEF\n" +
                                  "* 3 = AGI\n" +
                                  "* 4 = AP\n";
                });
                response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
                skill.SpellTreeNum = Convert.ToInt32(response.ToString());


                switch (Convert.ToInt32(response.ToString()))
                {
                    case 0:
                        skill.SpellTreeString = "General";
                        break;
                    case 1:
                        skill.SpellTreeString = "AD";
                        break;
                    case 2:
                        skill.SpellTreeString = "DEF";
                        break;
                    case 3:
                        skill.SpellTreeString = "AGI";
                        break;
                    case 4:
                        skill.SpellTreeString = "AP";
                        break;
                    default:
                        await SendMessAsync("You fucked up. change `SpellTreeNum` later");
                        skill.SpellTreeString = "ERROR";
                        break;
                }

                await Context.Channel.DeleteMessageAsync(response);
            }

            if (messSplit[1].Contains("пассивно"))
            {
                skill.SpellDmgType = 5;
            }
            else
            {
                await firstMess.ModifyAsync(msg =>
                {
                    msg.Content = "SpellDmgType\n" +
                                  "* 0 = Physic\n" +
                                  "* 1 = Magic\n" +
                                  "* 2 = True\n" +
                                  "* 3 = Mix\n";
                });
                response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
                skill.SpellDmgType = Convert.ToInt32(response.ToString());
                await Context.Channel.DeleteMessageAsync(response);
            }

            await firstMess.ModifyAsync(msg => { msg.Content = "SpellCd"; });
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellCd = Convert.ToInt32(response.ToString());
            await Context.Channel.DeleteMessageAsync(response);

            if (messSplit[1].Contains("пассивно"))
            {
                skill.WhereDmg = 5;
            }
            else
            {
                await firstMess.ModifyAsync(msg =>
                {
                    msg.Content = "WhereDmg\n" +
                                  "* 0 = Regular( stamina -> health)\n" +
                                  "* 1 = directly To health\n" +
                                  "* 2 = only to stamina\n";
                });
                response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
                skill.WhereDmg = Convert.ToInt32(response.ToString());
                await Context.Channel.DeleteMessageAsync(response);
            }

            _spellAccounts.SaveAccounts();
            await firstMess.ModifyAsync(msg => { msg.Content = "Done"; });
        }


        [Command("SeeSkill")]
        [Summary("Get details about a spell")]
        public async Task SeeSkill(ulong skillId)
        {
            try
            {
                var skill = _spellAccounts.GetAccount(skillId);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.AddField($"{skill.SpellNameEn}",
                    $"ID: {skill.SpellId}\nTree: {skill.SpellTreeNum}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}");


                await SendMessAsync( embed);
            }
            catch
            {
                //   await ReplyAsync("Такого скила нету. Наши скиллы начинаються с ид **1000**");
            }
        }

        [Command("AllSkill")]
        [Alias("AllSkills")]
        [Summary("Get details about all spells")]
        public async Task AllSkill()
        {
            string result;
            try
            {
                result = new WebClient().DownloadString(@"OctoGameDataBase/SpellBook.json");
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(AllSkill)");
                return;
            }

            var data = JsonConvert.DeserializeObject<List<SpellSetting>>(result);

            var embed = new EmbedBuilder();
            var allSkills = "";
            for (var i = 0; i < data.Count; i++) allSkills += $"{i + 1}. {data[i].SpellNameEn} {data[i].SpellId}\n";

            embed.WithAuthor(Context.User);
            embed.AddField("Все скилы:", $"{allSkills}");

            await SendMessAsync( embed);
        }
    }
}