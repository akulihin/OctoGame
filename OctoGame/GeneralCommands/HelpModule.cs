using System.Threading.Tasks;
using Discord.Commands;
using OctoGame.DiscordFramework;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.LocalPersistentData.ServerAccounts;

namespace OctoGame.GeneralCommands
{
    public class HelpModule : ModuleBaseCustom
    {
        private readonly CommandService _commandService;
        private readonly IServerAccounts _serverAccounts;
        private readonly DiscordHelpModule _discordHelpModule;


        public HelpModule(CommandService commandService, IServerAccounts serverAccounts,
            DiscordHelpModule discordHelpModule)
        {
            _commandService = commandService;
            _serverAccounts = serverAccounts;
            _discordHelpModule = discordHelpModule;

        }


        [Command("help"), Alias("assist", "h"), Summary("Shows help menu. || Say <help commandName> to see help about specific command")]
        public async Task Help([Remainder] string command = null)
        {
            var embed = _discordHelpModule.GetDefaultHelpEmbed(_commandService, command, _serverAccounts.GetServerAccount(Context.Guild).Prefix);
            await SendMessAsync(embed);
        }
    }
}