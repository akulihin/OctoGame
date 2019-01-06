using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotsList.Api;
using OctoGame.DiscordFramework;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.DiscordFramework.Language;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.GeneralCommands
{
    public class General : ModuleBaseCustom
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        readonly AuthDiscordBotListApi _dblApi = new AuthDiscordBotListApi(423593006436712458, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjQyMzU5MzAwNjQzNjcxMjQ1OCIsImJvdCI6dHJ1ZSwiaWF0IjoxNTMxODgwOTA3fQ.iFFl7gft4yI_ZysVbFoXW_VEUS_kUddQLlLb0kqG9mM");
        private readonly ILocalization _lang;
        private readonly IUserAccounts _accounts;

        private readonly SecureRandom _secureRandom;
        private readonly OctoPicPull _octoPicPull;
        private readonly OctoNamePull _octoNmaNamePull;
        private readonly HelperFunctions _helperFunctions;
        private readonly AudioService _service;

        public General(ILocalization lang, IUserAccounts accounts, SecureRandom secureRandom, OctoPicPull octoPicPull, OctoNamePull octoNmaNamePull, HelperFunctions helperFunctions, AudioService service)
        {
            _lang = lang;
            _accounts = accounts;

            _secureRandom = secureRandom;
            _octoPicPull = octoPicPull;
            _octoNmaNamePull = octoNmaNamePull;
            _helperFunctions = helperFunctions;
            _service = service;
        }


        [Command("tt")]
        [Summary("doing absolutely nothing. That's right - NOTHING")]
        public async Task Ttest([Remainder]string st)
        {
            await Task.CompletedTask;
        }



        [Command("upd")]
        [RequireOwner]
        [Summary("RequireOwner")]
        public async Task UpdateDiscordBotListGuildCount(int num)
        {
            _dblApi.UpdateStats(num);
            SendMessAsync( $"updated");
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins a voice channel")]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState)?.VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Summary("leaves a voice channel")]
        public async Task LeaveCmd()
        {
           await _service.LeaveAudio(Context.Guild);
        }
    
        [Command("play", RunMode = RunMode.Async)]
        [Summary("plays only 1 song. for now. Just because.")]
        public async Task PlayCmd([Remainder] string song = "nothing")
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        [Command("myPrefix")]
        [Summary("Shows or sets YOUR OWN prefix for the bot")]
        public async Task SetMyPrefix([Remainder] string prefix = null)
        {
            var account = _accounts.GetAccount(Context.User);

            if (prefix == null)
            {
                var myAccountPrefix = account.MyPrefix ?? "You don't have own prefix yet";

                await SendMessAsync(
                    $"Your prefix: **{myAccountPrefix}**");
                return;
            }

            if (prefix.Length < 100)
            {
                account.MyPrefix = prefix;
                if (prefix.Contains("everyone") || prefix.Contains("here"))
                {
                    await SendMessAsync(
                        "No `here` or `everyone` prefix allowed.");
                    return;
                }

                _accounts.SaveAccounts(Context.User);
                await SendMessAsync(
                    $"Booole~, your own prefix is now **{prefix}**");
            }
            else
            {
                await SendMessAsync(
                    "Booooo! Prefix have to be less than 100 characters");
            }
        }

        [Command("octo")]
        [Alias("окто", "octopus", "Осьминог", "Осьминожка", "Осьминога", "o", "oct", "о")]
        [Summary("Show a random oct. The pull contains only my own pictures.")]
        public async Task OctopusPicture()
        {
            var octoIndex = _secureRandom.Random(0, _octoPicPull.OctoPics.Length-1);
            var octoToPost = _octoPicPull.OctoPics[octoIndex];


            var color1Index = _secureRandom.Random(0, 255);
            var color2Index = _secureRandom.Random(0, 255);
            var color3Index = _secureRandom.Random(0, 255);

            var randomIndex = _secureRandom.Random(0, _octoNmaNamePull.OctoNameRu.Length-1);
            var randomOcto = _octoNmaNamePull.OctoNameRu[randomIndex];

            var embed = new EmbedBuilder();
            embed.WithDescription($"{randomOcto} found:");
            embed.WithFooter("lil octo notebook");
            embed.WithColor(color1Index, color2Index, color3Index);
            embed.WithAuthor(Context.User);
            embed.WithImageUrl("" + octoToPost);

            await SendMessAsync( embed);

            switch (octoIndex)
            {
                case 19:
                    {
                        var lll = await Context.Channel.SendMessageAsync("Ooooo, it was I who just passed Dark Souls!");
                        _helperFunctions.DeleteMessOverTime(lll, 6);
                        break;
                    }
                case 9:
                    {
                        var lll = await Context.Channel.SendMessageAsync("I'm drawing an octopus :3");
                        _helperFunctions.DeleteMessOverTime(lll, 6);
                        break;
                    }
                case 26:
                    {
                        var lll = await Context.Channel.SendMessageAsync(
                            "Oh, this is New Year! time to gift turtles!!");
                        _helperFunctions.DeleteMessOverTime(lll, 6);
                        break;
                    }
            }
        }


        [Command("test")]
        [Summary("just a test of a multiple language shit")]
        public async Task TestLanguges([Remainder] string rem)
        {
            var kek = _lang.Resolve($"{Context.User.Mention}\n[PRIVACY_DATA_REPORT_TEMPLATE(@DATA)]");
            SendMessAsync( $"{kek}");
        }
    }
}