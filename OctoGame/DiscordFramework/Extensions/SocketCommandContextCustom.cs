using Discord.Commands;
using Discord.WebSocket;

namespace OctoGame.DiscordFramework.Extensions
{
    public class SocketCommandContextCustom : ShardedCommandContext
    {
        public string MessageContentForEdit { get; }
        public string Language { get; }
        public CommandsInMemory CommandsInMemory { get; }
     

        public SocketCommandContextCustom(DiscordShardedClient client, SocketUserMessage msg, CommandsInMemory commandsInMemory, string messageContentForEdit = null, string language = null) : base(client, msg)
        {
            if (language == null)
                language = "en";
            CommandsInMemory = commandsInMemory;
            Language = language;
            MessageContentForEdit = messageContentForEdit;
        }
    }
}