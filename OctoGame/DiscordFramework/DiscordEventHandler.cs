using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoGame.OctoGame.ReactionHandling;

namespace OctoGame.DiscordFramework
{
    public class DiscordEventHandler
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private readonly DiscordShardedClient _client;
        private readonly CommandHandling _commandHandler;
        private readonly OctoGameReaction _octoGameReaction;
 

        public DiscordEventHandler(DiscordShardedClient client, CommandHandling commandHandler, OctoGameReaction octoGameReaction)
        {
            _client = client;
            _commandHandler = commandHandler;
            _octoGameReaction = octoGameReaction;
        }

        public void InitDiscordEvents()
        {
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
        }

        private async Task _client_ShardConnected(DiscordSocketClient arg)
        {
          
        }

        private async Task ChannelCreated(SocketChannel channel)
        {
        }

        private async Task ChannelDestroyed(SocketChannel channel)
        {
        }

        private async Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
        {
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
        }

        private async Task GuildUnavailable(SocketGuild guild)
        {
        }

        private async Task GuildUpdated(SocketGuild guildBefore, SocketGuild guildAfter)
        {
        }

        private async Task JoinedGuild(SocketGuild guild)
        {
        }


        private async Task LeftGuild(SocketGuild guild)
        {
        }

        private async Task Log(LogMessage logMessage)
        {
            LoogingFromConsole.Log(logMessage);
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
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if(message.Author.IsBot)
                return;
            _commandHandler.HandleCommandAsync(message);
           
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> cacheMessageBefore, SocketMessage messageAfter,
            ISocketMessageChannel channel)
        {

            if(!cacheMessageBefore.HasValue)
                return;
            if(cacheMessageBefore.Value.Author.IsBot)
                return;
            _commandHandler._client_MessageUpdated(cacheMessageBefore, messageAfter, channel);       
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
        }

        private async Task RoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
        }

        private async Task UserBanned(SocketUser user, SocketGuild guild)
        {
        }

        private async Task UserIsTyping(SocketUser user, ISocketMessageChannel channel)
        {
        }

        private async Task UserJoined(SocketGuildUser user)
        {
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
        }
    }
}