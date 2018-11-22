using System.Diagnostics.CodeAnalysis;

namespace OctoGame.LocalPersistentData.LoggingSystemJson
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    public class LoggingSystemSettings
    {
        public string UserName1 { get; set; }
        public string UserName2 { get; set; }

        public ulong DiscordUserId1 { get; set; }
        public ulong DiscordUserId2 { get; set; }

        public string UserWon { get; set; }

    }
}
