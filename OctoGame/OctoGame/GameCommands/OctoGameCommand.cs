using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.LoggingSystemJson;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.GamePlayFramework;

namespace OctoGame.OctoGame.GameCommands
{
    public class OctoGameCommand : ModuleBase<SocketCommandContextCustom>
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly ILoggingSystem _loggingSystem;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;
        private readonly CommandHandeling _command;
        private readonly GameFramework _gameFramework;

        public OctoGameCommand(IUserAccounts accounts, ISpellAccounts spellAccounts, Global global,
            AwaitForUserMessage awaitForUserMessage, CommandHandeling command, ILoggingSystem loggingSystem,
            GameFramework gameFramework)
        {
            _accounts = accounts;
            _spellAccounts = spellAccounts;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;
            _command = command;

            _loggingSystem = loggingSystem;
            _gameFramework = gameFramework;
        }


        [Command("CreateOcto", RunMode = RunMode.Async)]
        [Alias("UpdateOcto", "OctoCreate")]
        public async Task CreateOctopusFighter()
        {
            var account = _accounts.GetAccount(Context.User);
            var response = "бу";


            if (account.OctoInfo != null)
            {
                await _command.ReplyAsync(Context,
                    "У тебя уже есть осьминожка! Если ты хочешь ПОЛНОСТЬЮ обновить информацию осьминожки напиши **да**");
                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 6000);
                response = res.Content;
            }

            if (account.OctoInfo == null || response == "да")
            {
                account.OctoInfo = null;


                await _command.ReplyAsync(Context, "Введи имя своего осьминожка, у тебя 1 минута.");
                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctoInfo += res.Content + "|";
                account.OctoName = res.Content;

                await _command.ReplyAsync(Context, "Введи цвет своего осьминожка, у тебя 1 минута.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctoInfo += res.Content + "|";


                await _command.ReplyAsync(Context, "Введи характер своего осьминожка, у тебя 2 минуты.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctoInfo += res.Content + "|";

                await _command.ReplyAsync(Context, "Введи Лор своего осьминожка, у тебя 2 минуты.");
                res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctoInfo += res.Content + "|";


                account.Strength = 20;
                account.AD_Stats = 0;
                account.AP_Stats = 0;
                account.AG_Stats = 0;
                account.CritDmg = 1.5;
                account.DodgeChance = 0;
                account.Armor = 1;
                account.Resist = 1;
                account.Health = 100;
                account.Stamina = 200;
                account.OctoLvL = 1;
                account.ArmPen = 0;
                account.MagPen = 0;

                _accounts.SaveAccounts(Context.User);
                await _command.ReplyAsync(Context, "Готово! Ты создал или обновил информацию о своем осьминоге!");
            }
        }

        [Command("OctoInfo")]
        [Alias("InfoOcto", "OctoInfo", "myOcto")]
        public async Task CehckOctopusFighter(IGuildUser user = null)
        {
            AccountSettings account;
            if (user == null)
                account = _accounts.GetAccount(Context.User);
            else
                account = _accounts.GetAccount(user);
            _accounts.SaveAccounts(Context.User);
            if (account.OctoInfo == null)
            {
                await _command.ReplyAsync(Context,
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

            await _command.ReplyAsync(Context, embed);
        }

        [Command("endGame")]
        public async Task EndOctoGameCommand()
        {
            var account = _accounts.GetAccount(Context.User);
            account.PlayingStatus = 0;
            _accounts.SaveAccounts(Context.User);
            await _command.ReplyAsync(Context, "Мы закончили твою игру, можешь начинать другую");
        }


        //Math.Ceiling(dmg)
        public AccountSettings SetupOctopusStats(ulong userId)
        {
            var account = _accounts.GetAccount(userId);

            account.MoveListPage = 1;
            account.MaxHealth = account.Health;
            account.CurrentEnemy = 1;
            account.Health = Math.Ceiling(100.0); //  ONLY ITEMS + SKILLS
            account.Stamina = Math.Ceiling(100 + 3 * Convert.ToDouble(account.OctoLvL - 1));
            account.Strength = Math.Ceiling(20.0); // ONLY ITEMS + SKILLS
            account.AD_Stats = Math.Ceiling(account.Strength + account.Strength * (0.2 * account.OctoLvL)); // + ITEMS + SKILLS
            account.AP_Stats = Math.Ceiling(10 + 0.1 * account.OctoLvL); // +  ITEMS + SKILLS
            account.AG_Stats = Math.Ceiling(1.0); // ONLY ITEMS + SKILLS
            account.CritDmg = Math.Ceiling(150.0); // 250 MAX ONLY ITEMS + SKILLS
            account.CritChance = Math.Ceiling(account.AG_Stats); // + SKILLS
            account.DodgeChance = Math.Ceiling(account.AG_Stats - 1); // + SKILLS
            account.Armor = Math.Ceiling(1.0); // 1-6 MAX + ITEMS + SKILLS
            account.Resist = Math.Ceiling(1.0); // 1-6 MAX + ITEMS + SKILLS
            account.ArmPen = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.MagPen = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.OnHit =
                Math.Ceiling((account.OctoLvL / 80 + 1) *
                             (account.AG_Stats / 4 + 1)); // lvl/100 * (1(agility/2)) + ITEMS + SKILLS
            account.CurrentLogString = "";
            
           
            account.DamageOnTimer = new List<AccountSettings.DmgWithTimer>();
            account.PoisonDamage = new List<AccountSettings.Poison>();
            account.OctoItems = new List<AccountSettings.ArtifactEntities>();
            account.SkillCooldowns = new List<AccountSettings.CooldownClass>();
            account.Inventory = new List<AccountSettings.ArtifactEntities>();
  
          //  account.AD_Stats = account.Base_AD_Stats + account.Bonus_AD_Stats;
            account.MaxStamina = account.Stamina;
            account.FirstHit = true;
            account.dmgDealedLastTime = 0;
            account.PhysShield = 0;
            account.MagShield = 0;
            account.HowManyTimesCrited = 0;
            account.LifeStealPrec = 0;
            account.StatsForTime = new List<AccountSettings.StatsForTimeClass>();
            account.BlockShield = new List<AccountSettings.FullDmgBlock>();
            if (account.Passives == null)
                account.Passives = "";


            var passives = account.Passives.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            account.InstantBuff = new List<AccountSettings.InstantBuffClass>();
            account.InstantDeBuff = new List<AccountSettings.InstantBuffClass>();
            account.BuffToBeActivatedLater = new List<AccountSettings.OnTimeBuffClass>();
            account.DeBuffToBeActivatedLater = new List<AccountSettings.OnTimeBuffClass>();
            account.PassiveList = new List<AccountSettings.CooldownClass>();
            foreach (var passive in passives)
                account.PassiveList.Add(new AccountSettings.CooldownClass(Convert.ToUInt64(passive), 9999));

            _accounts.SaveAccounts(userId);

            return account;
        }

        [Command("Fight")]
        public async Task CreateFight()
        {
            var account = SetupOctopusStats(Context.User.Id);
            var enemy = SetupOctopusStats(account.CurrentEnemy);


            var logs = _loggingSystem.CreateNewLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCurrentFightLog(account.DiscordId, enemy.DiscordId);
            _loggingSystem.SaveCompletedFight(account.DiscordId, enemy.DiscordId);

            if (account.PlayingStatus >= 1)
            {
                //  await ReplyAsync("you are already playing");
                // return;
            }

            if (account.AD_Tree == null && account.DEF_Tree == null &&
                account.AG_Tree == null && account.AP_Tree == null)
            {
                await _command.ReplyAsync(Context, "You dont have any moves. You can get them using **boole** command");
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

                var newOctoGame =
                    new Global.OctoGameMessAndUserTrack(message.Id, Context.User.Id, message, Context.User);


                _global.OctopusGameMessIdList.Add(newOctoGame);
                _global.OctoGamePlaying += 1;
                account.PlayingStatus = 1;
                account.MessageIdInList = _global.OctopusGameMessIdList.Count - 1;

                await _gameFramework.CheckForPassivesAndUpdateStats(account, enemy);
                await _gameFramework.CheckForPassivesAndUpdateStats(enemy, account);
                _accounts.SaveAccounts(Context.User);
            }
            else
            {
                await _command.ReplyAsync(Context, "У тебя нет осьминога! создай его командой **CreateOcto**");
            }
        }

        [Command("CreateSkill", RunMode = RunMode.Async)]
        [Alias("CSS")]
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

                await _command.ReplyAsync(Context, embed1);

                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);

                if (res.Content == "да")
                {
                    await _command.ReplyAsync(Context, $"Ты изменяешь скилл {skill.SpellNameEn}");
                }
                else
                {
                    await _command.ReplyAsync(Context, $"никаких апдейтов. (ты сказал {res.Content})");
                    return;
                }
            }

            await _command.ReplyAsync(Context, "Введи Назваие скилла, у тебя 5 минута.");
            var response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 3000000);
            skill.SpellNameEn = response.ToString();

