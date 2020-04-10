using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public class ServerSettings
    {
        public string ServerName { get; set; }
        public ulong ServerId { get; set; }
        public string Prefix { get; set; }
        public string Language { get; set; }
        public int ServerActivityLog { get; set; }

        public ulong LogChannelId { get; set; }
        public string RoleOnJoin { get; set; }
        public ulong MessagesReceivedAll { get; set; }
        public int LoggingMessEditIgnoreChar { get; set; }
        public ulong BirthdayRoleId { get; set; }

        public ConcurrentDictionary<string, ulong> MessagesReceivedStatisctic { get; set; } = new ConcurrentDictionary<string, ulong>();
        public ConcurrentDictionary<string, string> Roles { get; set; } = new ConcurrentDictionary<string, string>();
        public List<BirthdayRoleActive> BirthdayRoleList { get; internal set; } = new List<BirthdayRoleActive>();
        public ServerSettings( ulong ServerId, string ServerName, string Prefix, string Language, int ServerActivityLog)
        {
            this.ServerName = ServerName;
            this.ServerId = ServerId;
            this.Prefix = Prefix;
            this.Language = Language;
            this.ServerActivityLog = ServerActivityLog;
        }

        public ServerSettings()
        {
            
        }
        public struct BirthdayRoleActive
        {
            public DateTime DateToRemoveRole;
            public ulong UserId;
            public string NickName;

            public BirthdayRoleActive(DateTime dateToRemoveRole, ulong userId, string nickName)
            {
                DateToRemoveRole = dateToRemoveRole;
                UserId = userId;
                NickName = nickName;
            }
        }
    }
}