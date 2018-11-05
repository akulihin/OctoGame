using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OctoGame.Accounts.Server
{
    public static class ServerDataStorage
    {
        //Save all ServerSettings

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