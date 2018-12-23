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
    public class General : ModuleBase<SocketCommandContextCustom>
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        readonly AuthDiscordBotListApi _dblApi = new AuthDiscordBotListApi(423593006436712458, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjQyMzU5MzAwNjQzNjcxMjQ1OCIsImJvdCI6dHJ1ZSwiaWF0IjoxNTMxODgwOTA3fQ.iFFl7gft4yI_ZysVbFoXW_VEUS_kUddQLlLb0kqG9mM");
        private readonly ILocalization _lang;
        private readonly IUserAccounts _accounts;
        private readonly CommandHandeling _command;
        private readonly SecureRandom _secureRandom;
        private readonly OctoPicPull _octoPicPull;
        private readonly OctoNamePull _octoNmaNamePull;
        private readonly HelperFunctions _helperFunctions;
        private readonly AudioService _service;

        public General(ILocalization lang, IUserAccounts accounts, CommandHandeling command, SecureRandom secureRandom, OctoPicPull octoPicPull, OctoNamePull octoNmaNamePull, HelperFunctions helperFunctions, AudioService service)
        {
            _lang = lang;
            _accounts = accounts;
            _command = command;
            _secureRandom = secureRandom;
            _octoPicPull = octoPicPull;
            _octoNmaNamePull = octoNmaNamePull;
            _helperFunctions = helperFunctions;
            _service = service;
        }

        [Command("upd")]
        [RequireOwner]
        public async Task UpdateDiscordBotListGuildCount(int num)
        {
            _dblApi.UpdateStats(num);
            _command.ReplyAsync(Context, $"updated");
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
           await _service.LeaveAudio(Context.Guild);
        }
    
        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        [Command("myPrefix")]
        public async Task SetMyPrefix([Remainder] string prefix = null)
        {
            var account = _accounts.GetAccount(Context.User);

            if (prefix == null)
            {
                await _command.ReplyAsync(Context,
                    $"Your prefix: **{account.MyPrefix}**");
                return;
            }

            if (prefix.Length < 100)
            {
                account.MyPrefix = prefix;
                if (prefix.Contains("everyone") || prefix.Contains("here"))
                {
                    await _command.ReplyAsync(Context,
                        "Boooooo! no `here` or `everyone` prefix!");
                    return;
                }

                _accounts.SaveAccounts(Context.User);
                await _command.ReplyAsync(Context,
                    $"Booole~, your own prefix is now **{prefix}**");
            }
            else
            {
                await _command.ReplyAsync(Context,
                    "Booooo! Prefix have to be less than 100 characters");
            }
        }

        [Command("octo")]
        [Alias("окто", "octopus", "Осьминог", "Осьминожка", "Осьминога", "o", "oct", "о")]
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

            await _command.ReplyAsync(Context, embed);

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
        public async Task TestLanguges([Remainder] string rem)
        {
            var kek = _lang.Resolve($"{Context.User.Mention}\n[PRIVACY_DATA_REPORT_TEMPLATE(@DATA)]");
            _command.ReplyAsync(Context, $"{kek}");
        }
    }
}