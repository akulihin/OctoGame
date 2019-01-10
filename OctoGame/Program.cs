using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using OctoGame.DiscordFramework;
using OctoGame.DiscordFramework.Extensions;
using System.Net.Http;
using OctoGame.DiscordFramework.Language;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.LoggingSystemJson;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;


namespace OctoGame
{
    public class ProgramOctoGame
    {
        private DiscordShardedClient _client;
        private Container _services;

        private readonly int[] _shardIds = {0};

        private void Main()
        {
            new ProgramOctoGame().RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient(_shardIds, new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                MessageCacheSize = 50,
                TotalShards = 1
            });
            
            _services = new Container(x => 
                {
                    x.AddSingleton(_client)
                        .AddSingleton<CancellationTokenSource>()
                        .AddSingleton<CommandService>()
                        .AddSingleton<HttpClient>()                       
                        .AddSingleton<IDataStorage, JsonLocalStorage>()
                        .AddSingleton<ILocalization, JsonLocalization>()
                        .AddSingleton<IUserAccounts, UserAccounts>()
                        .AddSingleton<IServerAccounts, ServerAccounts>()
                        .AddSingleton<ILoggingSystem, LoggingSystem>()
                        .AddSingleton<ISpellAccounts, SpellUserAccounts>()
                        .AddTransient<SecureRandom>()
                        .AutoAddServices()
                        .BuildServiceProvider();
                });
            
            await _services.InitializeServicesAsync();
         
            await _client.SetGameAsync("Boole~");

            await _client.LoginAsync(TokenType.Bot, _services.GetRequiredService<Config>().Token);
            await _client.StartAsync();

            SendMessagesUsingConsole.ConsoleInput(_client);

            try
            {

                await Task.Delay(-1, _services.GetRequiredService<CancellationTokenSource>().Token);
            }
            catch (TaskCanceledException)
            { }
        }
    }
}