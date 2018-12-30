using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Commands;

namespace OctoGame.DiscordFramework
{
    public class DiscordHelpModule
    {
        public  string GetFullModuleInfo(ModuleInfo module, string prefix)
        {
            var sb = new StringBuilder();
            foreach (var c in module.Commands)
            {
                var parameters = string.Join(", ", GetCommandParameters(c));
                sb.AppendLine($"**Usage**: `{prefix}{c.Name} {parameters}`");
            }

            return sb.ToString();
        }

        public string GetShortModuleInfo( ModuleInfo module)
        {
            var moduleCommands = string.Join(", ", module.Commands.Select(GetCommandName));
            var sb = new StringBuilder()
                .AppendLine(moduleCommands);
            return sb.ToString();
        }


        public  string GetModuleName( ModuleInfo module)
        {
            return module.Remarks != null ? $"{module.Remarks} {module.Name}" : module.Name;
        }

        public  string GetCommandInfo( CommandInfo command, string prefix)
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

        public  IEnumerable<string> GetCommandParameters( CommandInfo command)
        {
            var parameters = command.Parameters;
            const string optionalTemplate = "<{0}>";
            const string  mandatoryTemplate = "[{0}]";

            return parameters.Select(parameter => parameter.IsOptional
                    ? string.Format(optionalTemplate, parameter.Name)
                    : string.Format(mandatoryTemplate, parameter.Name))
                .ToList();
        }

        public  IEnumerable<string> GetCommandAliases( CommandInfo command)
        {
            var group = command.Module.Group;
            return !string.IsNullOrEmpty(@group) ? command.Aliases.Select(a => $"`{a}`") : command.Aliases;
        }

        public  string GetCommandName( CommandInfo command)
        {
            return command.Module.Group != null ? $"{command.Module.Group} {command.Name}" : command.Name;
        }
    }
}