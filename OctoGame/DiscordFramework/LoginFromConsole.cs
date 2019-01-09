using System.Threading.Tasks;
using Discord;

namespace  OctoGame.DiscordFramework
{
    public class LoginFromConsole
    {
        private readonly Log _log;

        public LoginFromConsole(Log log)
        {
            _log = log;
        }

        internal Task Log(LogMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Message)) return Task.CompletedTask;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    _log.Critical(message.Message, message.Source);
                    break;
                case LogSeverity.Error:
                    _log.Error(message.Message, message.Source);
                    break;
                case LogSeverity.Warning:
                    _log.Warning(message.Message, message.Source);
                    break;
                case LogSeverity.Info:
                    _log.Info(message.Message, message.Source);
                    break;
                case LogSeverity.Verbose:
                    _log.Verbose(message.Message, message.Source);
                    break;
                case LogSeverity.Debug:
                    _log.Debug(message.Message, message.Source);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}