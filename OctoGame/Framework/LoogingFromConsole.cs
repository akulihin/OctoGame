using System;
using System.IO;
using System.Threading.Tasks;
using Discord;

namespace  OctoGame.Framework
{
    internal static class LoogingFromConsole
    {
        private static string _runTime = @"OctoDataBase/Log.json";

        internal static Task Log(LogMessage logMessage)
        {
            var color = SeverityToConsoleColor(logMessage.Severity);
            var message = $"[{logMessage.Source}] {logMessage.Message}";
            Log(message, color);
            return Task.CompletedTask;
        }

        internal static void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - {message}");
            Console.ResetColor();

            File.AppendAllText(_runTime, $"\n{DateTime.Now.ToLongTimeString()} - {message}");
        }

        internal static void ClearLog()
        {
            File.AppendAllText(_runTime, "");
        }

        private static ConsoleColor SeverityToConsoleColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Debug:
                    return ConsoleColor.Blue;
                case LogSeverity.Error:
                    return ConsoleColor.Yellow;
                case LogSeverity.Info:
                    return ConsoleColor.Cyan;
                case LogSeverity.Verbose:
                    return ConsoleColor.Green;
                case LogSeverity.Warning:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}