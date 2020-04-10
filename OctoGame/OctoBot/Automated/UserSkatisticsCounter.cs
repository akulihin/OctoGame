using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class UserSkatisticsCounter : IServiceSingleton
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public Task InitializeAsync()
            => Task.CompletedTask;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public UserSkatisticsCounter(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
        {
            _serverAccounts = serverAccounts;
            _accounts = accounts;
            _global = global;
     
        }

        public async Task MessageReceived(SocketMessage msg)
        {
            var channel = msg.Channel as IGuildChannel;
            var guild = channel?.Guild;
            if (guild == null) return;
            var account = _accounts.GetAccount(msg.Author);
            account.UserStatistics.AddOrUpdate("all", 1, (key, value) => value + 1);
            account.UserStatistics.AddOrUpdate($"{msg.Channel.Id}", 1, (key, value) => value + 1);
            
        }

        public async Task Clien_MessageReceived(SocketMessage msg)
        {
            MessageReceived(msg);
        }

        public async Task MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel socketChannel)
        {
            var channel = socketChannel as IGuildChannel;
            var guild = channel?.Guild;
            if (guild == null) return;
            var account = _accounts.GetAccount(cacheMessage.Value.Author);
            account.UserStatistics.AddOrUpdate("deleted", 1, (key, value) => value + 1);
            
        }

        public async Task Client_MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
            MessageDeleted(cacheMessage, channel);
        }


        public async Task MessageUpdated(Cacheable<IMessage, ulong> cacheMessageBefore, SocketMessage messageAfter,
            ISocketMessageChannel socketChannel)
        {
            var channel = socketChannel as IGuildChannel;
            var guild = channel?.Guild;
            if (guild == null || messageAfter == null) return;
            var account = _accounts.GetAccount(cacheMessageBefore.Value.Author);
            account.UserStatistics.AddOrUpdate("updated", 1, (key, value) => value + 1);
            
        }

        public async Task Client_MessageUpdated(Cacheable<IMessage, ulong> cacheMessageBefore,
            SocketMessage messageAfter,
            ISocketMessageChannel channel)
        {
            MessageUpdated(cacheMessageBefore, messageAfter, channel);
        }
    }
}