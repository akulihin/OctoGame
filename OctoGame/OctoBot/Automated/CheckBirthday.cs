using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot.Automated
{
    public class CheckBirthday : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;
        private  Timer _loopingTimer;
        private readonly ServerAccounts _serverAccounts;
        private readonly UserAccounts _accounts;
        private readonly Global _global;

        public CheckBirthday(ServerAccounts serverAccounts, UserAccounts accounts, Global global)
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
                Interval = 60000,
                Enabled = true
            };
            _loopingTimer.Elapsed += CheckAllBirthdays;
            return Task.CompletedTask;
        }

        public  async void CheckAllBirthdays(object sender, ElapsedEventArgs e)
        {
            try
            {

                var allServersWithBirthdayRole = _serverAccounts.GetFilteredServerAccounts(s => s.BirthdayRoleId != 0);
                var timeUtcNow = DateTime.UtcNow;
                var removeLaterList = new List<ServerSettings.BirthdayRoleActive>();

                foreach (var server in allServersWithBirthdayRole)
                {
                    var allUserAccounts = _accounts.GetOrAddUserAccountsForGuild(server.ServerId);

                    foreach (var t in allUserAccounts)
                    {
                        if (t.Birthday.ToString(CultureInfo.InvariantCulture) == "0001-01-01T00:00:00")
                            continue;

                        if(_global.Client.Guilds.All(x => x.Id != server.ServerId))
                            continue;

                        var globalAccount = _global.Client.GetUser(t.DiscordId);

                            if(globalAccount == null)
                                continue;

                        var account = _accounts.GetAccount(globalAccount);
                        var timezone = account.TimeZone ?? "UTC";

                        try
                        {
                            var fftz = TimeZoneInfo.FindSystemTimeZoneById($"{timezone}");
                        }
                        catch
                        {
                            account.TimeZone = "UTC";
                            
                            Console.WriteLine($"{account.UserName} TimeZone changed to UTC");
                            continue;
                        }

                        var tz = TimeZoneInfo.FindSystemTimeZoneById($"{timezone}");

                        var timeWhenIsBirthdayByUtc = TimeZoneInfo.ConvertTimeToUtc(account.Birthday, tz);
                        var roleToGive = _global.Client.GetGuild(server.ServerId).GetRole(server.BirthdayRoleId);


                        if (roleToGive == null)
                        {
                            server.BirthdayRoleId = 0;
                            
                            Console.WriteLine($"Birthday Role == NULL ({server.ServerName} - {server.ServerId})");
                            return;
                        }

                        /*
                        if(account.Id == 181514288278536193)
                        Console.WriteLine($"account == {account.UserName}\n" +
                                          $"timeUtcNow.Month == {timeUtcNow.Month}\n" +
                                          $"timeWhenIsBirthdayByUtc == {timeWhenIsBirthdayByUtc.Month}\n" +
                                          $"timeUtcNow.Day == {timeUtcNow.Day}\n" +
                                          $"timeWhenIsBirthdayByUtc.Day == {timeWhenIsBirthdayByUtc.Day}\n" +
                                          $"{server.BirthdayRoleList.Any(x => x.UserId != account.Id).ToString()}");
*/

                        var check = 0;
                        foreach (var l in server.BirthdayRoleList)
                        {
                            if (l.UserId == account.Id)
                                check = 1;
                        }

                        if (timeUtcNow.Month == timeWhenIsBirthdayByUtc.Month &&
                            timeUtcNow.Day == timeWhenIsBirthdayByUtc.Day
                            && check == 0)
                        {
                            Console.WriteLine("here");
                            await _global.Client.GetGuild(server.ServerId).GetUser(account.Id)
                                .AddRoleAsync(roleToGive);

                            var newBirthday = new ServerSettings.BirthdayRoleActive(timeUtcNow +
                                                                                    TimeSpan.ParseExact("1d",
                                                                                        ReminderFormat.Formats,
                                                                                        CultureInfo
                                                                                            .CurrentCulture),
                                account.Id, account.UserName);

                            server.BirthdayRoleList.Add(newBirthday);
                            
                        }


                        if (server.BirthdayRoleList.Any(x => x.UserId == account.Id))
                            foreach (var v in server.BirthdayRoleList)
                                if (timeUtcNow > v.DateToRemoveRole)
                                {
                                    Console.WriteLine("removed");
                                    await _global.Client.GetGuild(server.ServerId).GetUser(v.UserId)
                                        .RemoveRoleAsync(roleToGive);
                                    removeLaterList.Add(v);
                                }
                    }
                    if (removeLaterList.Any())
                    {
                        removeLaterList.ForEach(item => server.BirthdayRoleList.Remove(item));
                        
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR!!! Birthday Role(Big try) Does not work: '{0}'", error);
            }
        }
    }
}