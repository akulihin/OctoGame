using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
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
        public ConcurrentDictionary<ulong, Stopwatch> TimeSpendOnLastMessage  = new ConcurrentDictionary<ulong, Stopwatch>();


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


        //OctoBot

        internal  ulong YellowTurlteChannelId { get; set; }
        internal  RestUserMessage YellowTurlteMessageTorack { get; set; }
        internal  int CommandEnabled { get; set; }

        public  List<OctoGameMessAndUserTrack2048> OctopusGameMessIdList2048 { get; internal set; } =
            new List<OctoGameMessAndUserTrack2048>();




        public struct OctoGameMessAndUserTrack2048
        {
            public ulong OctoGameMessIdToTrack2048;
            public ulong OctoGameUserIdToTrack2048;
            public RestUserMessage SocketMsg;
            public IUser Iuser;

            public OctoGameMessAndUserTrack2048(ulong octoGameMessIdToTrack2048, ulong octoGameUserIdToTrack2048,
                RestUserMessage socketMsg, IUser iuser)
            {
                OctoGameMessIdToTrack2048 = octoGameMessIdToTrack2048;
                OctoGameUserIdToTrack2048 = octoGameUserIdToTrack2048;
                SocketMsg = socketMsg;
                Iuser = iuser;
            }
        }






        public async Task<string> SendWebRequest(string requestUrl)
        {
            using var client = new HttpClient(new HttpClientHandler());
            client.DefaultRequestHeaders.Add("User-Agent", "OctoBot");
            using var response = await client.GetAsync(requestUrl);
            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode.ToString();
            return await response.Content.ReadAsStringAsync();
        }

    }

}
