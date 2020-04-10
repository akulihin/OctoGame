using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckForMute : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        private Timer _loopingTimer;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public CheckForMute(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
        {
            _serverAccounts = serverAccounts;
            _accounts = accounts;
            _global = global;
            CheckTimer();
        }

        internal  Task CheckTimer()
        {
            _loopingTimer = new Timer
            {
                AutoReset = true,
                Interval = 5000,
                Enabled = true
            };

            _loopingTimer.Elapsed += ChekAllMutes;


            return Task.CompletedTask;
        }

        public  async void ChekAllMutes(object sender, ElapsedEventArgs e)

        {
            return;
            try
            {
                var allUserAccounts = _accounts.GetOrAddUserAccountsForGuild(0);
                var now = DateTime.UtcNow;

                foreach (var t in allUserAccounts)
                {
                    if (_global.Client.GetUser(t.Id) == null)
                        continue;

                    var globalAccount = _global.Client.GetUser(t.DiscordId);
                    var account = _accounts.GetAccount(globalAccount);

                    if (account.MuteTimer <= now && account.MuteTimer != Convert.ToDateTime("0001-01-01T00:00:00"))
                    {
                        var roleToGive = _global.Client.GetGuild(338355570669256705).Roles
                            .SingleOrDefault(x => x.Name.ToString() == "Muted");
                        var wtf = _global.Client.GetGuild(338355570669256705).GetUser(account.Id);
                        await wtf.RemoveRoleAsync(roleToGive);
                        await wtf.ModifyAsync(u => u.Mute = false);
                        account.MuteTimer = Convert.ToDateTime("0001-01-01T00:00:00");
                        _accounts.SaveAccounts(0);

                        try
                        {
                            var dmChannel = await globalAccount.GetOrCreateDMChannelAsync();
                            var embed = new EmbedBuilder();
                            embed.WithFooter("lil octo notebook");
                            embed.WithColor(Color.Red);
                            embed.WithImageUrl("https://i.imgur.com/puNz7pu.jpg");
                            embed.WithDescription($"бу-бу-бу!\nБольше так не делай, тебя размутили.");

                            await dmChannel.SendMessageAsync("", false, embed.Build());
                        }
                        catch
                        {
                            var embed = new EmbedBuilder();
                            embed.WithFooter("lil octo notebook");
                            embed.WithColor(Color.Red);
                            embed.WithImageUrl("https://i.imgur.com/puNz7pu.jpg");
                            embed.WithDescription($"бу-бу-бу!\nБольше так не делай, тебя размутили.");

                            await _global.Client.GetGuild(338355570669256705).GetTextChannel(374914059679694848)
                                .SendMessageAsync("", false, embed.Build());
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR!!! ChekAllMutes(Big try) Does not work: '{0}'", error);
            }
        }
    }
}