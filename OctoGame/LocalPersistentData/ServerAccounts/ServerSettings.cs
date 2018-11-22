namespace OctoGame.LocalPersistentData.ServerAccounts
{
    public class ServerSettings
    {
        public string ServerName { get; set; }
        public ulong ServerId { get; set; }
        public string Prefix { get; set; }
        public string Language { get; set; }
        public int ServerActivityLog { get; set; }

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
    }
}