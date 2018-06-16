using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using OctoBot.Configs;
using OctoBot.Games.OctoGame.GameSpells;
using OctoBot.Games.OctoGame.GameUsers;
using OctoBot.Handeling;

namespace OctoBot.Games.OctoGame
{
    public class OctoGameCommand : ModuleBase<SocketCommandContext>
    {
        [Command("CreateOcto", RunMode = RunMode.Async)]
        [Alias("UpdateOcto", "OctoCreate")]
        public async Task CreateOctopusFighter()
        {
            var account = GameUserAccounts.GetAccount(Context.User);
            string response = "бу";
            
           
            if (account.OctopusFighterInfo != null)
            {
                await ReplyAsync("У тебя уже есть осьминожка! Если ты хочешь ПОЛНОСТЬЮ обновить информацию осьминожки напиши **да**");
               var res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 6000);
                response = res.Content;
            }

            if (account.OctopusFighterInfo == null ||  response == "да")
            {
             
                account.OctopusFighterInfo = null;
                
                
                await Context.Channel.SendMessageAsync("Введи имя своего осьминожка, у тебя 1 минута.");
                var res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctopusFighterInfo += res.Content + "|";
                account.OctopusFighterName = res.Content;

                await Context.Channel.SendMessageAsync("Введи цвет своего осьминожка, у тебя 1 минута.");
                res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);
                account.OctopusFighterInfo += res.Content + "|";
               

                await Context.Channel.SendMessageAsync("Введи характер своего осьминожка, у тебя 2 минуты.");
                res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctopusFighterInfo += res.Content + "|";

                await Context.Channel.SendMessageAsync("Введи Лор своего осьминожка, у тебя 2 минуты.");
                res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 120000);
                account.OctopusFighterInfo += res.Content + "|";


                account.OctopusFighterStrength = 20;
                account.OctopusFighterAd = 0;
                account.OctopusFighterAp = 0;
                account.OctopusFighterAgility = 0;
                account.OctopusFighterCritDmg = 1.5;
                account.OctopusFighterDodge = 0;
                account.OctopusFighterArmor = 1;
                account.OctopusFighterMagicResist = 1;
                account.OctopusFighterHealth = 100;
                account.OctopusFighterStamina = 200;
                account.OctopusFighterLvl = 1;
                account.OctopusFighterArmPen = 0;
                account.OctopusFighterMagPen = 0;

