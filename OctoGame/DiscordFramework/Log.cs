// Comment below to disable pretty logging
// Pretty logging splits each newline in the message to its own message

#define PRETTY_LOGGING
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace OctoGame.DiscordFramework
{
    public sealed class LoginFromConsole : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private string _runTime = @"DataBase/OctoDataBase/Log.json";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly int _padLength = 16;

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

        private readonly IReadOnlyDictionary<LogSeverity, string> _logAbbreviations =
            new Dictionary<LogSeverity, string>
            {
                {LogSeverity.Critical, "CRIT"},
                {LogSeverity.Error, "ERRO"},
                {LogSeverity.Warning, "WARN"},
                {LogSeverity.Info, "INFO"},
                {LogSeverity.Debug, "DBUG"},
                {LogSeverity.Verbose, "VRBS"}
            };

        internal Task Log(LogMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Message)) return Task.CompletedTask;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Critical(message.Message, message.Source);
                    break;
                case LogSeverity.Error:
                    Error(message.Message, message.Source);
                    break;
                case LogSeverity.Warning:
                    Warning(message.Message, message.Source);
                    break;
                case LogSeverity.Info:
                    Info(message.Message, message.Source);
                    break;
                case LogSeverity.Verbose:
                    Verbose(message.Message, message.Source);
                    break;
                case LogSeverity.Debug:
                    Debug(message.Message, message.Source);
                    break;
            }

            return Task.CompletedTask;
        }
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
                Console.Write($"{_logAbbreviations[severity]}");
                Console.ResetColor();
                var caller = callerFilePath;

                if(caller.Length > 20)
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
                var spaces = _padLength - str.Length;
                var padLeft = spaces / 2 + str.Length;

                return str.PadLeft(padLeft).PadRight(_padLength);
            }
        }
    }
}