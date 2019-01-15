using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;

namespace OctoGame
{
    public sealed class Config : IServiceSingleton
    {

        public Config(LoginFromConsole log)
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(@"DataBase/OctoDataBase/config.json"), this);
            }
            catch (Exception ex)
            {
                log.Critical(ex);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }

        public Task InitializeAsync() => Task.CompletedTask;

        [JsonProperty("Token")] public string Token { get; private set; }
    }
}