using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckToDeleteVoiceChannel : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        private Timer _loopingTimer;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public CheckToDeleteVoiceChannel(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
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
                Interval = 30000,
                Enabled = true
            };
            _loopingTimer.Elapsed += CheckToDeleteVoice;    
            return Task.CompletedTask;
        }

        private  void CheckToDeleteVoice(object sender, ElapsedEventArgs e)
        {
            var allUserAccounts = _accounts.GetAllAccount();
            foreach (var t in allUserAccounts)
                try
                {
                    var globalAccount = _global.Client.GetUser(t.DiscordId);
                    var account = _accounts.GetAccount(globalAccount);
                    if (account.VoiceChannelList.Count <= 0)
                        continue;

                    var difference = DateTime.UtcNow - account.VoiceChannelList[0].LastTimeLeftChannel;

                    if (difference.Minutes > 10)
                    {
                        var voiceChan = _global.Client.GetGuild(account.VoiceChannelList[0].GuildId)
                            .GetVoiceChannel(account.VoiceChannelList[0].VoiceChannelId);

                        account.VoiceChannelList = new List<AccountSettings.CreateVoiceChannel>();
                        

                        if (voiceChan.Users.Count <= 0)
                        {
                            voiceChan.DeleteAsync();
                        }
                        else if (voiceChan.Users.Count >= 1)
                        {
                            var usersList = voiceChan.Users.ToList();
                            var newHolder = _accounts.GetAccount(usersList[0]);

                            var newVoice = new AccountSettings.CreateVoiceChannel(DateTime.UtcNow.AddHours(10),
                                voiceChan.Id, voiceChan.Guild.Id);
                            newHolder.VoiceChannelList.Add(newVoice);
                            
                            var guildUser = _global.Client.GetGuild(newHolder.VoiceChannelList[0].GuildId)
                                .GetUser(newHolder.Id);
                            var k = voiceChan.AddPermissionOverwriteAsync(guildUser,
                                OverwritePermissions.AllowAll(voiceChan),
                                RequestOptions.Default);
                            var kk = voiceChan.RemovePermissionOverwriteAsync(globalAccount, RequestOptions.Default);
                        }
                    }
                }
                catch
                {
                    //
                }
        }
    }
}