using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace OctoGame
{
    public sealed class Global : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public Global(DiscordShardedClient client)
        {
            Client = client;
            TimeBotStarted = DateTime.Now;
        }
        public readonly DiscordShardedClient Client;
        public readonly DateTime TimeBotStarted;
        public uint OctoGamePlaying { get; set; }
        public uint TotalCommandsIssued { get; set; }
        public uint TotalCommandsDeleted { get; set; }
        public uint TotalCommandsChanged { get; set; }
    


        public  List<List<OctoGameMessAndUserTrack>> OctopusGameMessIdList { get; internal set; } =
            new List<List<OctoGameMessAndUserTrack>>();




        //not data
   

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