            var embed = new EmbedBuilder();
            embed.AddField("Введи Номер дерева скилла, у тебя 5 минута", "1 - AD\n2 - DEF\n3 - AGI\n4 - AP");

            await _command.ReplyAsync(Context, embed);


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
            await _command.ReplyAsync(Context, "Готово!");
        }


        [Command("CS", RunMode = RunMode.Async)]
        public async Task CreateSkillDev(ulong skillId, [Remainder] string skillNameRu)
        {
            var skill = _spellAccounts.GetAccount(skillId);

            var messSplit = skillNameRu.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);

            if (messSplit.Length >= 3)
            {
                await ReplyAsync(" only one `-` can be in a string (between `SpellNameRu` and `SpellDescriptionRu`)");
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
                        await ReplyAsync("You fucked up. change `SpellTreeNum` later");
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
        public async Task SeeSkill(ulong skillId)
        {
            try
            {
                var skill = _spellAccounts.GetAccount(skillId);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.AddField($"{skill.SpellNameEn}",
                    $"ID: {skill.SpellId}\nTree: {skill.SpellTreeNum}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}");


                await _command.ReplyAsync(Context, embed);
            }
            catch
            {
                //   await ReplyAsync("Такого скила нету. Наши скиллы начинаються с ид **1000**");
            }
        }

        [Command("AllSkill")]
        [Alias("AllSkills")]
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

            await _command.ReplyAsync(Context, embed);
        }
    }
}