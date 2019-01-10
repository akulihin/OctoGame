using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.GeneralCommands
{
    public class HelpModule :  ModuleBaseCustom
    {
        private readonly CommandService _commandService;
        private readonly IUserAccounts _userAccounts;
        private readonly IServerAccounts _serverAccounts;


        public HelpModule(CommandService commandService, IUserAccounts userAccounts, IServerAccounts serverAccounts)
        {
            _commandService = commandService;
            _userAccounts = userAccounts;
            _serverAccounts = serverAccounts;
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

            foreach (var c in moduleInfos) moduleWeNeed.AddRange(c.Commands.Where(h => string.Equals(h.Name.ToLower(), command.ToLower(), StringComparison.CurrentCultureIgnoreCase)));

            var module = commandModulesList.FirstOrDefault(m => m.Name.ToLower() == command.ToLower());
            if (module != null)
            {
                builder.AddField(GetModuleName(module), GetFullModuleInfo(module, guildAccount.Prefix));
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
                    builder.WithDescription(GetCommandInfo(moduleWeNeed[pageNum - 1], guildAccount.Prefix));

                    if (moduleWeNeed.Count >= 2)
                        builder.WithTitle($"Variant {pageNum}/{moduleWeNeed.Count}.\n" +
                                          "_______\n");
                }
            }

            await SendMessAsync(builder);
        }


        [Command("help")]
        [Alias("assist")]
        [Summary("Shows generic help menu.")]
        public async Task Help()
        {
            var commandModules = _commandService.Modules;

            var userAccount = _userAccounts.GetAccount(Context.User.Id);
            var guildAccount = _serverAccounts.GetServerAccount(Context.Guild);

            var botPrefix = $"`{guildAccount.Prefix}`";

            if (userAccount.MyPrefix != null && userAccount.MyPrefix.Length >= 1)
                botPrefix += $"**OR** `{userAccount.MyPrefix}`";

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

                builder.AddField(GetModuleName(module), GetShortModuleInfo(module));
            }

            await SendMessAsync(builder);
        }


        //TODO Move it as service

        private static string GetShortModuleInfo( ModuleInfo module)
        {
            var moduleCommands = string.Join(", ", module.Commands.Select(GetCommandName));
            var sb = new StringBuilder()
                .AppendLine(moduleCommands);
            return sb.ToString();
        }

        private static string GetCommandName( CommandInfo command)
        {
            return command.Module.Group != null ? $"{command.Module.Group} {command.Name}" : command.Name;
        }


        private static string GetFullModuleInfo(ModuleInfo module, string prefix)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var c in module.Commands)
            {
                i++;
                var parameters = string.Join(", ", GetCommandParameters(c));
                sb.AppendLine($"{i}. **Usage**: `{prefix}{c.Name} {parameters}`");
            }

            return sb.ToString();
        }

        private static IEnumerable<string> GetCommandParameters( CommandInfo command)
        {
            var parameters = command.Parameters;
            const string optionalTemplate = "<{0}>";
            const string  mandatoryTemplate = "[{0}]";

            return parameters.Select(parameter => parameter.IsOptional
                    ? string.Format(optionalTemplate, parameter.Name)
                    : string.Format(mandatoryTemplate, parameter.Name))
                .ToList();
        }

        private static string GetModuleName( ModuleInfo module)
        {
            return module.Remarks != null ? $"{module.Remarks} {module.Name}" : module.Name;
        }

        private static string GetCommandInfo( CommandInfo command, string prefix)
        {
            var aliases = string.Join(", ", GetCommandAliases(command));
            var module = command.Module.Name;
            var parameters = string.Join(", ", GetCommandParameters(command));
            var name = GetCommandName(command);
            var summary = command.Summary;
            var sb = new StringBuilder()
                .AppendLine($"**Command name**: {name}")
                .AppendLine($"**Module**: {module}")
                .AppendLine($"**Summary**: {summary}")
                .AppendLine($"**Usage**: `{prefix}{name} {parameters}`")
                .Append($"**Aliases**: {aliases}");
            return sb.ToString();
        }

        private static IEnumerable<string> GetCommandAliases( CommandInfo command)
        {
            return !string.IsNullOrEmpty(command.Module.Group) ? command.Aliases.Select(a => $"`{a}`") : command.Aliases;
        }
    }
} 