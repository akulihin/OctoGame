using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lamar.IoC;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.DiscordFramework
{
    public sealed class CommandHandling : ModuleBaseCustom, IServiceSingleton
    {

        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;
        private readonly CommandsInMemory _commandsInMemory;
        private readonly Scope _services;
        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;
        private readonly LoginFromConsole _log;
        private readonly Global _global;


        public CommandHandling(CommandService commands,
            DiscordShardedClient client, UserAccounts accounts, ServerAccounts serverAccounts, CommandsInMemory commandsInMemory,
            Scope scope, LoginFromConsole log, Global global)
        {
            _commands = commands;
            _services = scope;
            _log = log;
            _global = global;
            _client = client;
            _accounts = accounts;
            _serverAccounts = serverAccounts;
            _commandsInMemory = commandsInMemory;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                _services);
        }

        public async Task _client_MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
            foreach (var t in _commandsInMemory.CommandList)
                if (cacheMessage.Value.Id == t.MessageUserId)
                {
                    _global.TotalCommandsDeleted++;
                  await t.BotSocketMsg.DeleteAsync();
                    _commandsInMemory.CommandList.Remove(t);
                }

            await Task.CompletedTask;
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


            var before = messageBefore.HasValue ? messageBefore.Value : null;
            if (before == null)
                return;
            if (arg3 == null)
                return;
            if (before.Content == after?.Content)
                return;


            var list = _commandsInMemory.CommandList;
            foreach (var t in list)
            {
                if (t.MessageUserId != messageAfter.Id) continue;

                if (!(messageAfter is SocketUserMessage message)) continue;

                if (t.BotSocketMsg == null)
                    return;
                _global.TotalCommandsChanged++;
                var account = _accounts.GetAccount(messageAfter.Author);
                var context = new SocketCommandContextCustom(_client, message,  _commandsInMemory, "edit", account.MyLanguage);
                var argPos = 0;


                if (message.Channel is SocketDMChannel)
                {
                   
                    var resultTask = Task.FromResult(await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services));

                  

                    await resultTask.ContinueWith(async task =>
                        await CommandResults(task, context));

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

            
                    var resultTask = Task.FromResult(await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services));
                    

                    await resultTask.ContinueWith(async task =>
                        await CommandResults(task, context));
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
            var context = new SocketCommandContextCustom(_client, message, _commandsInMemory, null, account.MyLanguage);
            var argPos = 0;

            if (message.Author is SocketGuildUser userCheck && userCheck.IsMuted)
                return;

            if (msg.Author.IsBot)
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

               


                  await  resultTask.ContinueWith(async task =>
                        await CommandResults(task, context));


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

             

            await    resultTask.ContinueWith(async task =>
                    await CommandResults(task, context));
            }
        }


        public async Task CommandResults(Task<IResult> resultTask, SocketCommandContextCustom context)
        {
       
            _global.TimeSpendOnLastMessage.Remove(context.User.Id, out var watch);

            var guildName = context.Guild == null ? "DM" : $"{context.Guild.Name}({context.Guild.Id})";

            if (!resultTask.Result.IsSuccess)
            {
                _log.Warning(
                    $"Command [{context.Message.Content}] by [{context.User}] [{guildName}] after {watch.Elapsed:m\\:ss\\.ffff}s.\n" +
                    $"Reason: {resultTask.Result.ErrorReason}", "CommandHandling");
                _log.Error(resultTask.Exception);


                if (!resultTask.Result.ErrorReason.Contains("Unknown command"))
                    await SendMessAsync($"Error! {resultTask.Result.ErrorReason}", context);
            }
            else
            {
                _global.TotalCommandsIssued++;

                _log.Info(
                    $"Command [{context.Message.Content}] by [{context.User}] [{guildName}] after {watch.Elapsed:m\\:ss\\.ffff}s.",
                    "CommandHandling");
            }

   

            await Task.CompletedTask;
        }
    }
}