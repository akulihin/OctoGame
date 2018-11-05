using Discord.Commands;
using Discord.WebSocket;

namespace OctoGame.Framework.Library
{
    public class SocketCommandContextCustom : ShardedCommandContext
    {

        public string MessageContentForEdit { get; }
        public string Language { get; }

        public SocketCommandContextCustom(DiscordShardedClient client, SocketUserMessage msg, string messageContentForEdit = null, string language = null) : base(client, msg)
        {
            if (language == null)
                language = "en";
            Language = language;
            MessageContentForEdit = messageContentForEdit;
        }       
    }
}