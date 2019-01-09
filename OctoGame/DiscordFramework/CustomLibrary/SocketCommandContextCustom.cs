using Discord.Commands;
using Discord.WebSocket;

namespace OctoGame.DiscordFramework.CustomLibrary
{
    public class SocketCommandContextCustom : ShardedCommandContext
    {
        public string MessageContentForEdit { get; }
        public string Language { get; }
        public Global Global { get; }
     

        public SocketCommandContextCustom(DiscordShardedClient client, SocketUserMessage msg, Global global, string messageContentForEdit = null, string language = null) : base(client, msg)
        {
            if (language == null)
                language = "en";
            Language = language;
            MessageContentForEdit = messageContentForEdit;
            Global = global;
        }
    }
}