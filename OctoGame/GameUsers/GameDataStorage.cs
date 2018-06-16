using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OctoBot.Games.OctoGame.GameUsers
{
    public static class GameDataStorage
    {

        //Save all AccountSettings

        public static void SaveAccountSettings(IEnumerable<GameAccountSettings> accounts, string filePath)
        {
            try{
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            } catch {
                Console.WriteLine("Failed To ReadFile(SaveAccountSettings). Will ty in 5 sec.");
            }
        }

        //Get AccountSettings

        public static IEnumerable<GameAccountSettings> LoadAccountSettings(string filePath)
        {

            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<GameAccountSettings>>(json);

        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}
