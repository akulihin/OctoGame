using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Timers;

namespace OctoGame.LocalPersistentData.UsersAccounts
{
    public sealed class UserAccounts :  IServiceSingleton
    {
        private readonly ConcurrentDictionary<ulong, List<AccountSettings>> _userAccountsDictionary;

        private readonly DiscordShardedClient _client;
        private readonly UsersDataStorage _usersDataStorage;
        private Timer _loopingTimer;

        public UserAccounts(DiscordShardedClient client, UsersDataStorage usersDataStorage)
        {
            _client = client;
            _usersDataStorage = usersDataStorage;
            _userAccountsDictionary = LoadAllAccount();
            CheckTimer();
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        private ConcurrentDictionary<ulong, List<AccountSettings>> LoadAllAccount()
        {
            return _usersDataStorage.LoadAllAccountSettings();
        }

        internal Task CheckTimer()
        {
            _loopingTimer = new Timer
            {
                AutoReset = true,
                Interval = 60000,
                Enabled = true
            };
            _loopingTimer.Elapsed += SaveAccount;
            return Task.CompletedTask;
        }


        public  List<AccountSettings> GetOrAddUserAccountsForGuild(ulong userId)
        {
            return _userAccountsDictionary.GetOrAdd(userId, x => _usersDataStorage.LoadAccountSettings(userId).ToList());
        }

        public  AccountSettings GetAccount(IUser user)
        {
            return GetOrCreateAccount(user);
        }

        public List<AccountSettings> GetFilteredUserAccounts(Func<AccountSettings, bool> filter)
        {
            var accounts = GetAllAccount();
            return accounts.Where(filter).ToList();
        }

        public AccountSettings GetAccount(ulong userId)
        {
            if(userId > 1000)
                return GetOrCreateAccount(_client.GetUser(userId));
            else
                return _userAccountsDictionary.GetOrAdd(userId, x => _usersDataStorage.LoadAccountSettings(userId).ToList()).FirstOrDefault();
        }

        public  AccountSettings GetOrCreateAccount(IUser user)
        {
            var accounts = GetOrAddUserAccountsForGuild(user.Id);

            var account = accounts.FirstOrDefault() ?? CreateUserAccount(user);
            return account;
        }


        public  void SaveAccounts(ulong userId)
        {
        //depricated
        }

        public  void SaveAccounts(IUser user)
        {
         //depricated
        }

        private void SaveAccount(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<ulong, List<AccountSettings>> acount in _userAccountsDictionary)
            {
                _usersDataStorage.SaveAccountSettings(acount.Value, acount.Key);
            }
        }


        public  List<AccountSettings> GetAllAccount()
        {
            var accounts = new List<AccountSettings>();
            foreach (var values in _userAccountsDictionary.Values)
            {
                try
                {
                    accounts.AddRange(values);
                }
                catch (Exception error)
                {
                    Console.WriteLine("ERROR: GetAllAccount - '{0}'", error);
                }
                
            }
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
