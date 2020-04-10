using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

    public class LvLing : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        private readonly UserAccounts _accounts;

        public LvLing(UserAccounts accounts)
        {
            _accounts = accounts;
        }

        public async Task UserSentMess(SocketMessage message)
        {
            // if the user a timeout, ignore them
            var user = message.Author as SocketGuildUser;
            if (user != null && (user.IsBot || user.IsMuted)) return;
            var channel = message.Channel as SocketTextChannel;
            var mess = message.Content;


            var option = mess.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            double wordsPoints = 0;
            double wordsPointsActivity = 0;

            foreach (var t in option)
                if (t.Length >= 4)
                {
                    wordsPoints = 0.1 * t.Length;
                    wordsPointsActivity = 2.5 * t.Length;
                }
                else
                {
                    wordsPoints = 0;
                    wordsPointsActivity = 0;
                }

            if (channel != null)
            {
                var userAccount = _accounts.GetAccount(user);
                userAccount.Points += 5 + (int) wordsPoints;
                userAccount.LvlPoinnts += 30 + (uint) wordsPointsActivity;
                if (user != null) userAccount.UserName = user.Username;

                userAccount.Lvl = Math.Sqrt(userAccount.LvlPoinnts / 150);
            }

            
        }

        public async Task Client_UserSentMess(SocketMessage message)
        {
            UserSentMess(message);
        }
    }
}