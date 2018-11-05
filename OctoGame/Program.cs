using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OctoGame.Accounts.GameSpells;
using OctoGame.Accounts.Server;
using OctoGame.Accounts.Users;
using OctoGame.Framework;
using OctoGame.Framework.Language;
using OctoGame.Helpers;
using OctoGame.OctoGame.ReactionHandeling;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame
{
    public class ProgramOctoGame
    {
        private DiscordShardedClient _client;
        private IServiceProvider _services;
        
        private readonly int[] _shardIds = { 0,1};

        private static void Main()
        {
            new ProgramOctoGame().RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token)) return;
            _client = new DiscordShardedClient(_shardIds,new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                MessageCacheSize = 50,
                TotalShards = 2
            });

            _services = ConfigureServices();
            _services.GetRequiredService<DiscordEventHandler>().InitDiscordEvents();
            await _services.GetRequiredService<CommandHandeling>().InitializeAsync();
            
            var botToken = Config.Bot.Token;
            await _client.SetGameAsync("Boole! | *help");

            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
           

            SendMessagesUsingConsole.ConsoleInput(_client);
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {            
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandeling>()
                .AddSingleton<DiscordEventHandler>()
                .AddSingleton<OctoPicPull>()
                .AddSingleton<OctoNamePull>()
                .AddSingleton<Global>()

                .AddTransient<OctoGameUpdateMess>()
                .AddTransient<GameSpellHandeling>()
                .AddTransient<OctoGameReaction>()
                .AddTransient<CustomCalculator>()
                .AddTransient<HelperFunctions>()
                .AddTransient<SecureRandom>()
                .AddTransient<AwaitForUserMessage>()

                .AddSingleton<IDataStorage, JsonLocalStorage>()
                .AddSingleton<ILocalization, JsonLocalization>()
                .AddSingleton<IUserAccounts, UserAccounts>()
                .AddSingleton<IServerAccounts, ServerAccounts>()
                .AddSingleton<ISpellAccounts, SpellUserAccounts>()
                .BuildServiceProvider();
        }
    }
}