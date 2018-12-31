using System.Collections.Generic;
using System.IO;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace OctoGame
{
    public class Global
    {
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
            public IUserMessage BotGamingMsg1;
            public IUser Player1;


            public OctoGameMessAndUserTrack(
                IUserMessage botGamingMsg1, IUser player1)
            {

                BotGamingMsg1 = botGamingMsg1;
                Player1 = player1;

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

        public List<OctoGameMessAndUserTrack> CreateNewGame(IUserMessage botGamingMsg1, IUserMessage botGamingMsg2, IUser player1, IUser player2)
        {

            var gameDataList = new List<OctoGameMessAndUserTrack>
            {
                new OctoGameMessAndUserTrack(botGamingMsg1, player1),
                new OctoGameMessAndUserTrack(botGamingMsg2, player2)
            };

            return gameDataList;
        }

    }

    internal class Config
    {
        public static BotConfig Bot;
        
        static Config()
        {
            var json = File.ReadAllText(@"OctoDataBase/config.json");
            Bot = JsonConvert.DeserializeObject<BotConfig>(json);
        }
    }

    public struct BotConfig
    {
        public string Token;
    }
}