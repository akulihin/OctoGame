using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class ReputationAndPointsCommands : ModuleBaseCustom
    {
        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;
        private readonly AwaitForUserMessage _awaitForUserMessage;

        public ReputationAndPointsCommands(UserAccounts accounts, ServerAccounts serverAccounts, AwaitForUserMessage awaitForUserMessage)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
            _awaitForUserMessage = awaitForUserMessage;
        }

        [Command("OctoRep")]
        [Alias("Octo Rep", "Rep", "октоРепа", "Окто Репа", "Репа")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Adding Octo Rep on a Account")]
        public async Task AddPoints(IGuildUser user, long rep)
        {
            var comander = _accounts.GetAccount(Context.User);
            if (comander.OctoPass >= 100)
            {
                var account = _accounts.GetAccount((SocketUser) user);
                account.Rep += rep;
                

                await SendMessAsync(
                    $"{rep} Octo Reputation were credited, altogether {user.Mention} have {account.Rep} Octo Reputation!");
            }
            else
            {
                await SendMessAsync(
                    "Boole! You do not have a tolerance of this level!");
            }
        }

        [Command("OctoPoint")]
        [Alias("Octo Point", "OctoPoints", "Octo Points", "ОктоПоинты", "Окто Поинты", "Поинты", "points", "point")]
        // [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Adding Octo Points on a Account")]
        public async Task GivePoints(IGuildUser user, long points)
        {
            var comander = _accounts.GetAccount(Context.User);
            if (comander.OctoPass >= 100)
            {
                var account = _accounts.GetAccount((SocketUser) user);
                account.Points += points;
                

                await SendMessAsync(
                    $"{points} Octo Points were credited, altogether {user.Mention} have {account.Points} Octo Points!");
            }
            else
            {
                await SendMessAsync(
                    "Boole! You do not have a tolerance of this level!");
            }
        }

        [Command("pass", RunMode = RunMode.Async)]
        [Alias("Пасс", "Купить Пропуск", "Пропуск", "КупитьПропуск", "Доступ")]
        [Summary("Buy a Next LVL Pss to get more features from bot (commands)")]
        public async Task BuyPass()
        {
            try
            {
                var account = _accounts.GetAccount(Context.User);
                var cost = 4000 * (account.OctoPass + 1);

                if (account.Points >= cost)
                {
                    await Context.Channel.SendMessageAsync(
                        $"Are you sure about buying pass #{account.OctoPass + 1} for {cost} Octo Points? Than write **yes**!");
                    var response = await _awaitForUserMessage.AwaitMessage(Context.User.Id, Context.Channel.Id, 6000);

                    if (response.Content == "yes" || response.Content == "Yes")
                    {
                        account.OctoPass += 1;
                        account.Points -= cost;
                        

                        await SendMessAsync(
                            $"Booole! You've Got Access **#{account.OctoPass}**");
                    }
                    else
                    {
                        await SendMessAsync(
                            "You should say `yes` или `Yes` in 6s to get the pass.");
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync(
                        $"You did not earn enough Octo Points, current amount: **{account.Points}**\nFor pass #{account.OctoPass + 1} you will need **{cost}** Octo Points!");
                }
            }
            catch
            {
                // await ReplyAsync("boo... An error just appear >_< \nTry to use this command properly: **pass**\n Alias: Пасс, Пропуск, Доступ, КупитьПропуск, Купить Пропуск");
            }
        }

        [Command("CheckLvlLOL")]
        public async Task Check(uint xp)
        {
            var level = (uint) Math.Sqrt((double) xp / 100);
            await Context.Channel.SendMessageAsync("Это " + level + "сможешь ли ты достичь высот?");
        }

        [Command("GiftPoints")]
        [Alias("Gift Points", "GiftPoint", "Gift Point")]
        public async Task GidftPoints(IGuildUser user, long points)
        {
            try
            {
                var passCheck = _accounts.GetAccount(Context.User);
                if (passCheck.OctoPass >= 1)
                {
                    if (points <= 0)
                    {
                        await SendMessAsync(
                            "You cannot send 0 or -number, boo!");
                        return;
                    }

                    if (passCheck.Points >= points)
                    {
                        var account = _accounts.GetAccount((SocketUser) user);

                        var taxes = points * 0.9;
                        var bot = _accounts.GetAccount(Context.Client.CurrentUser);

                        account.Points += (int) taxes;
                        passCheck.Points -= points;

                        var toBank = points * 1.1 - points;
                        bot.Points += (int) toBank;
                        

                        await SendMessAsync(
                            $"Was transferred{points}\n {user.Mention} now have {account.Points} Octo Points!\nyou have left {passCheck.Points}\ntaxes: {taxes}");
                    }
                    else
                    {
                        await SendMessAsync(
                            $"You do not have enough Octo Points to pass them.");
                    }
                }
                else
                {
                    await SendMessAsync(
                        "Boole! You do not have a tolerance of this level!");
                }
            }
            catch
            {
                //     await ReplyAsync("boo... An error just appear >_< \nTry to use this command properly: **GiftPoints [ping_user(or user ID)] [number_of_points]**\nAlias: GiftPoint ");
            }
        }
    }
}