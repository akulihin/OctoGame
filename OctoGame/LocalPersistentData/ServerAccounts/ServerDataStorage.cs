using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public static class ServerDataStorage
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


        public static void SaveServerSettings(IEnumerable<ServerSettings> accounts, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveServerSettings). Will ty in 5 sec.");
            }
        }

        //Get ServerSettings

        public static IEnumerable<ServerSettings> LoadServerSettings(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ServerSettings>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}