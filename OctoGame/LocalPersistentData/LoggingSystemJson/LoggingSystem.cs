using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.LocalPersistentData.LoggingSystemJson
{
    public class LoggingSystem : ILoggingSystem
    {

        private static readonly ConcurrentDictionary<string, List<LoggingSystemSettings>> AllLogsDictionary =
            new ConcurrentDictionary<string, List<LoggingSystemSettings>>();


        private readonly IUserAccounts _accounts;

        public LoggingSystem( IUserAccounts accounts)
        {

            _accounts = accounts;
        }

        public  List<LoggingSystemSettings> GetOrAddLogsToDictionary(ulong userId1, ulong userId2)
        {
            var keyString = GetKeyString(userId1, userId2);
            return AllLogsDictionary.GetOrAdd(keyString, x => LoggingSystemDataStorage.LoadLogs(keyString).ToList());
        }



       public LoggingSystemSettings GetLogs(ulong userId1, ulong userId2)
        {
            return GetOrCreateLogs(userId1, userId2);
        }



        public LoggingSystemSettings GetOrCreateLogs(ulong userId1, ulong userId2)
        {
            var accounts = GetOrAddLogsToDictionary(userId1, userId2);
            var account = accounts.FirstOrDefault() ?? CreateNewLog(userId1, userId2);
            return account;
        }


        public void SaveCurrentFightLog(ulong userId1, ulong userId2)
        {
            var accounts = GetOrAddLogsToDictionary(userId1, userId2);

            var keyString = GetKeyString(userId1, userId2);

            LoggingSystemDataStorage.SaveLogs(accounts, keyString);
        }


        public void SaveCompletedFight(ulong userId1, ulong userId2)
        {
            var accounts = GetOrAddLogsToDictionary(userId1, userId2);

            var keyString = GetKeyString(userId1, userId2);

            LoggingSystemDataStorage.CompleteSaveLogs(accounts, keyString);
            AllLogsDictionary.Remove(keyString, out accounts);
        }

        public string GetKeyString(ulong userId1, ulong userId2)
        {
            var discordUser1 = _accounts.GetAccount(userId1);
            var discordUser2 = _accounts.GetAccount(userId2);
            return $"{discordUser1.DiscordUserName}-vs-{discordUser2.DiscordUserName}";

        }

        public LoggingSystemSettings CreateNewLog(ulong userId1, ulong userId2)
        {
            var accounts = GetOrAddLogsToDictionary(userId1, userId2);

            var discordUser1 = _accounts.GetAccount(userId1);
            var discordUser2 = _accounts.GetAccount(userId2);
 

            var newAccount = new LoggingSystemSettings
            {
                DiscordUserId1 = userId1,
                DiscordUserId2 = userId2,
                UserName1 = discordUser1.DiscordUserName,
                UserName2 = discordUser2.DiscordUserName,
                UserWon = "Not Finished"
            };

            accounts.Add(newAccount);
            SaveCurrentFightLog(userId1, userId2);
            return newAccount;
        }
    }
}
