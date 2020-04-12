using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class Top : ModuleBaseCustom
    {
        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;
        private readonly Global _global;

        public Top(UserAccounts accounts, ServerAccounts serverAccounts, Global global)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
            _global = global;
        }


        [Command("topo")]
        [Alias("topp")]
        [Summary("Top users by Octo Points")]
        public async Task TopByOctoPoints(int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    await SendMessAsync(
                        "Boole! Try different page <_<");
                    return;
                }

                var currentGuildUsersId = Context.Guild.Users.Select(user => user.Id);
                var accounts = _accounts.GetAllAccount();

                const int usersPerPage = 9;

                var lastPage = 1 + accounts.Count / (usersPerPage + 1);
                if (page > lastPage)
                {
                    await SendMessAsync(
                        $"Boole. Last Page is {lastPage}");
                    return;
                }

                var ordered = accounts.OrderByDescending(acc => acc.Points).ToList();

                var embB = new EmbedBuilder()
                    .WithTitle("Top By Octo Points:")
                    .WithFooter(
                        $"Page {page}/{lastPage} ● Say \"topp 2\" to see second page (you can edit previous message)");


                page--;
                for (var j = 0; j < ordered.Count; j++)
                    if (ordered[j].Id == Context.User.Id)
                        embB.WithDescription(
                            $"**#{j + usersPerPage * page + 1} {Context.User.Username} {ordered[j].Points} OctoPoints**\n**______**");

                for (var i = 1; i <= usersPerPage && i + usersPerPage * page <= ordered.Count; i++)
                {
                    var account = ordered[i - 1 + usersPerPage * page];
                    var user = _global.Client.GetUser(account.Id);
                    embB.AddField($"#{i + usersPerPage * page} {user.Username}", $"{account.Points} OctoPoints", true);
                }

                await SendMessAsync( embB);
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **topp [page_number]**(Top By Activity)\nAlias: topo");
            }
        }


        [Command("top")]
        [Alias("topl")]
        [Summary("Top users  by Activity on Server")]
        public async Task TopByLvL(int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    await SendMessAsync(
                        "Boole! Try different page <_<");
                    return;
                }


                var currentGuildUsersId = Context.Guild.Users.Select(user => user.Id);
                // Get only accounts of this server
                var accounts = _accounts.GetAllAccount();


                foreach (var t in accounts)
                {
                    t.Lvl = Math.Sqrt((double) t.LvlPoinnts / 150);
                }

                const int usersPerPage = 9;

                var lastPage = 1 + accounts.Count / (usersPerPage + 1);
                if (page > lastPage)
                {
                    await SendMessAsync(
                        $"Boole. Last Page is {lastPage}");

                    return;
                }

                var ordered = accounts.OrderByDescending(acc => acc.Lvl).ToList();

                var embB = new EmbedBuilder()
                    .WithTitle("Top By Activity:")
                    .WithFooter(
                        $"Page {page}/{lastPage} ● Say \"top 2\" to see second page (you can edit previous message)");


                page--;
                for (var j = 0; j < ordered.Count; j++)
                    if (ordered[j].Id == Context.User.Id)
                        embB.WithDescription(
                            $"**#{j + usersPerPage * page + 1} {Context.User.Username} {Math.Round(ordered[j].Lvl, 2)} LVL**\n**______**");

                for (var i = 1; i <= usersPerPage && i + usersPerPage * page <= ordered.Count; i++)
                {
                    var account = ordered[i - 1 + usersPerPage * page];
                    var user = _global.Client.GetUser(account.Id);
                    embB.AddField($"#{i + usersPerPage * page} {user.Username}", $"{Math.Round(account.Lvl, 2)} LVL",
                        true);
                }

                await SendMessAsync( embB);
            }
            catch
            {
              //  await ReplyAsync(
              //      "boo... An error just appear >_< \nTry to use this command properly: **top [page_number]**(Top By Activity)\nAlias: topl");
            }
        }

    }
}