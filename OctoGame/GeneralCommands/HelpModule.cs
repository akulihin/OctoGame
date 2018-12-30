using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OctoGame.DiscordFramework;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.GeneralCommands
{
    public class HelpModule : ModuleBase<SocketCommandContextCustom>
    {
        private readonly CommandService _commandService;
        private readonly IUserAccounts _userAccounts;
        private readonly IServerAccounts _serverAccounts;
        private readonly DiscordHelpModule _discordHelpModule;
        private readonly CommandHandeling _command;

        public HelpModule(CommandService commandService, IUserAccounts userAccounts, IServerAccounts serverAccounts,
            DiscordHelpModule discordHelpModule, CommandHandeling command)
        {
            _commandService = commandService;
            _userAccounts = userAccounts;
            _serverAccounts = serverAccounts;
            _discordHelpModule = discordHelpModule;
            _command = command;
        }

        [Command("help")]
        [Alias("assist")]
        [Summary(
            "Shows help about specific command or module. If command has 2 or more variants you can select a page, for example `help roll 2`")]
        public async Task HelpSpecific([Remainder] string command)
        {
            var isNumeric = int.TryParse(command[command.Length - 1].ToString(), out var pageNum);

            if (isNumeric)
                command = command.Substring(0, command.Length - 2);
            else
                pageNum = 1;


            var commandModules = _commandService.Modules;

            var guildAccount = _serverAccounts.GetServerAccount(Context.Guild);

            var builder = new EmbedBuilder();
            builder.WithFooter("Parameters between [ ] are mandatory, and < > are optional.");

            var commandModulesList = commandModules.ToList();
            var moduleInfos = commandModulesList.ToList();
            var moduleWeNeed = new List<CommandInfo>();

            foreach (var c in moduleInfos) moduleWeNeed.AddRange(c.Commands.Where(h => string.Equals(h.Name, command, StringComparison.CurrentCultureIgnoreCase)));

            var module = commandModulesList.FirstOrDefault(m => m.Name == command);
            if (module != null)
            {
                builder.AddField(_discordHelpModule.GetModuleName(module), _discordHelpModule.GetFullModuleInfo(module, guildAccount.Prefix));
            }
            else
            {
                var result = _commandService.Search(Context, command);

                if (!result.IsSuccess)
                {
                    builder.WithTitle("Command not found");
                }
                else
                {
                    builder.WithDescription(_discordHelpModule.GetCommandInfo(moduleWeNeed[pageNum - 1], guildAccount.Prefix));
                    
                    if (moduleWeNeed.Count >= 2)
                        builder.WithTitle($"Variant {pageNum}/{moduleWeNeed.Count}.\n" +
                                          "_______\n");
                }
            }

            await _command.ReplyAsync(Context, builder);
        }


        [Command("help")]
        [Alias("assist")]
        [Summary("Shows generic help menu.")]
        public async Task Help()
        {
            var commandModules = _commandService.Modules;

            var userAccount = _userAccounts.GetAccount(Context.User.Id);
            var guildAccount = _serverAccounts.GetServerAccount(Context.Guild);

            var botPrefix = $"{guildAccount.Prefix}";

            if (userAccount.MyPrefix != null && userAccount.MyPrefix.Length >= 1)
                botPrefix += $"**OR** {userAccount.MyPrefix}";

            var footerMessage =
                $"Use {guildAccount.Prefix}help [command module] or {guildAccount.Prefix}help [command name] for more information.";

            var builder = new EmbedBuilder()
                .WithFooter(footerMessage)       
                .AddField("General", "• If you will **edit** a command message - bot will edit the response\n" +
                                     "• If you will **delete** a command message - bot will delete the response\n" +
                                     $"• Prefix: {botPrefix}");

            builder.WithTitle("This is a list of things you can ask me to do");
            foreach (var module in commandModules)
            {
                if (module.Commands.Count <= 0) continue;

                builder.AddField(_discordHelpModule.GetModuleName(module), _discordHelpModule.GetShortModuleInfo(module));
            }

            await _command.ReplyAsync(Context, builder);
        }
    }
}