using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lamar.IoC;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;


namespace OctoGame.DiscordFramework
{
    public class CommandHandling : ModuleBaseCustom
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;
        private readonly Global _global;
        private readonly Scope _services;
        private readonly IUserAccounts _accounts;
        private readonly IServerAccounts _serverAccounts;

        private const string LogFile = @"OctoDataBase/Log.json";

        public CommandHandling(CommandService commands,
            DiscordShardedClient client, IUserAccounts accounts, IServerAccounts serverAccounts, Global global, Scope scope)
        {
            _commands = commands;
            _services = scope;
            _client = client;
            _accounts = accounts;
            _serverAccounts = serverAccounts;
            _global = global;

        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                _services);
        }

        public async Task _clinet_MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
            foreach (var t in _global.CommandList)
            {
                if (cacheMessage.Value.Id == t.UserSocketMsg.Id)
                {
                    t.BotSocketMsg.DeleteAsync();
                    _global.CommandList.Remove(t);
                }
            }
        }

        public async Task _client_MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            if (messageAfter.Author.IsBot)
                return;
            var after = messageAfter as IUserMessage;
            if (messageAfter.Content == null) return;

            if (messageAfter.Author is SocketGuildUser userCheck && userCheck.IsMuted)
                return;


            var before = (messageBefore.HasValue ? messageBefore.Value : null) as IUserMessage;
            if (before == null)
                return;
            if (arg3 == null)
                return;
            if (before.Content == after?.Content)
                return;


            var list = _global.CommandList;
            foreach (var t in list)
            {
                if (t.UserSocketMsg.Id != messageAfter.Id) continue;

                if (!(messageAfter is SocketUserMessage message)) continue;

                if (t.BotSocketMsg == null)
                    return;
                var account = _accounts.GetAccount(messageAfter.Author);
                var context = new SocketCommandContextCustom(_client, message, _global, "edit", account.MyLanguage);
                var argPos = 0;


                if (message.Channel is SocketDMChannel)
                {
                    var resultTask = await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services);
                  
                    if (!resultTask.IsSuccess  && !resultTask.ErrorReason.Contains("Unknown command")) SendMessAsync( $"Booole! {resultTask.ErrorReason}", context);
                    return;
                }

                var guild = _serverAccounts.GetServerAccount(context.Guild);
                


                if (message.HasStringPrefix(guild.Prefix, ref argPos) || message.HasStringPrefix(guild.Prefix + " ",
                                                                          ref argPos)
                                                                      || message.HasMentionPrefix(_client.CurrentUser,
                                                                          ref argPos)
                                                                      || message.HasStringPrefix(account.MyPrefix + " ",
                                                                          ref argPos)
                                                                      || message.HasStringPrefix(account.MyPrefix,
                                                                          ref argPos))
                {
                    var result = await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services);


                    if (!result.IsSuccess  && !result.ErrorReason.Contains("Unknown command")) SendMessAsync( $"Booole! {result.ErrorReason}", context);
                }

                return;
            }


            await HandleCommandAsync(messageAfter); 
        }


        public async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;
            var account = _accounts.GetAccount(msg.Author);
            var context = new SocketCommandContextCustom(_client, message, _global, null, account.MyLanguage);
            var argPos = 0;

            if (message.Author is SocketGuildUser userCheck && userCheck.IsMuted)
                return;

            if(msg.Author.IsBot)
                return;        

            switch (message.Channel)
            {
                case SocketDMChannel _ when context.User.IsBot:
                    return;
                case SocketDMChannel _:
                    var resultTask = _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services);
                    resultTask.ContinueWith(task =>
                    {
                        if (!task.Result.IsSuccess)
                        {
                            Console.ForegroundColor = LogColor("red");
                            Console.WriteLine(
                                $"{DateTime.Now.ToLongTimeString()} - DM: ERROR '{context.Channel}' {context.User}: {message} || {task.Result.ErrorReason}");
                            Console.ResetColor();

                            File.AppendAllText(LogFile,
                                $"{DateTime.Now.ToLongTimeString()} - DM: ERROR '{context.Channel}' {context.User}: {message} || {task.Result.ErrorReason} \n");

                            if(!task.Result.ErrorReason.Contains("Unknown command"))
                                SendMessAsync( $"Booole! {task.Result.ErrorReason}", context);
                        }
                        else
                        {
                            Console.ForegroundColor = LogColor("white");
                            Console.WriteLine(
                                $"{DateTime.Now.ToLongTimeString()} - DM: '{context.Channel}' {context.User}: {message}");
                            Console.ResetColor();

                            File.AppendAllText(LogFile,
                                $"{DateTime.Now.ToLongTimeString()} - DM: '{context.Channel}' {context.User}: {message} \n");
                        }
                    });

                    return;
            }

            var guild = _serverAccounts.GetServerAccount(context.Guild);

            
            if (message.HasStringPrefix(guild.Prefix, ref argPos) || message.HasStringPrefix(guild.Prefix + " ",
                                                                      ref argPos)
                                                                  || message.HasMentionPrefix(_client.CurrentUser,
                                                                      ref argPos)
                                                                  || message.HasStringPrefix(account.MyPrefix + " ",
                                                                      ref argPos)
                                                                  || message.HasStringPrefix(account.MyPrefix,
                                                                      ref argPos))
            {
                var resultTask = _commands.ExecuteAsync(
                    context,
                    argPos,
                    _services);
                resultTask.ContinueWith(task =>
                {
                    if (!task.Result.IsSuccess)
                    {
                        Console.ForegroundColor = LogColor("red");
                        Console.WriteLine(
                            $"{DateTime.Now.ToLongTimeString()} - ERROR '{context.Channel}' {context.User}: {message} || {task.Result.ErrorReason}");
                        Console.ResetColor();

                        File.AppendAllText(LogFile,
                            $"{DateTime.Now.ToLongTimeString()} - ERROR '{context.Channel}' {context.User}: {message} || {task.Result.ErrorReason} \n");

                        if(!task.Result.ErrorReason.Contains("Unknown command"))
                            SendMessAsync($"Booole! {task.Result.ErrorReason}", context);
                    }
                    else
                    {
                        Console.ForegroundColor = LogColor("white");
                        Console.WriteLine(
                            $"{DateTime.Now.ToLongTimeString()} - '{context.Channel}' {context.User}: {message}");
                        Console.ResetColor();

                        File.AppendAllText(LogFile,
                            $"{DateTime.Now.ToLongTimeString()} - '{context.Channel}' {context.User}: {message} \n");
                    }
                });
            }
        }


        private static ConsoleColor LogColor(string color)
        {
            switch (color)
            {
                case "red": //Critical or Error
                    return ConsoleColor.Red;
                case "green": //Debug
                    return ConsoleColor.Green;
                case "cyan": //Info
                    return ConsoleColor.Cyan;
                case "white": //Regular
                    return ConsoleColor.White;
                case "yellow": // Warning
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}