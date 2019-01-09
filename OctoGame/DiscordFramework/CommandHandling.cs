using System.Diagnostics;
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

        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;
        private readonly Global _global;
        private readonly Scope _services;
        private readonly IUserAccounts _accounts;
        private readonly IServerAccounts _serverAccounts;
        private readonly LoginFromConsole _log;


        public CommandHandling(CommandService commands,
            DiscordShardedClient client, IUserAccounts accounts, IServerAccounts serverAccounts, Global global,
            Scope scope, LoginFromConsole log)
        {
            _commands = commands;
            _services = scope;
            _log = log;
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

        public async Task _client_MessageDeleted(Cacheable<IMessage, ulong> cacheMessage, ISocketMessageChannel channel)
        {
            foreach (var t in _global.CommandList)
                if (cacheMessage.Value.Id == t.UserSocketMsg.Id)
                {
                  await  t.BotSocketMsg.DeleteAsync();
                    _global.CommandList.Remove(t);
                }

            await Task.CompletedTask;
        }

        public async Task _client_MessageUpdated(Cacheable<IMessage, ulong> messageBefore,
            SocketMessage messageAfter, ISocketMessageChannel arg3)
        {
            var watch = Stopwatch.StartNew();
            watch.Start();

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
                    var resultTask = Task.FromResult(await _commands.ExecuteAsync(
                        context,
                        argPos,
                        _services));

                    watch.Stop();

                    await resultTask.ContinueWith(async task =>
                        await CommandResults(task, context, watch));

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
                    watch.Stop();

                    await resultTask.ContinueWith(async task =>
                        await CommandResults(task, context, watch));
                }

                return;
            }

            watch.Stop();

            await HandleCommandAsync(messageAfter);
        }


        public async Task HandleCommandAsync(SocketMessage msg)
        {
            var watch = Stopwatch.StartNew();
            watch.Start();

            var message = msg as SocketUserMessage;
            if (message == null) return;
            var account = _accounts.GetAccount(msg.Author);
            var context = new SocketCommandContextCustom(_client, message, _global, null, account.MyLanguage);
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

                    watch.Stop();


                  await  resultTask.ContinueWith(async task =>
                        await CommandResults(task, context, watch));


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

                watch.Stop();

            await    resultTask.ContinueWith(async task =>
                    await CommandResults(task, context, watch));
            }
        }


        public async Task CommandResults(Task<IResult> resultTask, SocketCommandContextCustom context, Stopwatch watch)
        {
            var guildName = context.Guild == null ? "DM" : $"{context.Guild.Name}({context.Guild.Id})";

            if (!resultTask.Result.IsSuccess)
            {
                _log.Warning(
                    $"Command [{context.Message.Content}] by [{context.User}] [{guildName}] after {watch.Elapsed:m\\:ss\\.ffff}s.\n" +
                    $"Reason: {resultTask.Result.ErrorReason}", "CommandHandler");
                _log.Error(resultTask.Exception);


                if (!resultTask.Result.ErrorReason.Contains("Unknown command"))
                    await SendMessAsync($"Error! {resultTask.Result.ErrorReason}", context);
            }
            else
            {
                _log.Info(
                    $"Command [{context.Message.Content}] by [{context.User}] [{guildName}] after {watch.Elapsed:m\\:ss\\.ffff}s.",
                    "CommandHandler");
            }

            await Task.CompletedTask;
        }
    }
}