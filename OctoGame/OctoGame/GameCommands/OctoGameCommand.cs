using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using OctoGame.Accounts.GameSpells;
using OctoGame.Accounts.Users;
using OctoGame.Framework;
using OctoGame.Framework.Library;
using OctoGame.Helpers;

namespace OctoGame.OctoGame.GameCommands
{
    public class OctoGameCommand : ModuleBase<SocketCommandContextCustom>
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;
        private readonly CommandHandeling _command;

        public OctoGameCommand(IUserAccounts accounts, ISpellAccounts spellAccounts, Global global, AwaitForUserMessage awaitForUserMessage, CommandHandeling command)
        {
            _accounts = accounts;
            _spellAccounts = spellAccounts;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;
            _command = command;
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
            if(user == null)
             account = _accounts.GetAccount(Context.User);
            else
            account = _accounts.GetAccount(user);
            _accounts.SaveAccounts(Context.User);
            if (account.OctoInfo == null)
            {
                await _command.ReplyAsync(Context, "У тебя нет осьминожки все ещё! Чтобы создать, впиши команду **CreateOcto**");
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
            embed.AddField($"Твой осьминожка!",
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
            var account = _accounts.GetBotAccount(userId);

            account.MoveListPage = 1;
            account.MaxHealth = account.Health;
            account.CurrentEnemy = 1;
            account.Health = Math.Ceiling(100.0); //  ONLY ITEMS + SKILLS
            account.Stamina = Math.Ceiling(100 + 3 * Convert.ToDouble(account.OctoLvL-1)); 
            account.Strength = Math.Ceiling(20.0) ; // ONLY ITEMS + SKILLS
            account.AD_Stats = Math.Ceiling(account.Strength * (0.2 * account.OctoLvL));  // + ITEMS + SKILLS
            account.AP_Stats = Math.Ceiling(10 + 0.1*account.OctoLvL); // +  ITEMS + SKILLS
            account.AG_Stats = Math.Ceiling(1.0); // ONLY ITEMS + SKILLS
            account.CritDmg = Math.Ceiling(150.0); // 250 MAX ONLY ITEMS + SKILLS
            account.CritChance = Math.Ceiling(account.AG_Stats); // + SKILLS
            account.DodgeChance = Math.Ceiling(account.AG_Stats - 1); // + SKILLS
            account.Armor = Math.Ceiling(1.0); // 1-6 MAX + ITEMS + SKILLS
            account.Resist = Math.Ceiling(1.0);// 1-6 MAX + ITEMS + SKILLS
            account.ArmPen = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.MagPen = Math.Ceiling(0.0); // 1-5 MAX ONLY ITEMS + SKILLS
            account.OnHit = Math.Ceiling((account.OctoLvL/80+1) * (account.AG_Stats/4+1)); // lvl/100 * (1(agility/2)) + ITEMS + SKILLS
            account.CurrentLogString = "";
            account.Debuff = new List<AccountSettings.Cooldown>();
            account.Buff = new List<AccountSettings.Cooldown>();
            account.DamageOnTimer = new List<AccountSettings.DmgWithTimer>();
            account.DebuffInTime = new List<AccountSettings.DmgWithTimer>();
            account.OctoItems = new List<AccountSettings.ArtifactEntities>();
            account.SkillCooldowns = new List<AccountSettings.Cooldown>();
            account.MaxStamina = account.Stamina;
            

            if (account.Passives == null)
            account.Passives = "";
           
           
            var passives = account.Passives.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
         
            foreach (var passive in passives)
            {
                account.Buff.Add(new AccountSettings.Cooldown( Convert.ToUInt64(passive), 9999));
            }

            _accounts.SaveAccounts(userId);
            
            return account;
        }

        [Command("Fight")]
        public async Task CreateFight()
        {

            var account = SetupOctopusStats(Context.User.Id);
            var enemy = SetupOctopusStats(account.CurrentEnemy);
          

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
                embed.WithFooter($"Enemy choice");
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
                _accounts.SaveAccounts(Context.User);
            }
            else
            {
                await _command.ReplyAsync(Context, "У тебя нет осьминога! создай его командой **CreateOcto**");
            }
        }

        [Command("CreateSkill", RunMode = RunMode.Async)]
        [Alias("CS")]
        public async Task CreateSkill(ulong skillId)
        {
            var skill = _spellAccounts.GetAccount(skillId);

            if (skill.SpellName != null)
            {
                var embed1 = new EmbedBuilder();
                embed1.WithAuthor(Context.User);
                embed1.AddField("Этот скилл иди уже существует",
                    $"{skill.SpellName}\nID: {skill.SpellId}\nTree: {skill.SpellTree}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}\n" +
                    "Если хочешь полностью его изменить, напиши **да** (1 минута)");

                await _command.ReplyAsync(Context, embed1);

                var res = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);

                if (res.Content == "да")
                {
                    await _command.ReplyAsync(Context, $"Ты изхменяешь скилл {skill.SpellName}");
                }
                else
                {
                    await _command.ReplyAsync(Context, $"никкаких апдейтов. (ты сказал {res.Content})");
                    return;
                }
            }

            await _command.ReplyAsync(Context, "Введи Назваие скилла, у тебя 5 минута.");
            var response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellName = response.ToString();

            var embed = new EmbedBuilder();
            embed.AddField("Введи Номер дерева скилла, у тебя 5 минута", "1 - AD\n2 - DEF\n3 - AGI\n4 - AP");

            await _command.ReplyAsync(Context, embed);


            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellTree = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync(
                "Введи Русское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellDescriptionRu = response.ToString();

            await Context.Channel.SendMessageAsync(
                "Введи Английское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellDescriptionEn = response.ToString();

            var embedAc = new EmbedBuilder();
            embedAc.AddField("Введи Активка или Пассивка, у тебя 5 минута", "0 - Пассив\n1 - Актив");
            await Context.Channel.SendMessageAsync("", false, embedAc.Build());
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.ActiveOrPassive = Convert.ToInt32(response.ToString());
            /*
            await Context.Channel.SendMessageAsync("Введи Формулу описание скилла, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.SpellFormula = response.ToString();
            */
            var embedCd = new EmbedBuilder();
            embedCd.AddField("Введи КД скилла, у тебя 5 минут",
                "1)Если есть - в ходах\n2)Если КД = 1 раз в игру, пиши 9999\n3)Если КД нету вообще, пиши 0");
            await Context.Channel.SendMessageAsync("", false, embedCd.Build());
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellCd = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync("Тип урона (AD or AP), у тебя 5 минут.");
            response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellDmgType = response.ToString();

            /*
            await Context.Channel.SendMessageAsync("Введи Пойзен (прокает он хит), у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Poisen = response.ToString();
           
            await Context.Channel.SendMessageAsync("Введи ОнХит, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Onhit = response.ToString();

            await Context.Channel.SendMessageAsync("Введи Бафф, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Buff = response.ToString();

            await Context.Channel.SendMessageAsync("Введи ДЕбафф, у тебя 5 минут.");
            response = await AwaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.DeBuff = response.ToString();
            */
            _spellAccounts.SaveAccounts();
            await _command.ReplyAsync(Context, "Готово!");
        }

        [Command("SeeSkill")]
        public async Task SeeSkill(ulong skillId)
        {
            try
            {
                var skill = _spellAccounts.GetAccount(skillId);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.AddField($"{skill.SpellName}",
                    $"ID: {skill.SpellId}\nTree: {skill.SpellTree}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}");

               
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
            for (var i = 0; i < data.Count; i++) allSkills += $"{i + 1}. {data[i].SpellName} {data[i].SpellId}\n";

            embed.WithAuthor(Context.User);
            embed.AddField("Все скилы:", $"{allSkills}");

            await _command.ReplyAsync(Context, embed);
        }
    }
}