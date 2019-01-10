using System.Collections.Generic;

namespace OctoGame.LocalPersistentData.LoggingSystemJson
{
    public interface ILoggingSystem
    {
        List<LoggingSystemSettings> GetOrAddLogsToDictionary(ulong userId1, ulong userId2);
        LoggingSystemSettings GetLogs(ulong userId1, ulong userId2);
        LoggingSystemSettings GetOrCreateLogs(ulong userId1, ulong userId2);
        void SaveCurrentFightLog(ulong userId1, ulong userId2);
        LoggingSystemSettings CreateNewLog(ulong userId1, ulong userId2);
        void SaveCompletedFight(ulong userId1, ulong userId2);

    }
}