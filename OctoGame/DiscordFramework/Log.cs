// Comment below to disable pretty logging
// Pretty logging splits each newline in the message to its own message

#define PRETTY_LOGGING
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Discord;

namespace OctoGame.DiscordFramework
{
    public class Log
    {
        private readonly string _runTime = @"OctoDataBase/Log.json";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private const int PadLength = 16;

        private readonly IReadOnlyDictionary<LogSeverity, ConsoleColor> _logColors =
            new Dictionary<LogSeverity, ConsoleColor>
            {
                {LogSeverity.Critical, ConsoleColor.Red},
                {LogSeverity.Error, ConsoleColor.Yellow},
                {LogSeverity.Warning, ConsoleColor.Magenta},
                {LogSeverity.Info, ConsoleColor.Green},
                {LogSeverity.Debug, ConsoleColor.White},
                {LogSeverity.Verbose, ConsoleColor.DarkGray}
            };

        private readonly IReadOnlyDictionary<LogSeverity, string> LogAbbreviations =
            new Dictionary<LogSeverity, string>
            {
                {LogSeverity.Critical, "CRIT"},
                {LogSeverity.Error, "ERRO"},
                {LogSeverity.Warning, "WARN"},
                {LogSeverity.Info, "INFO"},
                {LogSeverity.Debug, "DBUG"},
                {LogSeverity.Verbose, "VRBS"}
            };

        public void Critical(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Critical, value.ToString(), callerFilePath, color);
        }

        public void Critical(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Critical, message, callerFilePath, color);
        }

        public void Error(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Error, value.ToString(), callerFilePath, color);
        }

        public void Error(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Error, message, callerFilePath, color);
        }

        public void Warning(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Warning, value.ToString(), callerFilePath, color);
        }

        public void Warning(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Warning, message, callerFilePath, color);
        }

        public void Info(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Info, value.ToString(), callerFilePath, color);
        }

        public void Info(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Info, message, callerFilePath, color);
        }

        public void Debug(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
#if DEBUG
            LogMessage(LogSeverity.Debug, value.ToString(), callerFilePath, color);
#endif
        }

        public void Debug(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
#if DEBUG
            LogMessage(LogSeverity.Debug, message, callerFilePath, color);
#endif
        }

        public void Verbose(object value, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Verbose, value.ToString(), callerFilePath, color);
        }

        public void Verbose(string message, [CallerFilePath] string callerFilePath = "",
            ConsoleColor color = ConsoleColor.Gray)
        {
            LogMessage(LogSeverity.Verbose, message, callerFilePath, color);
        }

#if PRETTY_LOGGING
        private void LogMessage(LogSeverity severity, string messages, string callerFilePath, ConsoleColor color)
        {
            if (string.IsNullOrEmpty(messages)) return;
            _semaphore.Wait();
            foreach (var message in messages.Split("\n", StringSplitOptions.RemoveEmptyEntries))
            {
#else
        private   void LogMessage(LogSeverity severity, string message, string callerFilePath)
        {
            if (string.IsNullOrEmpty(message)) return;
            Semaphore.Wait();
            
#endif
                Console.Write("[");
                Console.ForegroundColor = _logColors[severity];
                Console.Write($"{LogAbbreviations[severity]}");
                Console.ResetColor();
                var caller = callerFilePath;
                try
                {
                    caller = Path.GetFileNameWithoutExtension(new Uri(callerFilePath).AbsolutePath);
                }
                catch
                {
                    // wasn't a file path, just use the caller name directly
                }

                Console.Write($"|{PadCenter(caller)}] ");
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                File.AppendAllText(_runTime, $"\n{DateTime.Now.ToLongTimeString()} - {message}");
                Console.ResetColor();
#if PRETTY_LOGGING
            }
#endif
            _semaphore.Release();

            string PadCenter(string str)
            {
                var spaces = PadLength - str.Length;
                var padLeft = spaces / 2 + str.Length;
                return str.PadLeft(padLeft).PadRight(PadLength);
            }
        }
    }
}