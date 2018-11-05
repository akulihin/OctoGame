using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace OctoGame.Accounts.Server
{
   public interface IServerAccounts
   {
       void SaveServerAccounts();
       ServerSettings GetServerAccount(SocketGuild guild);
       ServerSettings GetServerAccount(IGuildChannel guild);
       ServerSettings GetOrCreateServerAccount(ulong id, string name);
       List<ServerSettings> GetAllServerAccounts();
       List<ServerSettings> GetFilteredServerAccounts(Func<ServerSettings, bool> filter);
       ServerSettings CreateServerAccount(ulong id, string name);
   }
}
