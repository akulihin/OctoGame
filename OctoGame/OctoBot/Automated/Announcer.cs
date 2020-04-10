using System.Threading.Tasks;
using Discord.WebSocket;

namespace OctoGame.OctoBot.Automated
{
    public class Announcer : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        internal async Task AnnounceUserJoin(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;


            var kek = 1; // DELETE
            if (kek != 1) // DELETE
                await channel.SendMessageAsync($" {user.Mention}, Приветвсвую тебя в подводный мир осьминожек! ");
        }
    }
}