using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;

namespace OctoGame.LocalPersistentData.LoggingSystemJson
{
    public sealed class LoggingSystemDataStorage : IServiceSingleton
    {
        //Save all AccountSettings

        private readonly LoginFromConsole _log;

        public LoggingSystemDataStorage(LoginFromConsole log)
        {
            _log = log;
        }

        public void SaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString, string json)
        {
            var filePath = $@"OctoDataBase/Logging/{keyString}.json";
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch(Exception e)
            {
                _log.Critical($"Failed To WRITE (SaveLogs) (3 params) : {e.Message}");
              
            }
        }


        public void SaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString)
        {
            var filePath = $@"OctoDataBase/Logging/{keyString}.json";
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                _log.Critical($"Failed To WRITE (SaveLogs) (2 params) : {e.Message}");

            }
        }

        public void CompleteSaveLogs(IEnumerable<LoggingSystemSettings> accounts, string keyString)
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
            catch (Exception e)
            {
                _log.Critical($"Failed To WRRITE (CompleteSaveLogs) (3 params) : {e.Message}");

            }
        }

        //Get AccountSettings

        public IEnumerable<LoggingSystemSettings> LoadLogs(string keyString)
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
                _log.Critical($"Failed to READ (LoadLogs), Back up created: {e.Message}");
                
                var newList = new List<LoggingSystemSettings>();
                SaveLogs(newList, $"{keyString}-BACK_UP-{DateTime.UtcNow}", json);
                return newList;
            }
        }


        public async Task InitializeAsync()
            => await Task.CompletedTask;
    }
}