using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckIfCommandGiveRole : ModuleBaseCustom
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public CheckIfCommandGiveRole(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
        {
            _serverAccounts = serverAccounts;
            _accounts = accounts;
            _global = global;

        }
        
        public async Task MessageReceived(SocketMessage message)
        {
            try
            {
                if (message.Author.IsBot || message.Content.Length > 30)
                {
                    return;
                }

                var account = _accounts.GetAccount(message.Author.Id);
                var server = _global.Client.Guilds.FirstOrDefault(x => x.Channels.Any(b => b.Id == message.Channel.Id));
                var guild = _serverAccounts.GetServerAccount(server);

                var rolesToGiveList = guild.Roles.ToList();

                if (rolesToGiveList.Count == 0)
                {
                    return;
                }



                var roleToGive = "2gagwerweghsxbd";


                var userCheck = "ju";
                if (account.MyPrefix !=null)
                    userCheck = message.Content.Substring(0, account.MyPrefix.Length);
                var serverCheck = message.Content.Substring(0, guild.Prefix.Length);

                if (serverCheck == guild.Prefix)
                {
                    roleToGive = message.Content.Substring(guild.Prefix.Length,
                        message.Content.Length - guild.Prefix.Length);
                }

                if (userCheck == account.MyPrefix)
                {
                    roleToGive = message.Content.Substring(account.MyPrefix.Length,
                        message.Content.Length - account.MyPrefix.Length);
                }
                if(userCheck != account.MyPrefix && serverCheck != guild.Prefix)
                    return;
                if (rolesToGiveList.Any(x => string.Equals(x.Key, roleToGive, StringComparison.CurrentCultureIgnoreCase)))
                {
                    SocketRole roleToAdd = null;

                    foreach (var t in rolesToGiveList)
                        if (string.Equals(t.Key, roleToGive, StringComparison.CurrentCultureIgnoreCase))
                            if (server != null)
                                roleToAdd = server.Roles.SingleOrDefault(x =>
                                    string.Equals(x.Name.ToString(), t.Value, StringComparison.CurrentCultureIgnoreCase));


                    if (!(message.Author is SocketGuildUser guildUser) || roleToAdd == null)
                        return;

                    var roleList = guildUser.Roles.ToArray();

                    try
                    {
                        if (roleList.Any(t => string.Equals(t.Name, roleToAdd.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            await guildUser.RemoveRoleAsync(roleToAdd);
                            await message.Channel.SendMessageAsync($"-{roleToAdd.Name}");
                            return;
                        }


                        await guildUser.AddRoleAsync(roleToAdd);
                        await message.Channel.SendMessageAsync( $"+{roleToAdd.Name}");
                    }
                    catch
                    {
                        await message.Channel.SendMessageAsync($"Error. Please make sure, that OctoBot's role is higher than yours");
                    }
                }
            }
            catch
            {
                //ignored
            }
       
        }
        

        public async Task Client_MessageReceived(SocketMessage message)
        {
            MessageReceived(message);
        }
        
    }
}