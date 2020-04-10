using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class StatsUser : ModuleBaseCustom
    {

        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;

        public StatsUser(UserAccounts accounts, ServerAccounts serverAccounts)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
        }

        [Command("stats", RunMode = RunMode.Async)]
        [Alias("статы")]
        public async Task Xp()
        {
            try
            {
                var account = _accounts.GetAccount(Context.User);


                //   ("https://cdn.discordapp.com/avatars/" + Context.User.Id + "/" + Context.User.AvatarId + ".png");

                var usedNicks = "";
                var usedNicks2 = "";
                var usedNicks3 = "";
                var usedNicks4 = "";
                if (account.ExtraUserName != null)
                {
                    var extra = account.ExtraUserName.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                    for (var i = 0; i < extra.Length; i++)
                        if (i == extra.Length - 1)
                            usedNicks += extra[i];
                        else if (usedNicks.Length <= 970)
                            usedNicks += extra[i] + ", ";
                        else if (usedNicks2.Length <= 970)
                            usedNicks2 += extra[i] + ", ";
                        else if (usedNicks3.Length <= 970)
                            usedNicks3 += extra[i] + ", ";
                        else if (usedNicks4.Length <= 970)
                            usedNicks4 += extra[i] + ", ";
                }
                else
                {
                    usedNicks = "None";
                }

                var octopuses = "";
                if (account.Octopuses != null)
                {
                    var octo = account.Octopuses.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);


                    for (var i = 0; i < octo.Length; i++)
                        if (i == octo.Length - 1)
                            octopuses += octo[i];
                        else
                            octopuses += octo[i] + ", ";
                }
                else
                {
                    octopuses = "None";
                }

                string[] warns = null;
                if (account.Warnings != null)
                    warns = account.Warnings.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                var statList = account.UserStatistics.ToList();
                ulong allMessages = 0;
                ulong editMesages = 0;
                ulong deletedMessages = 0;
                var mostActiveChannel = "";

                foreach (var t in statList)
                {
                    if (t.Key == "all")
                        allMessages = t.Value;
                    if (t.Key == "updated")
                        editMesages = t.Value;
                    if (t.Key == "deleted")
                        deletedMessages = t.Value;
                }


                var ordered = statList.OrderByDescending(x => x.Value).ToList();
                if (ordered.Count <= 2)
                    mostActiveChannel += "Not yet";
                else
                mostActiveChannel += $"<#{Convert.ToUInt64(ordered[1].Key)}> - {ordered[1].Value} messages";
               
                var embed = new EmbedBuilder();
                var user = Context.User as IGuildUser;

                embed.AddField("ID", "" + Context.User.Id, true);
                embed.AddField("UserName", "" + Context.User, true);
                embed.AddField("Registered", "" + user?.CreatedAt, true);
                embed.AddField("Joined", "" + user?.JoinedAt, true);
                embed.AddField("NickName", "" + Context.User.Mention, true);
                embed.AddField("Octo Points", "" + account.Points, true);
                embed.AddField("Octo Reputation", "" + account.Rep, true);
                embed.AddField("Access LVL", "" + account.OctoPass, true);
                embed.AddField("User LVL", "" + Math.Round(account.Lvl, 2), true);
                embed.AddField("Pull Points", "say: `pull`", true);
                embed.AddField("Best 2048 Game Score", $"{account.Best2048Score}", true);
                if (warns != null)
                    embed.AddField("Warnings", "" + warns.Length, true);
                else
                    embed.AddField("Warnings", "Clear.", true);

                embed.AddField("All Messages", $"{allMessages}", true);
                embed.AddField("All Edited Messages", $"{editMesages}", true);
                embed.AddField("All Deleted Messages", $"{deletedMessages}", true);
                embed.AddField("Channel Most Active In", $"{mostActiveChannel}", true);


                embed.AddField("OctoCollection ", "" + octopuses);
                embed.AddField("Used Nicknames", "" + usedNicks);

                embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                //embed.AddField("Роли", ""+avatar);

                await SendMessAsync( embed);
                if (usedNicks2.Length >= 2)
                {
                    var usedEmbed = new EmbedBuilder();

                    usedEmbed.AddField("Used Nicknames Co-nt:", $"{usedNicks2}");
                    if (usedNicks3.Length >= 2)
                        usedEmbed.AddField("boole?!", $"{usedNicks3}");
                    if (usedNicks4.Length >= 2)
                        usedEmbed.AddField("It's time to stop.", $"{usedNicks4}");
                    usedEmbed.WithColor(Color.Blue);
                    usedEmbed.WithAuthor(Context.User);
                    usedEmbed.WithFooter("lil octo notebook");
                    await SendMessAsync( usedEmbed);
                }
            }
            catch
            {
                //  await ReplyAsync("boo... An error just appear >_< \nTry to use this command properly: **Stats**");
            }
        }

        [Command("stats", RunMode = RunMode.Async)]
        [Alias("Статы")]
        public async Task CheckUser(IGuildUser user)
        {
            try
            {
                var comander = _accounts.GetAccount(Context.User);
                if (comander.OctoPass >= 4 || ((IGuildUser) Context.User).GuildPermissions.ManageMessages)
                {
                    var account = _accounts.GetAccount((SocketUser) user);

                    //   var avatar = ("https://cdn.discordapp.com/avatars/" + user.Id + "/" + user.AvatarId + ".png");

                    var usedNicks = "";
                    var usedNicks2 = "";
                    var usedNicks3 = "";
                    var usedNicks4 = "";
                    if (account.ExtraUserName != null)
                    {
                        var extra = account.ExtraUserName.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                        for (var i = 0; i < extra.Length; i++)
                            if (i == extra.Length - 1)
                                usedNicks += extra[i];
                            else if (usedNicks.Length <= 970)
                                usedNicks += extra[i] + ", ";
                            else if (usedNicks2.Length <= 970)
                                usedNicks2 += extra[i] + ", ";
                            else if (usedNicks3.Length <= 970)
                                usedNicks3 += extra[i] + ", ";
                            else if (usedNicks4.Length <= 970)
                                usedNicks4 += extra[i] + ", ";
                    }
                    else
                    {
                        usedNicks = "None";
                    }


                    var octopuses = "";
                    if (account.Octopuses != null)
                    {
                        var octo = account.Octopuses.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);


                        for (var i = 0; i < octo.Length; i++)
                            if (i == octo.Length - 1)
                                octopuses += octo[i];
                            else
                                octopuses += octo[i] + ", ";
                    }
                    else
                    {
                        octopuses = "None";
                    }

                    var warnings = "None";
                    if (account.Warnings != null)
                    {
                        var warns = account.Warnings.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                        warnings = "";
                        foreach (var t in warns)
                        {
                            warnings += t + "\n";
                        }
                    }

                    var statList = account.UserStatistics.ToList();
                    ulong allMessages = 0;
                    ulong editMesages = 0;
                    ulong deletedMessages = 0;
                    var mostActiveChannel = "N/A";

                    foreach (var t in statList)
                    {
                        if (t.Key == "all")
                            allMessages = t.Value;
                        if (t.Key == "updated")
                            editMesages = t.Value;
                        if (t.Key == "deleted")
                            deletedMessages = t.Value;
                    }

                    var ordered = statList.OrderByDescending(x => x.Value).ToList();
                    
                    mostActiveChannel = $"<#{Convert.ToUInt64(ordered[1].Key)}> - {ordered[1].Value} messages";

                    var embed = new EmbedBuilder();

                    embed.WithColor(Color.Purple);
                    embed.WithAuthor(user);
                    embed.WithFooter("lil octo notebook");
                    embed.AddField("ID", "" + user.Id, true);
                    embed.AddField("UserName", "" + user, true);
                    embed.AddField("Registered", "" + user.CreatedAt, true);
                    embed.AddField("Joined", "" + user.JoinedAt, true);
                    embed.AddField("NickName", "" + user.Mention, true);
                    embed.AddField("Octo Points", "" + account.Points, true);
                    embed.AddField("Octo Reputation", "" + account.Rep, true);
                    embed.AddField("Access LVL", "" + account.OctoPass, true);
                    embed.AddField("User LVL", "" + Math.Round(account.Lvl, 2), true);
                    embed.AddField("Pull Points", "" + account.DailyPullPoints, true);
                    embed.AddField("Best 2048 Game Score", $"{account.Best2048Score}", true);
                    embed.AddField("All Messages", $"{allMessages}", true);
                    embed.AddField("All Edited Messages", $"{editMesages}", true);
                    embed.AddField("All Deleted Messages", $"{deletedMessages}", true);
                    embed.AddField("Channel Most Active In", $"{mostActiveChannel}", true);




                    embed.AddField("Warnings", "" + warnings);

                    embed.AddField("OctoCollection ", "" + octopuses);
                    embed.AddField("Used Nicknames", "" + usedNicks);
                    embed.WithThumbnailUrl(user.GetAvatarUrl());


                    await SendMessAsync( embed);
                    if (usedNicks2.Length >= 2)
                    {
                        var usedEmbed = new EmbedBuilder();

                        usedEmbed.AddField("Used Nicknames Co-nt:", $"{usedNicks2}");
                        if (usedNicks3.Length >= 2)
                            usedEmbed.AddField("boole?!", $"{usedNicks3}");
                        if (usedNicks4.Length >= 2)
                            usedEmbed.AddField("It's time to stop.", $"{usedNicks4}");
                        usedEmbed.WithColor(Color.Blue);
                        usedEmbed.WithAuthor(user);
                        usedEmbed.WithFooter("lil octo notebook");
                        await SendMessAsync( usedEmbed);
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
                //    await ReplyAsync("boo... An error just appear >_< \nTry to use this command properly: **Stats [user_ping(or user ID)]**");
            }
        }
    }
}