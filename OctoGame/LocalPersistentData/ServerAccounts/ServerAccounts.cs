using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public sealed class ServerAccounts :  IServiceSingleton
    {
        public async Task InitializeAsync()
            => await Task.CompletedTask;
        /*
      это работуящая версия API варианта сторейджа

        private  readonly List<ServerSettings> _accounts;

        public ServerAccounts()
        {
           
            _accounts = ServerDataStorage.LoadServerSettings().Result.ToList();
        }


        public async Task SaveServerAccounts()
        {
            await ServerDataStorage.SaveAllServersData(_accounts);
        }

        public  ServerSettings GetServerAccount(SocketGuild guild)
        {
            return GetOrCreateServerAccount(guild.DiscordId, guild.Name);
        }

        public  ServerSettings GetServerAccount(IGuildChannel guild)
        {
            return GetOrCreateServerAccount(guild.Guild.DiscordId, guild.Guild.Name);
        }

        public  ServerSettings GetOrCreateServerAccount(ulong id, string name)
        {
            var result = from a in _accounts
                where a.ServerId == id
                select a;
            var account = result.FirstOrDefault() ?? CreateServerAccount(id, name);

            return account;
        }


        public  List<ServerSettings> GetAllServerAccounts()
        {
            return _accounts.ToList();
        }

        public  List<ServerSettings> GetFilteredServerAccounts(Func<ServerSettings, bool> filter)
        {
            return _accounts.Where(filter).ToList();
        }


        public  ServerSettings CreateServerAccount(ulong id, string name)
        {
            var newAccount = new ServerSettings
            {
                ServerName = name,
                ServerId = id,
                Prefix = "*",
                ServerActivityLog = 0,
                Language = "en"
            };

            _accounts.Add(newAccount);
        //    ServerDataStorage.AddNewAccount(newAccount);
            return newAccount;
        }
        */
        private  readonly List<ServerSettings> _accounts;
        private readonly ServerDataStorage _serverDataStorage;

        private  readonly string _serverAccountsFile = @"OctoDataBase/ServerAccounts.json";

         public ServerAccounts(ServerDataStorage serverDataStorage)
         {
             _serverDataStorage = serverDataStorage;
             if (_serverDataStorage.SaveExists(_serverAccountsFile))
            {
                _accounts = _serverDataStorage.LoadServerSettings(_serverAccountsFile).ToList();
            }
            else
            {
                _accounts = new List<ServerSettings>();
                SaveServerAccounts();
            }
         }



        public  void SaveServerAccounts()
        {
            _serverDataStorage.SaveServerSettings(_accounts, _serverAccountsFile);
        }

        public  ServerSettings GetServerAccount(SocketGuild guild)
        {
            return GetOrCreateServerAccount(guild.Id, guild.Name);
        }

        public  ServerSettings GetServerAccount(IGuildChannel guild)
        {
            return GetOrCreateServerAccount(guild.Guild.Id, guild.Guild.Name);
        }

        public  ServerSettings GetOrCreateServerAccount(ulong id, string name)
        {
            var result = from a in _accounts
                where a.ServerId == id
                select a;
            var account = result.FirstOrDefault() ?? CreateServerAccount(id, name);

            return account;
        }


        public  List<ServerSettings> GetAllServerAccounts()
        {
            return _accounts.ToList();
        }

        public  List<ServerSettings> GetFilteredServerAccounts(Func<ServerSettings, bool> filter)
        {
            return _accounts.Where(filter).ToList();
        }


        public  ServerSettings CreateServerAccount(ulong id, string name)
        {
            var newAccount = new ServerSettings
            {
                ServerName = name,
                ServerId = id,
                Prefix = "*",
                ServerActivityLog = 0,
                Language = "en"
            };

            _accounts.Add(newAccount);
            SaveServerAccounts();
            return newAccount;
        }
    }
}