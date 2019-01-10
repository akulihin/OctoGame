using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace OctoGame.LocalPersistentData.UsersAccounts
{
    public sealed class UserAccounts : IUserAccounts
    {
        private static readonly ConcurrentDictionary<ulong, List<AccountSettings>> UserAccountsDictionary =
            new ConcurrentDictionary<ulong, List<AccountSettings>>();

        private readonly DiscordShardedClient _client;
        private readonly UsersDataStorage _usersDataStorage;

        public UserAccounts(DiscordShardedClient client, UsersDataStorage usersDataStorage)
        {
            _client = client;
            _usersDataStorage = usersDataStorage;
        }
       
        /*
        static UserAccounts()
        {
            var guildList = ServerAccounts.GetAllServerAccounts();
            foreach (var guild in guildList)
                UserAccountsDictionary.GetOrAdd(guild.ServerId,
                    x => DataStorage.LoadAccountSettings(guild.ServerId).ToList());
        }*/


        public  List<AccountSettings> GetOrAddUserAccountsForGuild(ulong userId)
        {
            return UserAccountsDictionary.GetOrAdd(userId, x => _usersDataStorage.LoadAccountSettings(userId).ToList());
        }

        public  AccountSettings GetAccount(IUser user)
        {
            return GetOrCreateAccount(user);
        }

       public AccountSettings GetAccount(ulong userId)
        {
            if(userId > 1000)
            return GetOrCreateAccount(_client.GetUser(userId));
            else
            return UserAccountsDictionary.GetOrAdd(userId, x => _usersDataStorage.LoadAccountSettings(userId).ToList()).FirstOrDefault();
        }
        /*
        public AccountSettings GetBotAccount(ulong botId)
        {
            return UserAccountsDictionary.GetOrAdd(botId, x => UsersDataStorage.LoadAccountSettings(botId).ToList()).FirstOrDefault();
        }
        */
        public  AccountSettings GetOrCreateAccount(IUser user)
        {
            var accounts = GetOrAddUserAccountsForGuild(user.Id);
            var account = accounts.FirstOrDefault() ?? CreateUserAccount(user);
            return account;
        }


        public  void SaveAccounts(ulong userId)
        {
            var accounts = GetOrAddUserAccountsForGuild(userId);
            _usersDataStorage.SaveAccountSettings(accounts, userId);
        }

        public  void SaveAccounts(IUser user)
        {
            var accounts = GetOrAddUserAccountsForGuild(user.Id);
            _usersDataStorage.SaveAccountSettings(accounts, user.Id);
        }


        public  List<AccountSettings> GetAllAccount()
        {
            var accounts = new List<AccountSettings>();
            foreach (var values in UserAccountsDictionary.Values) accounts.AddRange(values);
            return accounts;
        }

        public  AccountSettings CreateUserAccount(IUser user)
        {
            var accounts = GetOrAddUserAccountsForGuild(user.Id);

            var newAccount = new AccountSettings
            {
                DiscordId = user.Id,
                DiscordUserName = user.Username,
                MyLanguage = "en",
                OctoLvL = 1
            };

            accounts.Add(newAccount);
            SaveAccounts(user);
            return newAccount;
        }
    }
}
