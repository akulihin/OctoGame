using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoGame.OctoBot.Automated;
using OctoGame.OctoGame.ReactionHandling;

namespace OctoGame.DiscordFramework
{
    public sealed class DiscordEventDispatcher : IServiceSingleton
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private readonly DiscordShardedClient _client;
        private readonly CommandHandling _commandHandler;
        private readonly OctoGameReaction _octoGameReaction;
        private readonly LoginFromConsole _log;
        private readonly Global _global;
        private readonly UserSkatisticsCounter _userSkatisticsCounter;
        private readonly ServerActivityLogger _serverActivityLogger;
        private readonly LvLing _lvLing;
        private readonly GiveRoleOnJoin _giveRoleOnJoin;
        private readonly CheckIfCommandGiveRole _checkIfCommandGiveRole;
        private readonly Announcer _announcer;
        private readonly CheckForVoiceChannelStateForVoiceCommand _checkForVoiceChannelStateForVoiceCommand;

        public DiscordEventDispatcher(DiscordShardedClient client, CommandHandling commandHandler, OctoGameReaction octoGameReaction, LoginFromConsole log, Global global, ServerActivityLogger serverActivityLogger, UserSkatisticsCounter userSkatisticsCounter, LvLing lvLing, GiveRoleOnJoin giveRoleOnJoin, CheckIfCommandGiveRole checkIfCommandGiveRole, Announcer announcer, CheckForVoiceChannelStateForVoiceCommand checkForVoiceChannelStateForVoiceCommand)
        {
            _client = client;
            _commandHandler = commandHandler;
            _octoGameReaction = octoGameReaction;
            _log = log;
            _global = global;
            _serverActivityLogger = serverActivityLogger;
            _userSkatisticsCounter = userSkatisticsCounter;
            _lvLing = lvLing;
            _giveRoleOnJoin = giveRoleOnJoin;
            _checkIfCommandGiveRole = checkIfCommandGiveRole;
            _announcer = announcer;
            _checkForVoiceChannelStateForVoiceCommand = checkForVoiceChannelStateForVoiceCommand;
        }

        public Task InitializeAsync()
        {
            _client.ShardDisconnected += _client_ShardDisconnected;
            _client.ChannelCreated += ChannelCreated;
            _client.ChannelDestroyed += ChannelDestroyed;
            _client.ChannelUpdated += ChannelUpdated;
            _client.CurrentUserUpdated += CurrentUserUpdated;
            _client.GuildAvailable += GuildAvailable;
            _client.GuildMembersDownloaded += GuildMembersDownloaded;
            _client.GuildMemberUpdated += GuildMemberUpdated;
            _client.GuildUnavailable += GuildUnavailable;
            _client.GuildUpdated += GuildUpdated;
            _client.JoinedGuild += JoinedGuild;
            _client.LeftGuild += LeftGuild;
            _client.Log += Log;
            _client.LoggedIn += LoggedIn;
            _client.LoggedOut += LoggedOut;
            _client.MessageDeleted += MessageDeleted;
            _client.MessageReceived += MessageReceived;
            _client.MessageUpdated += MessageUpdated;
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
            _client.ReactionsCleared += ReactionsCleared;
            _client.ShardConnected += _client_ShardConnected;    
            _client.RecipientAdded += RecipientAdded;
            _client.RecipientRemoved += RecipientRemoved;
            _client.RoleCreated += RoleCreated;
            _client.RoleDeleted += RoleDeleted;
            _client.RoleUpdated += RoleUpdated;
            _client.UserBanned += UserBanned;
            _client.UserIsTyping += UserIsTyping;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.UserUnbanned += UserUnbanned;
            _client.UserUpdated += UserUpdated;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdated;
            return Task.CompletedTask;
        }



        private async Task _client_ShardDisconnected(Exception arg1, DiscordSocketClient arg2)
        {
            _log.Warning($"Shard {arg2.ShardId} Disconnected");
            _serverActivityLogger.Client_Disconnected(arg1);

        }

        private async Task _client_ShardConnected(DiscordSocketClient arg)
        {
            _serverActivityLogger.Client_Connected();
        }

        private async Task ChannelCreated(SocketChannel channel)
        {
            _serverActivityLogger.Client_ChannelCreated(channel);
        }

        private async Task ChannelDestroyed(SocketChannel channel)
        {
            _serverActivityLogger.Client_ChannelDestroyed(channel);
        }

