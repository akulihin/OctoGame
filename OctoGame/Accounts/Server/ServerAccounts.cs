using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace OctoGame.Accounts.Server
{
    public  class ServerAccounts : IServerAccounts
    {
        private  readonly List<ServerSettings> _accounts;

        private  readonly string _serverAccountsFile = @"OctoDataBase/ServerAccounts.json";

         public ServerAccounts()
        {
            if (ServerDataStorage.SaveExists(_serverAccountsFile))
            {
                _accounts = ServerDataStorage.LoadServerSettings(_serverAccountsFile).ToList();
            }
            else
            {
                _accounts = new List<ServerSettings>();
                SaveServerAccounts();
            }
        }



        public  void SaveServerAccounts()
        {
            ServerDataStorage.SaveServerSettings(_accounts, _serverAccountsFile);
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