using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class CreateTemporaryVoiceChannel : ModuleBaseCustom
    {

        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;

        public CreateTemporaryVoiceChannel(UserAccounts accounts, ServerAccounts serverAccounts)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
        }

        [Command("voice")]
        [Summary(
            "Will create a voice channel under context Category. The owner is getting full Perms to do anything with the channel" +
            "He may have only 1 channel." +
            "As he leaves, after 10 minutes, he will transer the ownership to another person in that Voice Channel, and he may create another channel now" +
            "If no one there - Deletes the voice channel, and owner may create another channel" +
            "If you got the ownership from another person, you MAY NOT create second channel.")]
        public async Task CreateVoiceChannel(int maxSize = 5, string name = null)
        {
            var user = _accounts.GetAccount(Context.User);
            if (maxSize > 99) maxSize = 99;
            if (user.VoiceChannelList.Count >= 1)
            {
                await ReplyAsync($"You already have a channel.");
                return;
            }

            if (name == null)
            {
                name = $"{Context.User.Username}-Channel";
            }
            else
            {
                if (name.Length > 30) name = $"{Context.User.Username}-Channel";
            }

            var category = Context.Channel as ITextChannel;
            var voiceChannel = await Context.Guild.CreateVoiceChannelAsync(name, prop =>
            {
                prop.UserLimit = maxSize;
                prop.Bitrate = 64000;
                prop.Name = name;
                prop.CategoryId = category?.GetCategoryAsync().Result.Id;
            });
            await voiceChannel.AddPermissionOverwriteAsync(Context.User, OverwritePermissions.AllowAll(voiceChannel),
                RequestOptions.Default);


            if (category != null)
            {
                var newVoice =
                    new AccountSettings.CreateVoiceChannel(DateTime.UtcNow, voiceChannel.Id, category.Guild.Id);
                user.VoiceChannelList.Add(newVoice);
            }
            else
            {
                await ReplyAsync("error.");
                return;
            }

            await ReplyAsync(
                $"Voice Channel have been Created, please join it, or I will delete it in 10 min\n" +
                $"{voiceChannel.CreateInviteAsync().Result.Url}");
        }
    }
}