using System;
using System.Collections.Concurrent;
using System.ComponentModel;
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
    public class StatsServer : ModuleBaseCustom
    {
        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;
        private readonly Global _global;

        public StatsServer(UserAccounts accounts, ServerAccounts serverAccounts, Global global)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
            _global = global;
        }


        [Command("topRoles")]
        [Alias("topr")]
        [Description("Top by Roles (Statistics fore Roles in the Guild)")]
        public async Task TopByRoles(int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    await SendMessAsync(
                        "Boole! Try different page <_<");
                    return;
                }

                var rolesList = Context.Guild.Roles.ToList();

                const int usersPerPage = 8;

                var lastPage = 1 + rolesList.Count / (usersPerPage + 1);
                if (page > lastPage)
                {
                    await SendMessAsync(
                        $"Boole. Last Page is {lastPage}");
                    return;
                }

                var orderedRolesList = rolesList.OrderByDescending(acc => acc.Members.Count()).ToList();

                var embB = new EmbedBuilder()
                    .WithTitle("Top By Roles:")
                    .WithFooter(
                        $"Page {page}/{lastPage} ● Say \"topRoles 2\" to see second page (you can edit previous message)");
                page--;

                for (var i = 1; i <= usersPerPage && i + usersPerPage * page <= orderedRolesList.Count; i++)
                {
                    var num = i + usersPerPage * page - 1;
                    embB.AddField($"#{i + usersPerPage * page} {orderedRolesList[num].Name}",
                        $"**Members:** {orderedRolesList[num].Members.Count()}\n" +
                        $"**Color:** {orderedRolesList[num].Color}\n" +
                        $"**Created:** {orderedRolesList[num].CreatedAt.DateTime}\n" +
                        $"**Is Mentionable:** {orderedRolesList[num].IsMentionable}\n" +
                        $"**Position:** {orderedRolesList[num].Position}\n" +
                        $"**ID:** {orderedRolesList[num].Id}\n\n**_____**", true);
                }

                await SendMessAsync( embB);
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **top [page_number]**(Top By Activity)\nAlias: topl");
            }
        }


        [Command("topChannels")]
        [Alias("topChan", "top Channels", "topChannel", "top Channel")]
        [Description("Top by Roles (Statistics fore Channels in the Guild)")]
        public async Task TopByChannels(int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    await SendMessAsync(
                        "Boole! Try different page <_<");
                    return;
                }

                var guildAccount = _serverAccounts.GetServerAccount(Context.Guild);
                var allTextChannelsList = Context.Guild.TextChannels.ToList();

                var knownTextChannelsList = guildAccount.MessagesReceivedStatisctic.ToList();

                const int usersPerPage = 8;

                var lastPage = 1 + allTextChannelsList.Count / (usersPerPage + 1);
                if (page > lastPage)
                {
                    await SendMessAsync(
                        $"Boole. Last Page is {lastPage}");
                    return;
                }

                guildAccount.MessagesReceivedStatisctic = new ConcurrentDictionary<string, ulong>();
                foreach (var t1 in allTextChannelsList)
                {
                    ulong num = 0;
                    foreach (var t in knownTextChannelsList)
                        if (t1.Id.ToString() == t.Key)
                            num = t.Value;


                    guildAccount.MessagesReceivedStatisctic.AddOrUpdate(t1.Id.ToString(), num, (key, value) => num);
                    
                }

                var orderedKnownChannels = guildAccount.MessagesReceivedStatisctic
                    .OrderByDescending(channels => channels.Value).ToList();

                var embB = new EmbedBuilder()
                    .WithTitle("Top By Activity In Text Channels:")
                    .WithFooter(
                        $"Page {page}/{lastPage} ● Say \"topChan 2\" to see second page (you can edit previous message)")
                    .WithDescription(
                        "If `Messages Count: 0` that means, I can't read the channel, or no one using it.\n");

                page--;

                for (var i = 1; i <= usersPerPage && i + usersPerPage * page <= orderedKnownChannels.Count; i++)
                {
                    var num = i - 1 + usersPerPage * page;
                    SocketTextChannel something = null;
                    foreach (var t in allTextChannelsList)
                        if (orderedKnownChannels[num].Key == t.Id.ToString())
                            something = Context.Guild.GetTextChannel(t.Id);

                    var cat = "error";
                    if (something == null)
                        continue;
                    if (something.Category != null)
                        cat = something.Category.Name;

                    embB.AddField($"#{i + usersPerPage * page} {something.Name}",
                        $"**Messages Count:** {orderedKnownChannels[num].Value}\n" +
                        $"**Average per day:** {orderedKnownChannels[num].Value / 7}\n" +
                        $"**Members:** {something.Users.Count}\n" +
                        $"**Created:** {something.CreatedAt.DateTime}\n" +
                        $"**Category:** {cat}\n" +
                        $"**IsNsfw:** {something.IsNsfw.ToString()}\n" +
                        $"**Position:** {something.Position}\n" +
                        $"**ID:** {something.Id}\n\n**_____**", true);
                }

                await SendMessAsync( embB);
            }
            catch
            {
             //   Console.WriteLine(e.Message);
            }
        }


        
        [Command("sstats")]
        [Alias("ServerStats", "serverInfo")]
        [Description("Showing usefull Server Statistics")]
        public async Task ServerStats(ulong guildId = 0)
        {
            try
            {
                SocketGuild guild;

                if (guildId == 0)
                    guild = Context.Guild;
                else
                    guild = _global.Client.GetGuild(guildId);

                var userAccounts = _accounts.GetOrAddUserAccountsForGuild(guild.Id);
                var orderedByLvlUsers = userAccounts.OrderByDescending(acc => acc.Lvl).ToList();

                var guildAccount = _serverAccounts.GetServerAccount(guild);
                var orderedByChannels =
                    guildAccount.MessagesReceivedStatisctic.OrderByDescending(chan => chan.Value).ToList();


                var aliveUserCount = 0;
                var activeUserCount = 0;
                foreach (var t in userAccounts)
                {
                    if (t.Lvl > 2)
                        aliveUserCount++;
                    if (t.Lvl > 20)
                        activeUserCount++;
                }

                var adminCount = 0;
                var moderCount = 0;
                foreach (var t in userAccounts)
                {
                    var acc = guild.GetUser(t.Id);
                    if (acc == null)
                        continue;
                    if (acc.GuildPermissions.Administrator && !acc.IsBot)
                        adminCount++;
                    if (acc.GuildPermissions.DeafenMembers && acc.GuildPermissions.ManageMessages
                                                           && acc.GuildPermissions.ManageChannels && !acc.IsBot)
                        moderCount++;
                }

                var rolesList = guild.Roles.ToList();
                var orderedRoleList = rolesList.OrderByDescending(rl => rl.Members.Count()).ToList();

                var embed = new EmbedBuilder();
                embed.WithColor(Color.Blue);
                embed.WithAuthor(Context.User);
                embed.AddField($"{guild.Name} Statistic", $"**Created:** {Context.Guild.CreatedAt}\n" +
                                                          $"**Owner:** {Context.Guild.Owner}\n" +
                                                          $"**Verification Level:** {Context.Guild?.VerificationLevel}\n" +
                                                          $"**Users:** {Context.Guild.MemberCount}\n" +
                                                          $"**Alive Users:** {aliveUserCount}\n" +
                                                          $"**Active Users:** {activeUserCount}\n" +
                                                          $"**Admins:** {adminCount}\n" +
                                                          $"**Moderators:** {moderCount}\n");
                if(orderedByLvlUsers.Count >= 3 && orderedByChannels.Count >= 4 && orderedRoleList.Count >= 5) {
                    embed.AddField("**______**", "**Top 3 Active users:**\n" +
                                                 $"{Context.Guild.GetUser(orderedByLvlUsers[0].Id).Mention} - {Math.Round(orderedByLvlUsers[0].Lvl, 2)} LVL\n" +
                                                 $"{Context.Guild.GetUser(orderedByLvlUsers[1].Id).Mention} - {Math.Round(orderedByLvlUsers[1].Lvl, 2)} LVL\n" +
                                                 $"{Context.Guild.GetUser(orderedByLvlUsers[2].Id).Mention} - {Math.Round(orderedByLvlUsers[2].Lvl, 2)} LVL\n" +
                                                 "(to see all - say `top`)\n" +
                                                 "(to see a user statistic say `stats @user`\n\n" +
                                                 "**Top 4 Channels:**\n" +
                                                 $"{Context.Guild.GetTextChannel(Convert.ToUInt64(orderedByChannels[0].Key)).Mention} - {orderedByChannels[0].Value} Messages\n" +
                                                 $"{Context.Guild.GetTextChannel(Convert.ToUInt64(orderedByChannels[1].Key)).Mention} Messages\n" +
                                                 $"{Context.Guild.GetTextChannel(Convert.ToUInt64(orderedByChannels[2].Key)).Mention} Messages\n" +
                                                 $"{Context.Guild.GetTextChannel(Convert.ToUInt64(orderedByChannels[3].Key)).Mention} Messages\n" +
                                                 $"All Messages: {guildAccount.MessagesReceivedAll}\n" +
                                                 "(to see all - say `topChan`)\n\n" +
                                                 "**Top 4 Roles:**\n" +
                                                 $"{orderedRoleList[1]?.Mention} - {orderedRoleList[1]?.Members.Count()} Members\n" +
                                                 $"{orderedRoleList[2]?.Mention} - {orderedRoleList[2]?.Members.Count()} Members\n" +
                                                 $"{orderedRoleList[3]?.Mention} - {orderedRoleList[3]?.Members.Count()} Members\n" +
                                                 $"{orderedRoleList[4]?.Mention} - {orderedRoleList[4]?.Members.Count()} Members\n" +
                                                 "(to see all - say `topRoles`");
                }
                else
                {
                    await ReplyAsync("**ERROR**\n" +
                                     "Not everything is displayed:\n" +
                                     "You need to have at least 3 users, 4 channels with messages and 4 roles to be able to use this command correctly");
                }
                embed.AddField("**______**", $"**Text Channels Count:** {Context.Guild?.TextChannels.Count}\n" +
                                             $"**Voice Channels Count:** {Context.Guild?.VoiceChannels.Count}\n" +
                                             $"**All Channels and Category Count:** {Context.Guild?.Channels.Count}\n" +
                                             $"**Roles Count:** {Context.Guild.Roles?.Count}\n" +
                                             $"**AFK Channel:** {Context.Guild?.AFKChannel} ({Context.Guild?.AFKTimeout} Timeout)\n");
                embed.WithThumbnailUrl(guild.IconUrl);
                embed.WithCurrentTimestamp();


                await SendMessAsync( embed);
            }
            catch 
            {
             //   Console.WriteLine(e.Message);
            }
        }

    }
}