                GameUserAccounts.SaveAccounts();
                await Context.Channel.SendMessageAsync("Готово! Ты создал или обновил информацию о своем осьминоге!");
            }      
        }

        [Command("OctoInfo")]
        [Alias("InfoOcto", "OctoInfo", "myOcto")]
        public async Task CehckOctopusFighter()
        {
            
            var account = GameUserAccounts.GetAccount(Context.User);
            if (account.OctopusFighterInfo == null)
            {
                await ReplyAsync("У тебя нет осьминожки все ещё! Чтобы создать, впиши команду **CreateOcto**");
                return;
            }

            var octoInfoArray = account.OctopusFighterInfo.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
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
            embed.AddField($"Твой осьминожка!", $"**Имя:** {octoInfoArray[0]}\n**Цвет:** {octoInfoArray[1]}\n**Характер** {octoInfoArray[2]}\n**Лор:** {octoInfoArray[3]}");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("endGame")]
        public async Task EndOctoGameCommand()
        {
            var account = GameUserAccounts.GetAccount(Context.User);
            account.OctopusFightPlayingStatus = 0;
            GameUserAccounts.SaveAccounts();
            await ReplyAsync("Мы закончили твою игру, можешь начинать другую");
        }

        [Command("Fight")]
        public async Task CreateFight()
        {

            var account = GameUserAccounts.GetAccount(Context.User);
            account.CurrentOctopusFighterStrength = account.OctopusFighterStrength;
            account.CurrentOctopusFighterAd =  account.OctopusFighterAd;
            account.CurrentOctopusFighterAp = account.OctopusFighterAp;
            account.CurrentOctopusFighterAgility = account.OctopusFighterAgility;
            account.CurrentOctopusFighterCritDmg = account.OctopusFighterCritDmg;
            account.CurrentOctopusFighterDodge =  account.OctopusFighterDodge;
            account.CurrentOctopusFighterArmor = account.OctopusFighterArmor;
            account.CurrentOctopusFighterMagicResist = account.OctopusFighterMagicResist;
            account.CurrentOctopusFighterHealth = account.OctopusFighterHealth;
            account.CurrentOctopusFighterStamina = account.OctopusFighterStamina;
            account.CurrentOctopusFighterLvl = account.OctopusFighterLvl;
            account.CurrentOctopusFighterArmPen = account.OctopusFighterArmPen;
            account.CurrentOctopusFighterMagPen =  account.OctopusFighterMagPen;
            account.CurrentOctopusFighterSkillSetAd = account.OctopusFighterSkillSetAd;
            account.CurrentOctopusFighterSkillSetDef = account.OctopusFighterSkillSetDef;
            account.CurrentOctopusFighterSkillSetAgi = account.OctopusFighterSkillSetAgi;
            account.CurrentOctopusFighterSkillSetAp = account.OctopusFighterSkillSetAp;
            account.CurrentLogString = null;
            GameUserAccounts.SaveAccounts();

            if (account.OctopusFightPlayingStatus >= 1)
            {
                await ReplyAsync("you are already playing");
                return;
            }

            if (account.CurrentOctopusFighterSkillSetAp == null && account.CurrentOctopusFighterSkillSetAd == null &&
                account.CurrentOctopusFighterSkillSetDef == null && account.CurrentOctopusFighterSkillSetAgi == null)
            {
                await ReplyAsync("You dont have any moves. You can get them using **boole** command");
                return;
            }

            if (account.OctopusFighterInfo != null)
            {

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.WithFooter($"Enemy choice");
                embed.WithColor(Color.DarkGreen);
                embed.AddField("Choose enemy lvl : ", $"{new Emoji("1⃣")} Enemy {account.CurrentOctopusFighterLvl - 1} LvL\n" +
                                                                   $"{new Emoji("2⃣")} Enemy {account.CurrentOctopusFighterLvl} LvL\n" +
                                                                   $"{new Emoji("3⃣")} Enemy {account.CurrentOctopusFighterLvl + 1} LvL\n" +
                                                                   $"{new Emoji("❌")} - End Fight.");
       
                var message = await Context.Channel.SendMessageAsync("", false, embed.Build());

                await message.AddReactionAsync(new Emoji("1⃣"));             
                await message.AddReactionAsync(new Emoji("2⃣"));           
                await message.AddReactionAsync(new Emoji("3⃣"));
                await message.AddReactionAsync(new Emoji("❌"));
                
                var newOctoGame = new Global.OctoGameMessAndUserTrack(message.Id, Context.User.Id, message, Context.User);

         

                Global.OctopusGameMessIdList.Add(newOctoGame);
                Global.OctoGamePlaying += 1;
                account.OctopusFightPlayingStatus = 1;
                GameUserAccounts.SaveAccounts();                        
            }
            else
                await ReplyAsync("У тебя нет осьминога! создай его командой **CreateOcto**");
            
         
        }

        [Command("CreateSkill", RunMode = RunMode.Async)]
        [Alias("CS")]
        public async Task CreateSkill(ulong skillId)
        {
            var skill = SpellUserAccounts.GetAccount(skillId);

            if (skill.SpellName != null)
            {
                var embed1 = new EmbedBuilder();
                embed1.WithAuthor(Context.User);
                embed1.AddField("Этот скилл иди уже существует", $"{skill.SpellName}\nID: {skill.SpellId}\nTree: {skill.SpellTree}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}\n" +
                                                                  "Если хочешь полностью его изменить, напиши **да** (1 минута)");
                await ReplyAsync("", false, embed1.Build());

                var res = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 60000);

                if (res.Content == "да")
                {
                    await ReplyAsync($"Ты изхменяешь скилл {skill.SpellName}");
                }
                else
                {
                    await ReplyAsync($"никкаких апдейтов. (ты сказал {res.Content})");
                    return;
                }

            }

            await Context.Channel.SendMessageAsync("Введи Назваие скилла, у тебя 5 минута.");
            var response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellName = response.ToString();

            var embed = new EmbedBuilder();
            embed.AddField("Введи Номер дерева скилла, у тебя 5 минута", "1 - AD\n2 - DEF\n3 - AGI\n4 - AP");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.SpellTree = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync("Введи Русское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.SpellDescriptionRu = response.ToString();

            await Context.Channel.SendMessageAsync("Введи Английское описание скилла (либо просто **нету**), у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.SpellDescriptionEn = response.ToString();

            var embedAc = new EmbedBuilder();
            embedAc.AddField("Введи Активка или Пассивка, у тебя 5 минута", "0 - Пассив\n1 - Актив");
            await Context.Channel.SendMessageAsync("", false, embedAc.Build());
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);
            skill.ActiveOrPassive = Convert.ToInt32(response.ToString());
            /*
            await Context.Channel.SendMessageAsync("Введи Формулу описание скилла, у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.SpellFormula = response.ToString();
            */
            var embedCd = new EmbedBuilder();      
            embedCd.AddField("Введи КД скилла, у тебя 5 минут", "1)Если есть - в ходах\n2)Если КД = 1 раз в игру, пиши 9999\n3)Если КД нету вообще, пиши 0");
            await Context.Channel.SendMessageAsync("", false, embedCd.Build());
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000);      
            skill.SpellCd = Convert.ToInt32(response.ToString());

            await Context.Channel.SendMessageAsync("Тип урона (AD or AP), у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.SpellDmgType = response.ToString();

            /*
            await Context.Channel.SendMessageAsync("Введи Пойзен (прокает он хит), у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Poisen = response.ToString();
           
            await Context.Channel.SendMessageAsync("Введи ОнХит, у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Onhit = response.ToString();

            await Context.Channel.SendMessageAsync("Введи Бафф, у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.Buff = response.ToString();

            await Context.Channel.SendMessageAsync("Введи ДЕбафф, у тебя 5 минут.");
            response = await CommandHandeling.AwaitMessage(Context.User.Id, Context.Channel.Id, 300000 );
            skill.DeBuff = response.ToString();
            */
            SpellUserAccounts.SaveAccounts();
            await ReplyAsync("Готово!");

            
        }

        [Command("SeeSkill")]
        public async Task SeeSkill(ulong skillId)
        {

            try
            {
                var skill = SpellUserAccounts.GetAccount(skillId);

                var embed = new EmbedBuilder();
                embed.WithAuthor(Context.User);
                embed.AddField($"{skill.SpellName}",
                    $"ID: {skill.SpellId}\nTree: {skill.SpellTree}\nRU: {skill.SpellDescriptionRu}\nEN: {skill.SpellDescriptionEn}\nFormula: {skill.SpellFormula}\nCD: {skill.SpellCd}");

                await ReplyAsync("", false, embed.Build());
            }
            catch
            {
                await ReplyAsync("Такого скила нету. Наши скиллы начинаються с ид **1000**");
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
            for (var i = 0; i < data.Count; i++)
            {
                allSkills += $"{i + 1}. {data[i].SpellName} {data[i].SpellId}\n";

            }

            embed.WithAuthor(Context.User);
            embed.AddField("Все скилы:", $"{allSkills}");

            await ReplyAsync("", false, embed.Build());
        }

    }
}
