using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctoGame.Accounts.Server;
using OctoGame.Accounts.Users;
using OctoGame.Framework.Library;

namespace OctoGame.Framework
{
    public class CommandHandeling
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;
        private readonly Global _global;
        private readonly IServiceProvider _services;
        private readonly IUserAccounts _accounts;
        private readonly IServerAccounts _serverAccounts;
        private const string LogFile = @"OctoDataBase/Log.json";

        public CommandHandeling(IServiceProvider services, CommandService commands,
            DiscordShardedClient client, IUserAccounts accounts, IServerAccounts serverAccounts, Global global)
        {
            _commands = commands;
            _services = services;
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
                var context = new SocketCommandContextCustom(_client, message, "edit", account.MyLanguage);
                var argPos = 0;


                if (message.Channel is SocketDMChannel)
                {
                    var resultTask = await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services);
                  
                    if (!resultTask.IsSuccess  && !resultTask.ErrorReason.Contains("Unknown command")) ReplyAsync(context, $"Booole! {resultTask.ErrorReason}");
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
                    var resultTaskk = await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services);


                    if (!resultTaskk.IsSuccess  && !resultTaskk.ErrorReason.Contains("Unknown command")) ReplyAsync(context, $"Booole! {resultTaskk.ErrorReason}");
                }

                return;
            }


            await HandleCommandAsync(messageAfter); 
        }

        public async Task ReplyAsync(SocketCommandContextCustom context, EmbedBuilder embed)
        {
            if (context.MessageContentForEdit == null)
            {
                var message = await context.Channel.SendMessageAsync("", false, embed.Build());
                var kek = new Global.CommandRam(context.User, context.Message, message);
                _global.CommandList.Add(kek);
            }
            else if (context.MessageContentForEdit == "edit")
            {
                foreach (var t in _global.CommandList)
                    if (t.UserSocketMsg.Id == context.Message.Id)
                    {
                        if (context.Message.Content.Contains("top"))
                            await t.BotSocketMsg.ModifyAsync(message =>
                            {
                                message.Content = "";
                                message.Embed = null;
                                //  message.Embed = embed.Build();
                            });

                        await t.BotSocketMsg.ModifyAsync(message =>
                        {
                            message.Content = "";
                            message.Embed = null;
                            message.Embed = embed.Build();
                        });
                    }
            }
        }


        public async Task ReplyAsync(SocketCommandContextCustom context, [Remainder] string regularMess = null)
        {
            if (context.MessageContentForEdit == null)
            {
                var message = await context.Channel.SendMessageAsync($"{regularMess}");
                var kek = new Global.CommandRam(context.User, context.Message, message);
                
                _global.CommandList.Add(kek);
            }
            else if (context.MessageContentForEdit == "edit")
            {
                foreach (var t in _global.CommandList)
                    if (t.UserSocketMsg.Id == context.Message.Id)
                        await t.BotSocketMsg.ModifyAsync(message =>
                        {
                            message.Content = "";
                            message.Embed = null;
                            if (regularMess != null) message.Content = regularMess.ToString();
                        });
            }
        }


        public async Task HandleCommandAsync(SocketMessage msg)

        {
            var message = msg as SocketUserMessage;
            if (message == null) return;
            var account = _accounts.GetAccount(msg.Author);
            var context = new SocketCommandContextCustom(_client, message, null, account.MyLanguage);
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
                            ReplyAsync(context, $"Booole! {task.Result.ErrorReason}");
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
                        ReplyAsync(context, $"Booole! {task.Result.ErrorReason}");
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