using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using OctoGame.DiscordFramework;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckReminders : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        private Timer _loopingTimer;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;
        private readonly LoginFromConsole _log;

        public CheckReminders(ServerAccounts serverAccounts, UserAccounts accounts, Global global, LoginFromConsole log)
        {
            _serverAccounts = serverAccounts;
            _accounts = accounts;
            _global = global;
            _log = log;
            CheckTimer();
        }
        private Task CheckTimer()
        {
            _loopingTimer = new Timer
            {
                AutoReset = true,
                Interval = 30000,
                Enabled = true
            };
            _loopingTimer.Elapsed += CheckForBirthdayRole;
            return Task.CompletedTask;
        }

        public  async void CheckForBirthdayRole(object sender, ElapsedEventArgs e)
        {
            try
            {
                var allUserAccounts = _accounts.GetAllAccount();
                var now = DateTime.UtcNow;

                foreach (var t in allUserAccounts)
                {
                    if (_global.Client.GetUser(t.DiscordId) == null)
                        continue;


                    var globalAccount = _global.Client.GetUser(t.DiscordId);
                    var account = _accounts.GetAccount(globalAccount);

                    var removeLaterList = new List<AccountSettings.CreateReminder>();

                    for (var j = 0; j < account.ReminderList?.Count; j++)
                    {
                        if (account.ReminderList[j].DateToPost > now || removeLaterList.Any( x => x.ReminderMessage == account.ReminderList[j].ReminderMessage))
                            continue;

                        try
                        {
                            var dmChannel = await globalAccount.GetOrCreateDMChannelAsync();
                            var embed = new EmbedBuilder();
                            embed.WithFooter("lil octo notebook");
                            embed.WithColor(Color.Teal);
                            embed.WithTitle("Pink turtle remindinds you:");
                            embed.WithDescription($"\n{account.ReminderList[j].ReminderMessage}");

                            await dmChannel.SendMessageAsync("", false, embed.Build());

                            removeLaterList.Add(account.ReminderList[j]);
                        }
                        catch (Exception closedDm)
                        {
                            try
                            {
                                _log.Error(
                                    $" [REMINDER] ({account.UserName}) - {account.ReminderList[j].ReminderMessage}");
                                if (!closedDm.Message.Contains("404") || !closedDm.Message.Contains("403")) continue;
                                account.ReminderList = new List<AccountSettings.CreateReminder>();
                                
                                return;
                            }
                            catch
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"ERROR REMINDER (Catch-catch) ?????? {account.UserName}");
                                Console.ResetColor();
                            }
                        }
                    }

                    if (removeLaterList.Any())
                    {
                        removeLaterList.ForEach(item => account.ReminderList.Remove(item));
                        
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR!!! REMINDER(Big try) Does not work: '{0}'", error);
            }
        }
    }
}