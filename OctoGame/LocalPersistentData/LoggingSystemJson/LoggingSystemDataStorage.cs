using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OctoGame.LocalPersistentData.LoggingSystemJson
{
    public static class LoggingSystemDataStorage
    {
        //Save all AccountSettings

        public static void SaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString, string json)
        {
            var filePath = $@"OctoDataBase/Logging/{keyString}.json";
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveCurrentFightLog). Will ty in 5 sec.");
            }
        }


        public static void SaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString)
        {
            var filePath = $@"OctoDataBase/Logging/{keyString}.json";
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveCurrentFightLog). Will ty in 5 sec.");
            }
        }

        public static void CompleteSaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString)
        {
            var index = 1;
            var filePath = $@"OctoDataBase/Logging/{keyString}-{index}.json";

            while (File.Exists(filePath))
            {
                filePath = $@"OctoDataBase/Logging/{keyString}-{index++}.json";
            }

            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveCurrentFightLog). Will ty in 5 sec.");
            }
        }

        //Get AccountSettings

        public static IEnumerable<LoggingSystemSettings> LoadLogs(string keyString)
        {
            var filePath = $@"OctoDataBase/Logging/{keyString}.json";
            if (!File.Exists(filePath))
            {
                var newList = new List<LoggingSystemSettings>();
                SaveLogs(newList, keyString);
                return newList;
            }

            var json = File.ReadAllText(filePath);

            try
            {
                return JsonConvert.DeserializeObject<List<LoggingSystemSettings>>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"LoadLogs TRY_CATCH: {e}");
                var newList = new List<LoggingSystemSettings>();
                SaveLogs(newList, $"{keyString}-BACK_UP-{DateTime.UtcNow}", json);
                return newList;
            }
        }

    
    }
}