using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class ServerActivityLogger : IServiceSingleton
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public Task InitializeAsync()
            => Task.CompletedTask;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;
        private readonly DiscordShardedClient _client;
        private readonly IServiceProvider _services;

        public ServerActivityLogger(DiscordShardedClient client, IServiceProvider services, Global global, UserAccounts accounts, ServerAccounts serverAccounts)
        {
            _client = client;
            _services = services;
            _global = global;
            _accounts = accounts;
            _serverAccounts = serverAccounts;
        }

        public async Task Client_MessageRecivedForServerStatistics(SocketMessage mess)
        {
            if (mess.Author.IsBot)
                return;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var guild = mess.Channel as IGuildChannel;
            if (guild?.Guild.Id == 338355570669256705 && mess.Channel.Id == 421637061787779072)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Color.Green);
                embed.WithAuthor(mess.Author);
                embed.AddField($"{mess.Author.Username}", $"{mess.Content}");

                if (mess.Content.Length > 1)
                    _global.Client.GetGuild(375104801018609665).GetTextChannel(465290463507775490)
                        .SendMessageAsync("", false, embed.Build());

                if (mess.Attachments.Any())
                {
                    await _client.GetGuild(375104801018609665).GetTextChannel(465290463507775490)
                        .SendMessageAsync($"{mess.Attachments.FirstOrDefault()?.Url}");
                }
            }
            else if (guild?.Guild.Id == 375104801018609665 && mess.Channel.Id == 465290463507775490)
            {
                if (mess.Content.Length > 1)
                    _global.Client.GetGuild(338355570669256705).GetTextChannel(421637061787779072)
                        .SendMessageAsync($"{mess.Content}");

                if (mess.Attachments.Any())
                {
                    await _client.GetGuild(338355570669256705).GetTextChannel(421637061787779072)
                        .SendMessageAsync($"{mess.Attachments.FirstOrDefault()?.Url}");
                }
            }






            if (guild?.Guild.Id == 338355570669256705 && mess.Channel.Id == 374635063976919051)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Color.Green);
                embed.WithAuthor(mess.Author);
                embed.AddField($"{mess.Author.Username}", $"{mess.Content}");

                if (mess.Content.Length > 1)
                    _global.Client.GetGuild(375104801018609665).GetTextChannel(468227956230324224)
                        .SendMessageAsync("", false, embed.Build());

                if (mess.Attachments.Any())
                {
                    await _client.GetGuild(375104801018609665).GetTextChannel(468227956230324224)
                        .SendMessageAsync($"{mess.Attachments.FirstOrDefault()?.Url}");
                }
            }
            else if (guild?.Guild.Id == 375104801018609665 && mess.Channel.Id == 468227956230324224)
            {
                if (mess.Content.Length > 1)
                    _global.Client.GetGuild(338355570669256705).GetTextChannel(374635063976919051)
                        .SendMessageAsync($"{mess.Content}");

                if (mess.Attachments.Any())
                {
                    await _client.GetGuild(338355570669256705).GetTextChannel(374635063976919051)
                        .SendMessageAsync($"{mess.Attachments.FirstOrDefault()?.Url}");
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (mess.Channel is IGuildChannel channel)
            {
                var serverAcc = _serverAccounts.GetServerAccount((SocketGuild) channel.Guild);
                serverAcc.MessagesReceivedStatisctic.AddOrUpdate(channel.Id.ToString(), 1, (key, oldValue) => oldValue + 1);
                
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Write Log to Channel
        /// VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
        /// VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV
        public async Task MessageReceivedDownloadAttachment(SocketMessage arg)
        {
            try
            {
                if (arg.Attachments.Count == 1)
                {
                    var ll = arg.Channel as IGuildChannel;

                    Directory.CreateDirectory(@"OctoDataBase/OctoAttachments");
                    Directory.CreateDirectory($@"OctoDataBase/OctoAttachments/{ll?.GuildId}");

                    var temp = arg.Attachments.FirstOrDefault()?.Url;
                    if (!arg.Attachments.Any())
                        return;
                    var check = $"{temp?.Substring(temp.Length - 8, 8)}";
                    var output = check.Substring(check.IndexOf('.') + 1);

                    if (output == "png" || output == "jpg" || output == "gif")
                        using (var client = new WebClient())
                        {
                            client.DownloadFileAsync(new Uri(arg.Attachments.FirstOrDefault()?.Url),
                                $@"OctoDataBase/OctoAttachments/{ll?.GuildId}/{arg.Id}.{output}");
                        }
                    else
                        using (var client = new WebClient())
                        {
                            client.DownloadFileAsync(new Uri(arg.Attachments.FirstOrDefault()?.Url),
                                $@"OctoDataBase/OctoAttachments/{ll?.GuildId}/{arg.Id}.{output}");
                        }
                }
                else
                {
                    for (var i = 0; i < arg.Attachments.Count; i++)
                    {
                        var ll = arg.Channel as IGuildChannel;

                        Directory.CreateDirectory(@"OctoDataBase/OctoAttachments");
                        Directory.CreateDirectory($@"OctoDataBase/OctoAttachments/{ll?.GuildId}");

                        var temp = arg.Attachments.ToList();

                        var check = $"{temp[i].Url.Substring(temp[i].Url.Length - 8, 8)}";
                        var output = check.Substring(check.IndexOf('.') + 1);

                        if (output == "png" || output == "jpg" || output == "gif")
                            using (var client = new WebClient())
                            {
                                client.DownloadFileAsync(new Uri(temp[i].Url),
                                    $@"OctoDataBase/OctoAttachments/{ll?.GuildId}/{arg.Id}-{i + 1}.{output}");
                            }
                        else
                            using (var client = new WebClient())
                            {
                                client.DownloadFileAsync(new Uri(temp[i].Url),
                                    $@"OctoDataBase/OctoAttachments/{ll?.GuildId}/{arg.Id}-{i + 1}.{output}");
                            }
                    }
                }

                if (arg.Attachments.Count >= 1)
                {
                    var text = "";
                    var attachList = arg.Attachments.ToList();
                    var chacn = arg.Channel as IGuildChannel;

                    foreach (var t in attachList) text += $"{t.Url}\n";

                    _global.Client.GetGuild(375104801018609665).GetTextChannel(466478954887512076)
                        .SendMessageAsync($"{text} ({chacn?.Guild.Name} ({chacn?.Name}))");
                }
            }
            catch
            {
                //
            }
        }

        public async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.Id == _client.CurrentUser.Id)
                return;

            MessageReceivedDownloadAttachment(arg);
        }

        public async Task ChannelDestroyed(IChannel arg)
        {
            try
            {
                if (arg is IGuildChannel currentIguildChannel)
                {
                    var octoBot = currentIguildChannel.Guild.GetUserAsync(_global.Client.CurrentUser.Id);
                    var guild = _serverAccounts.GetServerAccount(currentIguildChannel);

                    if (guild.ServerActivityLog != 1)
                        return;
                    if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                    {
                        await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("View Audit Log Permissions Missing.");
                        return;
                    }

                    var embed = new EmbedBuilder();
                    embed.WithColor(181, 116, 67);

                    if (arg is ITextChannel channel)
                    {
                       var log = await channel.Guild.GetAuditLogsAsync(1);
                        var audit = log.ToList();
                        var name = audit[0].Action == ActionType.ChannelDeleted ? audit[0].User.Mention : "error";
                        var auditLogData = audit[0].Data as ChannelDeleteAuditLogData;
                       
                        embed.AddField("🚫 Channel Destroyed", $"Name: {arg.Name}\n" +
                                                               $"WHO: {name}\n" +
                                                               $"Type {auditLogData?.ChannelType}\n" +
                                                               $"NSFW: {channel.IsNsfw}\n" +
                                                               $"Category: {channel.GetCategoryAsync()?.Result?.Name}\n" +
                                                               $"ID: {arg.Id}\n");
                        embed.WithTimestamp(DateTimeOffset.UtcNow);
                        embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");
                    }

                    if (arg is SocketVoiceChannel voiceChannel)
                    {
                        embed.AddField("🚫 Voice Channel Destroyed", $"Name: {arg.Name}\n" +
                                                                     $"Typ: Voice Channel\n" +
                                                                     $"Limit: {voiceChannel.UserLimit}\n" +
                                                                     $"Category: {voiceChannel.Category}\n" +
                                                                     $"Created: {voiceChannel.CreatedAt.DateTime}\n" +
                                                                     $"Position: {voiceChannel.Position}" +
                                                                     $"ID: {arg.Id}\n");
                        embed.WithTimestamp(DateTimeOffset.UtcNow);
                    }

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                //ignored
            }
        }

        public async Task Client_ChannelDestroyed(IChannel arg)
        {
            ChannelDestroyed(arg);
        }

        public async Task ChannelCreated(IChannel arg)
        {
            try
            {
                var embed = new EmbedBuilder();
                if (arg is ITextChannel channel)
                {
                    var octoBot = channel.Guild.GetUserAsync(_global.Client.CurrentUser.Id);
                    var guild = _serverAccounts.GetServerAccount(channel);

                    if (guild.ServerActivityLog != 1)
                        return;
                    if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                    {
                        await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("View Audit Log Permissions Missing.");
                        return;
                    }

                    var log = await channel.Guild.GetAuditLogsAsync(1);
                    var audit = log.ToList();


                    var name = audit[0].Action == ActionType.ChannelCreated ? audit[0].User.Mention : "error";
                    var auditLogData = audit[0].Data as ChannelCreateAuditLogData;


                    embed.WithColor(14, 243, 247);
                    embed.AddField("📖 Channel Created", $"Name: {arg.Name}\n" +
                                                         $"WHO: {name}\n" +
                                                         $"Type: {auditLogData?.ChannelType.ToString()}\n" +
                                                         $"NSFW: {channel.IsNsfw}\n" +
                                                         $"Category: {channel.GetCategoryAsync().Result.Name}\n" +
                                                         $"ID: {arg.Id}\n");
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
                else if (arg is IVoiceChannel voiceChannel)
                {
                    var octoBot = voiceChannel.Guild.GetUserAsync(_global.Client.CurrentUser.Id);
                    var guild = _serverAccounts.GetServerAccount(voiceChannel);

                    if (guild.ServerActivityLog != 1)
                        return;
                    if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                    {
                        await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("View Audit Log Permissions Missing.");
                        return;
                    }

                    var log = await voiceChannel.Guild.GetAuditLogsAsync(1);
                    var audit = log.ToList();

                    var name = audit[0].Action == ActionType.ChannelCreated ? audit[0].User.Mention : "error";
                    var auditLogData = audit[0].Data as ChannelCreateAuditLogData;


                    embed.WithColor(14, 243, 247);
                    embed.AddField("📖 Channel Created", $"Name: {arg.Name}\n" +
                                                         $"WHO: {name}\n" +
                                                         $"Type: {auditLogData?.ChannelType.ToString()}\n" +
                                                         $"Limit: {voiceChannel.UserLimit}\n" +
                                                         $"Category: {voiceChannel.GetCategoryAsync().Result.Name}\n" +
                                                         $"ID: {arg.Id}\n");
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                // ignored
            }
        }

        public async Task Client_ChannelCreated(IChannel arg)
        {
            ChannelCreated(arg);
        }

        public async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            try
            {
                if (after == null || before == after || before.IsBot)
                    return;
                
                var octoBot = ((IGuild) after.Guild).GetUserAsync(_global.Client.CurrentUser.Id);
                var guild = _serverAccounts.GetServerAccount(after.Guild);

                if (guild.ServerActivityLog != 1)
                    return;
                if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("View Audit Log Permissions Missing.");
                    return;
                }


                var embed = new EmbedBuilder();
                if (before.Nickname != after.Nickname)
                {
                    var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                    var audit = log.ToList();
                    var beforeName = before.Nickname ?? before.Username;

                    var afterName = after.Nickname ?? after.Username;

                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 Nickname Changed:",
                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        "Before:\n" +
                        $"**{beforeName}**\n" +
                        "After:\n" +
                        $"**{afterName}**");
                    if (audit[0].Action == ActionType.MemberUpdated)
                        embed.AddField("WHO:", $"{audit[0].User.Mention}\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");


                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());


                    var userAccount = _accounts.GetAccount(after);
                    var user = after;
                    if (userAccount.ExtraUserName != null)
                    {
                        var dublicate = 0;
                        var extra = userAccount.ExtraUserName.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var t in extra)
                            if (t == user.Nickname && t != null)
                                dublicate = 1;

                        if (dublicate != 1 && user.Nickname != null)
                            userAccount.ExtraUserName += user.Nickname + "|";
                    }
                    else if (user.Nickname != null)
                    {
                        userAccount.ExtraUserName = user.Nickname + "|";
                    }

                    
                }

                if (before.GetAvatarUrl() != after.GetAvatarUrl())
                {
                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 Avatar Changed:",
                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        "Before:\n" +
                        $"**{before.GetAvatarUrl()}**\n" +
                        "After:\n" +
                        $"**{after.GetAvatarUrl()}**");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }

                if (before.Username != after.Username || before.Id != after.Id)
                {
                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField("💢 USERNAME Changed:",
                        $"Server: **{before.Guild.Name}**\n" +
                        "Before:\n" +
                        $"**{before.Username} {before.Id}**\n" +
                        "After:\n" +
                        $"**{after.Username} {after.Id}**\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }

                if (before.Roles.Count != after.Roles.Count)
                {
                    string roleString;
                    var list1 = before.Roles.ToList();
                    var list2 = after.Roles.ToList();
                    var role = "";
                    if (before.Roles.Count > after.Roles.Count)
                    {
                        roleString = "Removed";
                        var differenceQuery = list1.Except(list2);
                        var socketRoles = differenceQuery as SocketRole[] ?? differenceQuery.ToArray();
                        role = socketRoles.Aggregate(role, (current, t) => current + t + "\n");
                    }
                    else
                    {
                        roleString = "Added";
                        var differenceQuery = list2.Except(list1);
                        var socketRoles = differenceQuery as SocketRole[] ?? differenceQuery.ToArray();
                        role = socketRoles.Aggregate(role, (current, t) => current + t + "\n");
                        if (role == "LoL")
                            await _client.GetGuild(338355570669256705).GetTextChannel(429345059486564352)
                                .SendMessageAsync(
                                    $"Буль тебе, {after.Mention}! Если ты новенький в этом мире, то ты можешь попросить у нас реферальную ссылку, чтобы получить **сразу 50 персов на аккаунт**\n" +
                                    "А если ты профи, то можешь попробовать спросить mylorik аккаунт с персонажами, на время, разумеется.");
                    }

                    var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                    var audit = log.ToList();

                    embed.WithColor(255, 255, 0);
                    embed.WithTimestamp(DateTimeOffset.UtcNow);
                    embed.AddField($"👑 Role Update (Role {roleString}):",
                        $"User: **{before.Username} {before.Id}**\n" +
                        $"Server: **{before.Guild.Name}**\n" +
                        $"Role ({roleString}): **{role}**");
                    if (audit[0].Action == ActionType.MemberRoleUpdated)
                        embed.AddField("WHO:", $"{audit[0].User.Mention}\n");
                    embed.WithThumbnailUrl($"{after.GetAvatarUrl()}");

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                Console.WriteLine("boole");
            }
        }

        public async Task Client_GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            GuildMemberUpdated(before, after);
        }

        public async Task JoinedGuild(SocketGuild arg)
        {
            //<:octo_hi:371424193008369664>
            //<:octo_ye:365703699601031170>
            //<:octo_shrug_v0_2:434100874889920522>
            try
            {
                var emoji = Emote.Parse("<:octo_ye:465374379048435712>");

                var embed = new EmbedBuilder();
                await _client.GetGuild(375104801018609665).GetTextChannel(460612886188916736).SendMessageAsync(
                    $"<@181514288278536193> OctoBot have been connected to {arg.Name}({arg.MemberCount}) - {arg.Id}");

                embed.WithColor(Color.Blue);
                embed.WithFooter("Boole.");
                embed.AddField("Boole!",
                    $"{new Emoji("<:octo_hi:465374417644552192>")} We are **Octopuses** and we do many thing, you may check it via `Help` commands\n" +
                    $"**_____**\n" +
                    $"**Set Prefix:** `{_client.CurrentUser.Mention} setPrefix whatever_you_want`\n" +
                    "**Set Channel for logs:** `SetLog` OR `SetLog #channel`(I can logg ANY files and even 2000 lenght messages), `offLog` to turn it off\n" +
                    "**Set Role On Join:** `RoleOnJoin role` will give the role every user who joined the server\n" +
                    "**Set Commands To Get Roles:** `add KeyName RoleName` where **KeyName** anything you want(even emoji), and **RoleName** is a role, you want to get by using `*KeyName`\n" +
                    "So now, by saying ***KeyName** you will get **RoleName** role if it exist\n");
                embed.AddField("Main Features", "1) Full logging\n" +
                                                "2) Reminders \n" +
                                                "3) command `Octo` or `oct`\n" +
                                                "4) command `pull` -  points to get steam game\n" +
                                                "5) command `voice` - will create a voice channel, the owner will have full permissions on this channel and if no one using it - gets deleted in 10 minutes\n" +
                                                "6) commands `top` - will give you statistics about the server or users ( learn more using help)\n" +
                                                "7) command `help` A lot of help.");
                embed.AddField("_____",
                    "**You can edit your previous commands, and OctoBot will edit previous response to that command**, so you don't have to spam Channels with messages\n" +
                    "We need an admin role (see channel, manage emojis, messages, roles, channels, **Audit log** access, etc...) to logg all info to `SetLog`, otherwise, I will not log anything.\n" +
                    "**______**\n" +
                    "If you have **any** questions, suggestions, wishes or need fix please DM  mylorik#2828\n" +
                    $"Also, `boole` is bot's language, pronunciation is same to `boolean` without that `an` sound 🐙");

                var mess = await arg.DefaultChannel.SendMessageAsync("", false, embed.Build());

                await mess.AddReactionAsync(emoji);
            }
            catch
            {
             //   Console.WriteLine(e.Message);
            }
        }

        public async Task Client_JoinedGuild(SocketGuild arg)
        {
            JoinedGuild(arg);
        }

        public async Task Client_Connected()
        {
            await _client.GetGuild(375104801018609665).GetTextChannel(460612886188916736)
                .SendMessageAsync("OctoBot on Duty!");
        }

        public async Task Client_Disconnected(Exception arg)
        {
            /*
            _client.Ready -= GreenBuuTimerClass.StartTimer; ////////////// Timer1 Green Boo starts
            _client.Ready -= DailyPull.CheckTimerForPull; ////////////// Timer3 For Pulls   
            _client.Ready -= Reminder.CheckTimer; ////////////// Timer4 For For Reminders
            _client.Ready -= ForBot.TimerForChangeBotAvatar;
            _client.Ready -= _client_Ready;
            */
            await _client.GetGuild(375104801018609665).GetTextChannel(454435962089373696)
                .SendMessageAsync($"OctoBot Disconnect: {arg.Message}");
            // await LogOwnerTextChannel.SendMessageAsync($"<@181514288278536193> Disconnect!");
        }



        public async Task MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            try
            {
                if (!(arg3 is IGuildChannel currentIGuildChannel))
                    return;
                var after = messageAfter as IUserMessage;
                var octoBot = currentIGuildChannel.GetUserAsync(_global.Client.CurrentUser.Id);
                var guild = _serverAccounts.GetServerAccount(currentIGuildChannel);

               // if(messageAfter.Content.ToArray().Except(messageBefore.Value.Content.ToArray()).Count() < guild.LoggingMessEditIgnoreChar)
               //     return;


               if (guild.ServerActivityLog != 1)
                    return;
                if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("View Audit Log Permissions Missing.");
                    return;
                }

                if (messageAfter.Author.IsBot)
                    return;
                if (messageAfter.Content == null) return;

                var before = (messageBefore.HasValue ? messageBefore.Value : null) as IUserMessage;
                if (before == null)
                    return;
                if (before.Content == after?.Content)
                    return;

                var embed = new EmbedBuilder();
                embed.WithColor(Color.Green);
                embed.WithFooter($"MessId: {messageBefore.Id}");
                embed.WithThumbnailUrl($"{messageBefore.Value.Author.GetAvatarUrl()}");
                embed.WithTimestamp(DateTimeOffset.UtcNow);
                embed.WithTitle("📝 Updated Message");
                embed.WithDescription($"Where: <#{before.Channel.Id}>" +
                                      $"\nMess Author: **{after?.Author}**\n");


                if (messageBefore.Value.Content.Length > 1000)
                {
                    var string1 = messageBefore.Value.Content.Substring(0, 1000);

                    embed.AddField("Before:", $"{string1}");

                    if (messageBefore.Value.Content.Length <= 2000)
                    {
                        var string2 =
                            messageBefore.Value.Content.Substring(1000, messageBefore.Value.Content.Length - 1000);
                        embed.AddField("Before: Continued", $"...{string2}");
                    }
                }
                else if (messageBefore.Value.Content.Length != 0)
                {
                    embed.AddField("Before:", $"{messageBefore.Value.Content}");
                }

                if (messageAfter.Content.Length > 1000)
                {
                    var string1 = messageAfter.Content.Substring(0, 1000);

                    embed.AddField("After:", $"{string1}");

                    if (messageAfter.Content.Length <= 2000)
                    {
                        var string2 =
                            messageAfter.Content.Substring(1000, messageAfter.Content.Length - 1000);
                        embed.AddField("After: Continued", $"...{string2}");
                    }
                }
                else if (messageAfter.Content.Length != 0)
                {
                    embed.AddField("After:", $"{messageAfter.Content}");
                }

                if (messageBefore.Value.Attachments.Any())
                {
                    var temp = messageBefore.Value.Attachments.FirstOrDefault()?.Url;
                    var check2 = $"{temp?.Substring(temp.Length - 8, 8)}";
                    var output = check2.Substring(check2.IndexOf('.') + 1);
                    var guildChannel = (IGuildChannel) arg3;

                    if (messageBefore.Value.Attachments.Count == 1)
                    {
                        if (output == "png" || output == "jpg" || output == "gif")
                        {
                            embed.WithImageUrl(
                                $"attachment://{Path.GetFileName($"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{output}")}");
                            if (guild.ServerActivityLog == 1)
                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{output}",
                                        "",
                                        embed: embed.Build());
                        }
                        else
                        {
                            await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                .SendMessageAsync("", false, embed.Build());
                            await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                .SendFileAsync(
                                    $@"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{output}",
                                    "");
                        }
                    }
                    else
                    {
                        var sent = 0;
                        for (var i = 0; i < messageBefore.Value.Attachments.Count; i++)
                        {
                            var tempMulty = messageBefore.Value.Attachments.ToList();
                            var checkMulty = $"{tempMulty[i].Url.Substring(tempMulty[i].Url.Length - 8, 8)}";
                            var outputMylty = checkMulty.Substring(checkMulty.IndexOf('.') + 1);

                            if (i == 0 && (outputMylty == "png" || outputMylty == "jpg" || outputMylty == "gif"))
                            {
                                sent = 1;
                                embed.WithImageUrl(
                                    $"attachment://{Path.GetFileName($"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}")}");


                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}",
                                        "",
                                        embed: embed.Build());
                            }
                            else
                            {
                                if (sent != 1)
                                    await _client.GetGuild(guild.ServerId)
                                        .GetTextChannel(guild.LogChannelId)
                                        .SendMessageAsync("", false, embed.Build());
                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $@"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{
                                                messageBefore.Id
                                            }-{i + 1}.{outputMylty}",
                                        "");
                                sent = 1;
                            }
                        }
                    }
                }
                else
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                //  ignored
            }
        }

        public async Task Client_MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            MessageUpdated(messageBefore, messageAfter, arg3);
        }

        public async Task DeleteLogg(Cacheable<IMessage, ulong> messageBefore,
            ISocketMessageChannel arg3)
        {
            try
            {

                if (messageBefore.Value == null)
                    return;


                var currentIGuildChannel = arg3 as IGuildChannel;
                if (currentIGuildChannel == null)
                    return;

                var octoBot = currentIGuildChannel.GetUserAsync(_global.Client.CurrentUser.Id);
                var guild = _serverAccounts.GetServerAccount(currentIGuildChannel);

                if (guild.ServerActivityLog != 1)
                    return;
                if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("View Audit Log Permissions Missing.");
                    return;
                }

                if (messageBefore.Value.Author.IsBot)
                    return;
                if (messageBefore.Value.Channel is ITextChannel)
                {
                    /*
                    var log = await kek.Guild.GetAuditLogsAsync(1);
                    var audit = log.ToList();

                    var name = $"{messageBefore.Value.Author.Mention}";
                    var check = audit[0].Data as MessageDeleteAuditLogData;

                    if (check?.ChannelId == messageBefore.Value.Channel.Id &&
                        audit[0].Action == ActionType.MessageDeleted)
                        name = $"{audit[0].User.Mention}";
*/
                    var embedDel = new EmbedBuilder();

                    embedDel.WithFooter($"MessId: {messageBefore.Id}");
                    embedDel.WithTimestamp(DateTimeOffset.UtcNow);
                    embedDel.WithThumbnailUrl($"{messageBefore.Value.Author.GetAvatarUrl()}");

                    embedDel.WithColor(Color.Red);
                    embedDel.WithTitle("🗑 Deleted Message");
                    embedDel.WithDescription($"Where: <#{messageBefore.Value.Channel.Id}>\n" +
                                             //$"WHO: **{name}** (not always correct)\n" +
                                             $"Mess Author: **{messageBefore.Value.Author}**\n");


                    if (messageBefore.Value.Content.Length > 1000)
                    {
                        var string1 = messageBefore.Value.Content.Substring(0, 1000);

                        embedDel.AddField("Content1", $"{string1}");

                        if (messageBefore.Value.Content.Length <= 2000)
                        {
                            var string2 =
                                messageBefore.Value.Content.Substring(1000, messageBefore.Value.Content.Length - 1000);
                            embedDel.AddField("Continued", $"...{string2}");
                        }
                    }
                    else if (messageBefore.Value.Content.Length != 0)
                    {
                        embedDel.AddField("Content", $"{messageBefore.Value.Content}");
                    }

                    if (messageBefore.Value.Attachments.Any())
                    {
                        var temp = messageBefore.Value.Attachments.FirstOrDefault()?.Url;
                        var check2 = $"{temp?.Substring(temp.Length - 8, 8)}";
                        var output = check2.Substring(check2.IndexOf('.') + 1);
                        //OctoAttachments/{ll?.GuildId}
                        var guildChannel = (IGuildChannel) arg3;


                        if (messageBefore.Value.Attachments.Count == 1)
                        {
                            if (output == "png" || output == "jpg" || output == "gif")
                            {
                                embedDel.WithImageUrl(
                                    $"attachment://{Path.GetFileName($"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{output}")}");

                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{output}",
                                        "",
                                        embed: embedDel.Build());
                            }
                            else
                            {
                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendMessageAsync("", false, embedDel.Build());
                                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                    .SendFileAsync(
                                        $@"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}.{
                                                output
                                            }",
                                        "");
                            }
                        }
                        else
                        {
                            var sent = 0;
                            for (var i = 0; i < messageBefore.Value.Attachments.Count; i++)
                            {
                                var tempMulty = messageBefore.Value.Attachments.ToList();
                                var checkMulty = $"{tempMulty[i].Url.Substring(tempMulty[i].Url.Length - 8, 8)}";
                                var outputMylty = checkMulty.Substring(checkMulty.IndexOf('.') + 1);

                                if (i == 0 && (outputMylty == "png" || outputMylty == "jpg" || outputMylty == "gif"))
                                {
                                    sent = 1;
                                    embedDel.WithImageUrl(
                                        $"attachment://{Path.GetFileName($"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}")}");

                                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}-{i + 1}.{outputMylty}",
                                            "",
                                            embed: embedDel.Build());
                                }
                                else
                                {
                                    if (sent != 1)
                                        await _client.GetGuild(guild.ServerId)
                                            .GetTextChannel(guild.LogChannelId)
                                            .SendMessageAsync("", false, embedDel.Build());
                                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                                        .SendFileAsync(
                                            $@"OctoDataBase/OctoAttachments/{guildChannel.GuildId}/{messageBefore.Id}-{
                                                    i + 1
                                                }.{outputMylty}",
                                            "");
                                    sent = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("", false, embedDel.Build());
                    }
                }
            }
            catch
            {
             //   Console.WriteLine(e);
            }
        }

        public async Task Client_MessageDeleted(Cacheable<IMessage, ulong> messageBefore,
            ISocketMessageChannel arg3)
        {
            DeleteLogg(messageBefore, arg3);
        }

        public async Task RoleUpdated(SocketRole before, SocketRole after)
        {
            try
            {
                if (after == null)
                    return;
                if (before == after)
                    return;

                var octoBot = ((IGuild) before.Guild).GetUserAsync(_global.Client.CurrentUser.Id);
                var guild = _serverAccounts.GetServerAccount(before.Guild);

                if (guild.ServerActivityLog != 1)
                    return;
                if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("View Audit Log Permissions Missing.");
                    return;
                }

                var roleString = "nothing";
                var list1 = before.Permissions.ToList();
                var list2 = after.Permissions.ToList();
                var role = "\n";

                if (list1.Count > list2.Count)
                {
                    roleString = "Removed";
                    var differenceQuery = list1.Except(list2);
                    var socketRoles = differenceQuery as GuildPermission[] ?? differenceQuery.ToArray();
                    foreach (var t in socketRoles)
                        role += $"{t}\n";
                }
                else if (list1.Count < list2.Count)
                {
                    roleString = "Added";
                    var differenceQuery = list2.Except(list1);
                    var socketRoles = differenceQuery as GuildPermission[] ?? differenceQuery.ToArray();
                    foreach (var t in socketRoles)
                        role += $"{t}\n";
                }

                var extra = "";
                if (before.Name != after.Name)
                {
                    extra += "__**Before:**__\n" +
                             $"Name: **{before}**\n";
                    if (before.Color.ToString() != after.Color.ToString()) extra += $"Color: {before.Color}\n";

                    extra += "__**After:**__\n" +
                             $"Name: **{after}**\n";
                    if (before.Color.ToString() != after.Color.ToString()) extra += $"Color: {after.Color}\n";
                }
                else if (before.Color.ToString() != after.Color.ToString())
                {
                    extra += "__**Before:**__\n";
                    extra += $"Color: {before.Color}\n";
                    extra += "__**After:**__\n";
                    extra += $"Color: {after.Color}\n";
                }

                var log = await before.Guild.GetAuditLogsAsync(1).FlattenAsync();
                var audit = log.ToList();
                var check = audit[0].Data as RoleUpdateAuditLogData;
                var name = "error";
                if (check?.After.Name == after.Name) name = audit[0].User.Mention;


                var embed = new EmbedBuilder();
                embed.WithColor(57, 51, 255);
                embed.AddField($"🛠️ Role Updated({roleString})", $"Role: {after.Mention}\n" +
                                                                  $"WHO: {name}\n" +
                                                                  $"ID: {before.Id}\n" +
                                                                  $"Guild: {before.Guild.Name}\n" +
                                                                  $"{extra}" +
                                                                  $"Permission ({roleString}): **{role}**");
                embed.WithTimestamp(DateTimeOffset.UtcNow);
                embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");


                var s = role.Replace("\n", "");
                if (s.Length < 1 && extra.Length < 1)
                    return;

                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                    .SendMessageAsync("", false, embed.Build());
            }
            catch
            {
                //ignored
            }
        }

        public async Task Client_RoleUpdated(SocketRole arg1, SocketRole arg2)
        {
            RoleUpdated(arg1, arg2);
        }

        public async Task ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            try
            {
                if (arg1 is IGuildChannel currentIguildChannel)
                {
                    if (arg2 == null)
                        return;

                    var octoBot = currentIguildChannel.Guild.GetUserAsync(_global.Client.CurrentUser.Id);
                    var guild = _serverAccounts.GetServerAccount(currentIguildChannel);

                    if (guild.ServerActivityLog != 1)
                        return;
                    if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                    {
                        await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                            .SendMessageAsync("View Audit Log Permissions Missing.");
                        return;
                    }

                    var embed = new EmbedBuilder();
                    embed.WithColor(255, 255, 0);


                    if (arg1 is ITextChannel channelBefore && arg2 is ITextChannel channelAfter)
                    {
                        if (channelBefore == channelAfter)
                            return;

                        var textBefore = "";
                        var textAfter = "";

                        if (channelBefore.Name != channelAfter.Name)
                        {
                            textBefore += $"Name: {channelBefore.Name}\n";
                            textAfter += $"Name: {channelAfter.Name}\n";
                        }

                        if (channelBefore.Topic != channelAfter.Topic)
                        {
                            textBefore += $"Topic: {channelBefore.Topic}\n";
                            textAfter += $"Topic: {channelAfter.Topic}\n";
                        }

                        if (channelBefore.GetCategoryAsync()?.Result.Name !=
                            channelAfter.GetCategoryAsync()?.Result.Name)
                        {
                            textBefore += $"Category: {channelBefore.GetCategoryAsync()?.Result.Name}\n";
                            textAfter += $"Category: {channelAfter.GetCategoryAsync()?.Result.Name}\n";

                            textBefore += $"Position: {channelBefore.Position}\n";
                            textAfter += $"Position: {channelAfter.Position}\n";
                        }


                        if (channelBefore.IsNsfw != channelAfter.IsNsfw)
                        {
                            textBefore += $"IsNsfw: {channelBefore.IsNsfw.ToString()}\n";
                            textAfter += $"IsNsfw: {channelAfter.IsNsfw.ToString()}\n";
                        }

                        embed.AddField("💉 Text Channel Updated", $"Channel: {channelBefore.Mention}\n" +
                                                                  $"Category: {channelBefore.GetCategoryAsync()?.Result?.Name}\n" +
                                                                  $"Created: {channelBefore.CreatedAt.UtcDateTime.Date} UTC\n" +
                                                                  $"ID: {channelBefore.Id}\n");
                        embed.AddField("Before:", textBefore);
                        embed.AddField("After", textAfter);

                        embed.WithTimestamp(DateTimeOffset.UtcNow);
                    }


                    if (arg1 is IVoiceChannel voiceChannelBefore && arg2 is IVoiceChannel voiceChannelAfter)
                    {
                        if (voiceChannelBefore == voiceChannelAfter)
                            return;

                        var textBefore = "";
                        var textAfter = "";

                        if (voiceChannelBefore.Name != voiceChannelAfter.Name)
                        {
                            textBefore += $"Name: {voiceChannelBefore.Name}\n";
                            textAfter += $"Name: {voiceChannelAfter.Name}\n";
                        }

                        if (voiceChannelBefore.GetCategoryAsync()?.Result.Name !=
                            voiceChannelAfter.GetCategoryAsync()?.Result.Name)
                        {
                            textBefore += $"Category: {voiceChannelBefore.GetCategoryAsync()?.Result.Name}\n";
                            textAfter += $"Category: {voiceChannelAfter.GetCategoryAsync()?.Result.Name}\n";

                            textBefore += $"Position: {voiceChannelBefore.Position}\n";
                            textAfter += $"Position: {voiceChannelAfter.Position}\n";
                        }

                        if (voiceChannelBefore.UserLimit != voiceChannelAfter.UserLimit)
                        {
                            textBefore += $"UserLimit: {voiceChannelBefore.UserLimit.ToString()}\n";
                            textAfter += $"UserLimit: {voiceChannelAfter.UserLimit.ToString()}\n";
                        }

                        if (voiceChannelBefore.Bitrate != voiceChannelAfter.Bitrate)
                        {
                            textBefore += $"Bitrate: {voiceChannelBefore.Bitrate}\n";
                            textAfter += $"Bitrate: {voiceChannelAfter.Bitrate}\n";
                        }

                        embed.AddField("💉 Voice Channel Updated",
                            $"Channel: {voiceChannelBefore.Name}\n" +
                            $"Category: {voiceChannelBefore.GetCategoryAsync()?.Result.Name}\n" +
                            $"Created: {voiceChannelBefore.CreatedAt.UtcDateTime.Date} UTC\n" +
                            $"ID: {voiceChannelBefore.Id}\n");
                        embed.AddField("Before:", textBefore);
                        embed.AddField("After", textAfter);

                        embed.WithTimestamp(DateTimeOffset.UtcNow);
                    }

                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("", false, embed.Build());
                }
            }
            catch
            {
                //ignored
            }
        }

        public async Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            ChannelUpdated(arg1, arg2);
        }

        public async Task RoleDeleted(SocketRole arg)
        {
            try
            {
                var octoBot = ((IGuild) arg.Guild).GetUserAsync(_global.Client.CurrentUser.Id);
                var guild = _serverAccounts.GetServerAccount(arg.Guild);

                if (guild.ServerActivityLog != 1)
                    return;
                if (!octoBot.Result.GuildPermissions.ViewAuditLog)
                {
                    await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                        .SendMessageAsync("View Audit Log Permissions Missing.");
                    return;
                }


                var log = await arg.Guild.GetAuditLogsAsync(1).FlattenAsync();
                var audit = log.ToList();
                var check = audit[0].Data as RoleDeleteAuditLogData;
                var name = "erorr";

                if (check?.RoleId == arg.Id) name = audit[0].User.Mention;

                var embed = new EmbedBuilder();
                embed.WithColor(240, 51, 255);
                embed.AddField("⚰️ Role Deleted", $"WHO: {name}\n" +
                                                  $"Name: {arg.Name} ({arg.Guild})\n" +
                                                  $"Color: {arg.Color}\n" +
                                                  $"ID: {arg.Id}\n");
                embed.WithTimestamp(DateTimeOffset.UtcNow);
                embed.WithThumbnailUrl($"{audit[0].User.GetAvatarUrl()}");


                await _client.GetGuild(guild.ServerId).GetTextChannel(guild.LogChannelId)
                    .SendMessageAsync("", false, embed.Build());
            }
            catch
            {
                //
            }
        }

        public async Task Client_RoleDeleted(SocketRole arg)
        {
            RoleDeleted(arg);
        }
    }
}