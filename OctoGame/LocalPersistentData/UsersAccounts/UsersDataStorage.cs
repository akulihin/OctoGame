using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;

namespace OctoGame.LocalPersistentData.UsersAccounts
{
       public sealed class UsersDataStorage : IService
    {
        //Save all AccountSettings

        private readonly LoginFromConsole _log;

        public UsersDataStorage(LoginFromConsole log)
        {
            _log = log;
        }

        public async Task InitializeAsync()
            => await Task.CompletedTask;


        public void SaveAccountSettings(IEnumerable<AccountSettings> accounts, string idString, string json)
        {
            var filePath = $@"OctoDataBase/UserAccounts/account-{idString}.json";
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch(Exception e)
            {
                _log.Critical($"Save USER AccountSettings (3 params): {e.Message}");
              
            }
        }


        public void SaveAccountSettings(IEnumerable<AccountSettings> accounts, ulong userId)
        {
            var filePath = $@"OctoDataBase/UserAccounts/account-{userId}.json";
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                _log.Critical($"Save USER AccountSettings (2 params): {e.Message}");

            }
        }

        //Get AccountSettings

        public  IEnumerable<AccountSettings> LoadAccountSettings(ulong userId)
        {
            var filePath = $@"OctoDataBase/UserAccounts/account-{userId}.json";
            if (!File.Exists(filePath))
            {
                var newList = new List<AccountSettings>();
                SaveAccountSettings(newList, userId);
                return newList;
            }

            var json = File.ReadAllText(filePath);

            try
            {
                return JsonConvert.DeserializeObject<List<AccountSettings>>(json);
            }
            catch (Exception e)
            {
                _log.Critical($"LoadAccountSettings, BACK UP CREATED: {e}");
            
                var newList = new List<AccountSettings>();
                SaveAccountSettings(newList, $"{userId}-BACK_UP", json);
                return newList;
            }
        }

    }
}