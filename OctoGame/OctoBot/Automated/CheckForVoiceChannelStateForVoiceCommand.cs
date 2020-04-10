using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckForVoiceChannelStateForVoiceCommand : IServiceSingleton
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public CheckForVoiceChannelStateForVoiceCommand(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
        {
            _serverAccounts = serverAccounts;
            _accounts = accounts;
            _global = global;
        }

        public async Task UserVoiceStateUpdatedForCreateVoiceChannel(SocketUser user,
            SocketVoiceState voiceStateBefore,
            SocketVoiceState voiceStateAfter)
        {
            var userAccount = _accounts.GetAccount(user);
            if (userAccount.VoiceChannelList.Count <= 0)
                return;
            if (voiceStateAfter.VoiceChannel?.Id != userAccount.VoiceChannelList[0].VoiceChannelId &&
                voiceStateBefore.VoiceChannel.Id != userAccount.VoiceChannelList[0].VoiceChannelId)
                return;


            var voiceChannelId = userAccount.VoiceChannelList[0].VoiceChannelId;
            var guildId = userAccount.VoiceChannelList[0].GuildId;

            if (voiceStateAfter.VoiceChannel != null && voiceStateAfter.VoiceChannel.Users.Contains(user))
            {
                var newVoice =
                    new AccountSettings.CreateVoiceChannel(DateTime.UtcNow.AddHours(10), voiceChannelId, guildId);
                userAccount.VoiceChannelList[0] = newVoice;
            }

            if (voiceStateAfter.VoiceChannel == null || !voiceStateAfter.VoiceChannel.Users.Contains(user))
            {
                var newVoice = new AccountSettings.CreateVoiceChannel(DateTime.UtcNow, voiceChannelId, guildId);
                userAccount.VoiceChannelList[0] = newVoice;
            }
        }


        public async Task Client_UserVoiceStateUpdatedForCreateVoiceChannel(SocketUser user,
            SocketVoiceState voiceStateBefore,
            SocketVoiceState voiceStateAfter)
        {
            UserVoiceStateUpdatedForCreateVoiceChannel(user, voiceStateBefore, voiceStateAfter);
        }
    }
}