using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OctoGame.Accounts.Users
{
       public static class UsersDataStorage
    {
        //Save all AccountSettings

        public static void SaveAccountSettings(IEnumerable<AccountSettings> accounts, string idString, string json)
        {
            var filePath = $@"OctoDataBase/UserAccounts/account-{idString}.json";
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveAccountSettings). Will ty in 5 sec.");
            }
        }


        public static void SaveAccountSettings(IEnumerable<AccountSettings> accounts, ulong userId)
        {
            var filePath = $@"OctoDataBase/UserAccounts/account-{userId}.json";
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                Console.WriteLine("Failed To ReadFile(SaveAccountSettings). Will ty in 5 sec.");
            }
        }

        //Get AccountSettings

        public static IEnumerable<AccountSettings> LoadAccountSettings(ulong userId)
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
                Console.WriteLine($"LoadAccountSettings TRY_CATCH: {e}");
                var newList = new List<AccountSettings>();
                SaveAccountSettings(newList, $"{userId}-BACK_UP", json);
                return newList;
            }
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}