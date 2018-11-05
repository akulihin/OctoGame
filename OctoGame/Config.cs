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

        public  List<OctoGameMessAndUserTrack> OctopusGameMessIdList { get; internal set; } =
            new List<OctoGameMessAndUserTrack>();

        public struct OctoGameMessAndUserTrack
        {           
            public ulong OctoGameMessIdToTrack;
            public ulong OctoGameUserIdToTrack;
            public RestUserMessage SocketMsg;
            public IUser Iuser;


            public OctoGameMessAndUserTrack(ulong octoGameMessIdToTrack, ulong octoGameUserIdToTrack,
                RestUserMessage socketMsg, IUser iuser)
            {
                OctoGameMessIdToTrack = octoGameMessIdToTrack;
                OctoGameUserIdToTrack = octoGameUserIdToTrack;
                SocketMsg = socketMsg;
                Iuser = iuser;
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