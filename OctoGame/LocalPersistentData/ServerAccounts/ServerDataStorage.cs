using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;

namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public sealed class ServerDataStorage : IServiceSingleton
    {

        /*
        это работуящая версия API варианта сторейджа


        private static readonly HttpClient Client = new HttpClient();

        //Save all ServerSettings
        public static async Task SaveAllServersData(List<ServerSettings> accounts)
        {
            var json = JsonConvert.SerializeObject(accounts[0], Formatting.Indented);

            var httpContent = new StringContent(json, Encoding.UTF8);
            var httpResponse = await Client.PostAsync("http://localhost:3000/post", httpContent);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            var i = 0;

        }

        //Get ServerSettings
        
        public static async Task<IEnumerable<ServerSettings>> LoadServerSettings()
        {
            HttpResponseMessage response = await Client.GetAsync("http://localhost:3000/pool");
            return JsonConvert.DeserializeObject<List<ServerSettings>>(response.Content.ReadAsStringAsync().Result);
        }
        */



        private readonly LoginFromConsole _log;

        public ServerDataStorage(LoginFromConsole log)
        {
            _log = log;
        }

        public void SaveServerSettings(IEnumerable<ServerSettings> accounts, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch(Exception e)
            {
                _log.Critical($"Failed To ReadFile(Save SERVER Settings): {e.Message}");
              
            }
        }

        //Get ServerSettings

        public IEnumerable<ServerSettings> LoadServerSettings(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ServerSettings>>(json);
        }

        public bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public async Task InitializeAsync()
            => await Task.CompletedTask;

    }
}