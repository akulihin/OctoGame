using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;

namespace OctoGame.DiscordFramework
{
    public class DiscordHelpModule
    {
        public Embed GetDefaultHelpEmbed(CommandService commandService, string command, string prefix)
        {
            EmbedBuilder helpEmbedBuilder;
            var commandModules = GetModulesWithCommands(commandService);
            var moduleMatch = commandModules.FirstOrDefault(m => m.Name == command);

            if (string.IsNullOrEmpty(command))
                helpEmbedBuilder = GenerateHelpCommandEmbed(commandService);
            else if (moduleMatch != null)
                helpEmbedBuilder = GenerateSpecificModuleHelpEmbed(moduleMatch);
            else
                helpEmbedBuilder = GenerateSpecificCommandHelpEmbed(commandService, command, prefix);

            helpEmbedBuilder.WithFooter(GenerateUsageFooterMessage(prefix));
            return helpEmbedBuilder.Build();
        }

        private string GenerateUsageFooterMessage(string botPrefix)
        {
            return $"Use {botPrefix}help [command module] or {botPrefix}help [command name] for more information.";
        }

        private IEnumerable<ModuleInfo> GetModulesWithCommands(CommandService commandService)
        {
            return commandService.Modules.Where(module => module.Commands.Count > 0);
        }

        private EmbedBuilder GenerateSpecificCommandHelpEmbed(CommandService commandService, string command,
            string prefix)
        {
            var isNumeric = int.TryParse(command[command.Length - 1].ToString(), out var pageNum);

            if (isNumeric)
                command = command.Substring(0, command.Length - 2);
            else
                pageNum = 1;

            var helpEmbedBuilder = new EmbedBuilder();
            var commandSearchResult = commandService.Search(command);


            var commandModulesList = commandService.Modules.ToList();
            var commandsInfoWeNeed = new List<CommandInfo>();
            foreach (var c in commandModulesList)
                commandsInfoWeNeed.AddRange(c.Commands.Where(h =>
                    string.Equals(h.Name, command, StringComparison.CurrentCultureIgnoreCase)));


            if (pageNum > commandsInfoWeNeed.Count || pageNum <= 0)
                pageNum = 1;


            if (!commandSearchResult.IsSuccess || commandsInfoWeNeed.Count <= 0)
            {
                helpEmbedBuilder.WithTitle("Command not found");
                return helpEmbedBuilder;
            }

            var commandInformation = GetCommandInfo(commandsInfoWeNeed[pageNum - 1], prefix);


            helpEmbedBuilder.WithDescription(commandInformation);

            if (commandsInfoWeNeed.Count >= 2)
                helpEmbedBuilder.WithTitle($"Variant {pageNum}/{commandsInfoWeNeed.Count}.\n" +
                                           "_______\n");

            return helpEmbedBuilder;
        }

        private EmbedBuilder GenerateSpecificModuleHelpEmbed(ModuleInfo module)
        {
            var helpEmbedBuilder = new EmbedBuilder();
            helpEmbedBuilder.AddField(GetModuleName(module), GetModuleInfo(module));
            return helpEmbedBuilder;
        }

        private EmbedBuilder GenerateHelpCommandEmbed(CommandService commandService)
        {
            var helpEmbedBuilder = new EmbedBuilder();
            var commandModules = GetModulesWithCommands(commandService);
            helpEmbedBuilder.WithTitle("How can I help you?");

            foreach (var module in commandModules)
                helpEmbedBuilder.AddField(GetModuleName(module), GetModuleInfo(module));
            return helpEmbedBuilder;
        }


        public string GetModuleInfo(ModuleInfo module)
        {
            var moduleCommands = string.Join(", ", module.Commands.Select(GetCommandNameWithGroup));
            var sb = new StringBuilder()
                .AppendLine(moduleCommands);
            return sb.ToString();
        }

        /// <summary>
        ///     Attach the remarks before the module name.
        ///     Useful to add an emote
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string GetModuleName(ModuleInfo module)
        {
            return module.Remarks != null ? $"{module.Remarks} {module.Name}" : module.Name;
        }

        public string GetCommandInfo(CommandInfo command, string prefix)
        {
            var aliases = string.Join(", ", command.Aliases);
            var module = command.Module.Name;
            var parameters = string.Join(", ", GetCommandParameters(command));
            var name = GetCommandNameWithGroup(command);
            var summary = command.Summary;
            var sb = new StringBuilder()
                .AppendLine($"**Command name**: {name}")
                .AppendLine($"**Module**: {module}")
                .AppendLine($"**Summary**: {summary}")
                .AppendLine($"**Usage**: {prefix}{name} {parameters}")
                .Append($"**Aliases**: {aliases}");
            return sb.ToString();
        }


        public IEnumerable<string> GetCommandParameters(CommandInfo command)
        {
            var parameters = command.Parameters;
            var optionalTemplate = "<{0}>";
            var mandatoryTemplate = "[{0}]";
            var parametersFormated = new List<string>();

            foreach (var parameter in parameters)
                if (parameter.IsOptional)
                    parametersFormated.Add(string.Format(optionalTemplate, parameter.Name));
                else
                    parametersFormated.Add(string.Format(mandatoryTemplate, parameter.Name));

            return parametersFormated;
        }

        /// <summary>
        ///     Returns the command name with the group name attached.
        ///     If there is no group, will return the command name.
        /// </summary>
        public string GetCommandNameWithGroup(CommandInfo commandInfo)
        {
            return commandInfo.Module.Group != null
                ? $"{commandInfo.Module.Group} {commandInfo.Name}"
                : commandInfo.Name;
        }
    }
}