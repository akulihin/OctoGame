using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace OctoGame
{
    public sealed class Global : IService
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public Global(DiscordShardedClient client)
        {
            Client = client;
        }

        public DiscordShardedClient Client{get; set;}

        public int OctoGamePlaying { get; set; }

        public  List<List<OctoGameMessAndUserTrack>> OctopusGameMessIdList { get; internal set; } =
            new List<List<OctoGameMessAndUserTrack>>();

        public struct OctoGameMessAndUserTrack
        {           
            public IUserMessage GamingWindowFromBot;
            public IUser PlayerDiscordAccount;


            public OctoGameMessAndUserTrack(
                IUserMessage gamingWindowFromBot, IUser playerDiscordAccount)
            {

                GamingWindowFromBot = gamingWindowFromBot;
                PlayerDiscordAccount = playerDiscordAccount;

            }
        }


        public List<CommandRam> CommandList { get; set; } = new List<CommandRam>();

        public class CommandRam
        {
            public IUser BlogAuthor;
            public IUserMessage UserSocketMsg;
            public IUserMessage BotSocketMsg;
            

            public CommandRam(IUser blogAuthor, IUserMessage userSocketMsg, IUserMessage botSocketMsg)
            {
                BlogAuthor = blogAuthor;
                UserSocketMsg = userSocketMsg;
                BotSocketMsg = botSocketMsg;
            }
        }

        public List<OctoGameMessAndUserTrack> CreateNewGame(IUserMessage gamingWindowFromBot1, IUserMessage gamingWindowFromBot2, IUser playerDiscordAccount1, IUser playerDiscordAccount2)
        {

            var gameDataList = new List<OctoGameMessAndUserTrack>
            {
                new OctoGameMessAndUserTrack(gamingWindowFromBot1, playerDiscordAccount1),
                new OctoGameMessAndUserTrack(gamingWindowFromBot2, playerDiscordAccount2)
            };

            return gameDataList;
        }

    }

}