        private async Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
        {
            _serverActivityLogger.Client_ChannelUpdated(channelBefore, channelAfter);
        }

        private async Task CurrentUserUpdated(SocketSelfUser userBefore, SocketSelfUser userAfter)
        {
        }

        private async Task GuildAvailable(SocketGuild guild)
        {
        }

        private async Task GuildMembersDownloaded(SocketGuild guild)
        {
        }


        private async Task GuildMemberUpdated(SocketGuildUser userBefore, SocketGuildUser userAfter)
        {
            _serverActivityLogger.Client_GuildMemberUpdated(userBefore, userAfter);
        }

        private async Task GuildUnavailable(SocketGuild guild)
        {
        }

        private async Task GuildUpdated(SocketGuild guildBefore, SocketGuild guildAfter)
        {
        }

        private async Task JoinedGuild(SocketGuild guild)
        {
            _serverActivityLogger.Client_JoinedGuild(guild);
        }


        private async Task LeftGuild(SocketGuild guild)
        {
        }

        private async Task Log(LogMessage logMessage)
        {
            _log.Log(logMessage);
        }

        private async Task LoggedIn()
        {
        }

        private async Task LoggedOut()
        {
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
            if (!cacheMessage.HasValue || cacheMessage.Value.Author.IsBot)
            {
                return; //IActivity guess
            }
            _commandHandler._client_MessageDeleted(cacheMessage, channel);
            _serverActivityLogger.Client_MessageDeleted(cacheMessage, channel);
            _userSkatisticsCounter.Client_MessageDeleted(cacheMessage, channel);
        }

        private async Task MessageReceived(SocketMessage message)
        {

            if(message.Author.IsBot)
                return;
            _global.TimeSpendOnLastMessage.AddOrUpdate(message.Author.Id, Stopwatch.StartNew(), (key, oldValue) =>  Stopwatch.StartNew());
            _commandHandler.HandleCommandAsync(message);
            _checkIfCommandGiveRole.Client_MessageReceived(message, _client);
            _serverActivityLogger.Client_MessageReceived(message);
            _serverActivityLogger.Client_MessageRecivedForServerStatistics(message);
            _lvLing.Client_UserSentMess(message);
            _userSkatisticsCounter.Clien_MessageReceived(message);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> cacheMessageBefore, SocketMessage messageAfter,
            ISocketMessageChannel channel)
        {

            if(!cacheMessageBefore.HasValue)
                return;
            if(cacheMessageBefore.Value.Author.IsBot)
                return;


            _global.TimeSpendOnLastMessage.AddOrUpdate(messageAfter.Author.Id, Stopwatch.StartNew(), (key, oldValue) =>  Stopwatch.StartNew());


            _commandHandler._client_MessageUpdated(cacheMessageBefore, messageAfter, channel);

            _serverActivityLogger.Client_MessageUpdated(cacheMessageBefore, messageAfter, channel);
            _userSkatisticsCounter.Client_MessageUpdated(cacheMessageBefore, messageAfter, channel);

        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheMessage, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return;
            _octoGameReaction.ReactionAddedForOctoGameAsync(cacheMessage, channel, reaction);
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cacheMessage, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
            }
        }

        private async Task ReactionsCleared(Cacheable<IUserMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
        }

        private async Task RecipientAdded(SocketGroupUser user)
        {
        }

        private async Task RecipientRemoved(SocketGroupUser user)
        {
        }

        private async Task RoleCreated(SocketRole role)
        {
        }


        private async Task RoleDeleted(SocketRole role)
        {
            _serverActivityLogger.Client_RoleDeleted(role);
        }

        private async Task RoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
            _serverActivityLogger.Client_RoleUpdated(roleBefore, roleAfter);
        }

        private async Task UserBanned(SocketUser user, SocketGuild guild)
        {
        }

        private async Task UserIsTyping(SocketUser user, ISocketMessageChannel channel)
        {
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            _announcer.AnnounceUserJoin(user);
            _giveRoleOnJoin.Client_UserJoined_ForRoleOnJoin(user);
        }

        private async Task UserLeft(SocketGuildUser user)
        {
        }

        private async Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
        }

        private async Task UserUpdated(SocketUser user, SocketUser guild)
        {
        }

        private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState voiceStateBefore,
            SocketVoiceState voiceStateAfter)
        {
            _checkForVoiceChannelStateForVoiceCommand.Client_UserVoiceStateUpdatedForCreateVoiceChannel(user,
                voiceStateBefore, voiceStateAfter);
        }

    }
}