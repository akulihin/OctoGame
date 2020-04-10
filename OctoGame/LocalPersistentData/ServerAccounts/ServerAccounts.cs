using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Timers;

namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public sealed class ServerAccounts :  IServiceSingleton
    {
        public async Task InitializeAsync()
            => await Task.CompletedTask;

        private readonly ConcurrentDictionary<ulong, List<ServerSettings>> _serverAccountsDictionary;
        private readonly ServerDataStorage _serverDataStorage;
        private Timer _loopingTimer;



        public ServerAccounts(ServerDataStorage serversDataStorage)
        {
            _serverDataStorage = serversDataStorage;
            _serverAccountsDictionary = LoadAllAccount();
            CheckTimer();
        }


        private ConcurrentDictionary<ulong, List<ServerSettings>> LoadAllAccount()
        {
           return _serverDataStorage.LoadAllServersSettings();
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

        public ServerSettings GetServerAccount(SocketGuild guild)
        {
            return GetOrCreateServerAccount(guild.Id, guild.Name);
        }

        public ServerSettings GetServerAccount(IGuildChannel guild)
        {
            return GetOrCreateServerAccount(guild.Guild.Id, guild.Guild.Name);
        }

        public List<ServerSettings> GetOrAddServerAccountsForGuild(ulong serverId)
        {
            return _serverAccountsDictionary.GetOrAdd(serverId, x => _serverDataStorage.LoadServerSettings(serverId).ToList());
        }

        public  List<ServerSettings> GetFilteredServerAccounts(Func<ServerSettings, bool> filter)
        {
            var accounts = GetAllAccount();
            return accounts.Where(filter).ToList();
        }

        public ServerSettings GetOrCreateServerAccount(ulong id, string name)
        {
            var accounts = GetOrAddServerAccountsForGuild(id);
            var account = accounts.FirstOrDefault() ?? CreateServerAccount(id, name);
            return account;
        }



        private void SaveAccount(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<ulong, List<ServerSettings>> acount in _serverAccountsDictionary)
            {
                _serverDataStorage.SaveServerSettings(acount.Value, acount.Key);
            }
        }


        public List<ServerSettings> GetAllAccount()
        {
            var accounts = new List<ServerSettings>();
            foreach (var values in _serverAccountsDictionary.Values) accounts.AddRange(values);
            return accounts;
        }

        public ServerSettings CreateServerAccount(ulong id, string name)
        {
            var accounts = GetOrAddServerAccountsForGuild(id);

            var newAccount = new ServerSettings
            {
                ServerName = name,
                ServerId = id,
                Prefix = "*",
                ServerActivityLog = 0,
                Language = "en"
            };

            accounts.Add(newAccount);
            return newAccount;
        }
    }
}