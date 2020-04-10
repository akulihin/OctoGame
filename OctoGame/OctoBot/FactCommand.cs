using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class Fact : ModuleBaseCustom
    {
        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;

        public Fact(UserAccounts accounts, ServerAccounts serverAccounts)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
        }

        [Command("записать")]
        [Alias("факт", "write", "write down", "fact")]
        public async Task WriteFuckt(IGuildUser user, [Remainder] string message)
        {
            try
            {
                var account = _accounts.GetAccount((SocketUser) user);
                if (account == null)
                    return;

                account.Fuckt += message + "|";
                
                var id = Context.Message.Id;


                var msg = await Context.Channel.GetMessageAsync(id);
                await msg.DeleteAsync();


                await SendMessAsync(
                    $"We wrote down this fact about {user.Mention}!");
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **fact [user_ping(or user ID)] [message]**(write down a fact about user!)\n" +
             //       "Alias: факт, write, fact, write down");
            }
        }

        [Command("факт")]
        [Alias("fact")]
        public async Task ReadFuckt(SocketUser user)
        {
            try
            {
                var account = _accounts.GetAccount(user);

                if (account.Fuckt == null)
                {
                    await SendMessAsync(
                        "boole. :c\nWe could not find the facts about this user");


                    return;
                }

                var randomFuktArr = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                var rand = new Random();
                var randomIndex = rand.Next(randomFuktArr.Length);
                var randomFukt = $"{randomFuktArr[randomIndex]}";


                string httpsCheck = null;
                if (randomFukt.Length >= 5)
                    httpsCheck = $"{randomFukt[0]}{randomFukt[1]}{randomFukt[2]}{randomFukt[3]}{randomFukt[4]}";


                //onsole.WriteLine($"Длина: {RandomFuktArr.Length} | Индекс: {randomIndex} | HTTP Check: {httpsCheck}");

                string randomNick = null;
                if (account.ExtraUserName != null)
                {
                    var extra = account.ExtraUserName.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                    var randomIndexTwo = rand.Next(extra.Length);
                    randomNick = $"{extra[randomIndexTwo]}";
                }


                var embed = new EmbedBuilder();
                embed.WithColor(Color.Purple);
                embed.WithAuthor(user);
                embed.WithFooter("lil octo notebook");
                if (randomNick != null) embed.AddField("Was seen under the nickname: ", " " + randomNick);

                if (httpsCheck == "https")
                    embed.WithImageUrl($"{randomFukt}");
                else
                    embed.AddField("Random fact: ", " " + randomFukt);


                await SendMessAsync( embed);
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **fact [user_ping(or user ID)]**(show a random fact about user)");
            }
        }

        [Command("факт")]
        [Alias("fact")]
        public async Task ReadFucktIndex(SocketUser user, int index)
        {
            try
            {
                var account = _accounts.GetAccount(user);
                var comander = _accounts.GetAccount(Context.User);
                if (comander.OctoPass >= 10)
                {
                    if (account.Fuckt == null)
                    {
                        await SendMessAsync(
                            "boole. :c\nWe could not find the facts about this user");


                        return;
                    }

                    var randomFuktArr = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                    var rand = new Random();

                    var randomFukt = $"{randomFuktArr[index]}";

                    string httpsCheck = null;
                    if (randomFukt.Length >= 5)
                        httpsCheck = $"{randomFukt[0]}{randomFukt[1]}{randomFukt[2]}{randomFukt[3]}{randomFukt[4]}";


                    string randomNick = null;
                    if (account.ExtraUserName != null)
                    {
                        var extra = account.ExtraUserName.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                        var randomIndexTwo = rand.Next(extra.Length);
                        randomNick = $"{extra[randomIndexTwo]}";
                    }


                    var embed = new EmbedBuilder();
                    embed.WithColor(Color.Purple);
                    embed.WithAuthor(user);
                    embed.WithFooter("lil octo notebook");
                    if (randomNick != null) embed.AddField("Was seen under the nickname: ", " " + randomNick);

                    if (httpsCheck == "https")
                        embed.WithImageUrl($"{randomFukt}");
                    else
                        embed.AddField("Random fact: ", " " + randomFukt);


                    await SendMessAsync( embed);
                }
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **fact [user_ping(or user ID)] [index]**(show [index] fact about user)");
            }
        }


        [Command("ВсеФакты")]
        [Alias("Все Факты", "allfact", "allfacts", "all fact", "all facts")]
        public async Task DeleteTheFucktUser()
        {
            try
            {
                var account = _accounts.GetAccount(Context.User);
                if (account.OctoPass >= 3)
                {
                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);


                    var mess = "";
                    for (var i = 0; i < fuckts.Length; i++) mess += $"index: {i} | {fuckts[i]}\n";

                    var embed = new EmbedBuilder();
                    embed.WithFooter("lil octo notebook");
                    embed.WithTitle("All the facts about you:");
                    embed.WithDescription($"{mess}\n**del [index]** to delete the fact");

                    await SendMessAsync( embed);
                }
                else


                {
                    await SendMessAsync(
                        "Boole :< You do not have 3rd level tolerance");
                }
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **allfacts [user_ping(or user ID)]**(show all of your facts)\n");
            }
        }


        [Command("ВсеФакты")]
        [Alias("Все Факты", "allfact", "allfacts", "all fact", "all facts")]
        public async Task DeleteTheFuckt(IGuildUser user)
        {
            try
            {
                var account = _accounts.GetAccount((SocketUser) user);
                var comander = _accounts.GetAccount(Context.User);
                if (comander.OctoPass >= 4 || ((IGuildUser) Context.User).GuildPermissions.ManageMessages)
                {
                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                    var mess = "";
                    for (var i = 0; i < fuckts.Length; i++) mess += $"index: {i} | {fuckts[i]}\n";

                    var embed = new EmbedBuilder();
                    embed.WithFooter("lil octo notebook");
                    embed.WithTitle("All the facts about you:");
                    embed.WithDescription($"{mess}\n**del [index]** to delete the fact");

                    await SendMessAsync( embed);
                }
                else

                {
                    await SendMessAsync(
                        "Boole :< You do not have 4rd level tolerance");
                }
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **allfacts [user_ping(or user ID)]**(show all facts about user)\n" +
             //       "Alias: allfact, all facts, ВсеФакты, Все Факты ");
            }
        }


        [Command("УдалитьФакт")]
        [Alias("Удалить Факт", "del")]
        public async Task DeleteTheFucktUser(int index)
        {
            try
            {
                var account = _accounts.GetAccount(Context.User);
                if (account.OctoPass >= 2)
                {
                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    account.Fuckt = null;

                    for (var i = 0; i < fuckts.Length; i++)
                        if (i != index)
                            account.Fuckt += $"{fuckts[i]}|";

                    


                    await SendMessAsync(
                        $"fact under index {index} was removed from the lil octo notebook ;c");
                }
                else

                {
                    await SendMessAsync(
                        "Boole :< You do not have 3rd level tolerance");
                }
            }
            catch
            {
            //    await ReplyAsync(
            //        "boo... An error just appear >_< \nTry to use this command properly: **del [index]**(delete [index] fact)\n" +
            //        "Alias: УдалитьФакт");
            }
        }


        [Command("УдалитьФакт")]
        [Alias("Удалить Факт", "del")]
        public async Task DeleteTheFuckt(IGuildUser user, int index)
        {
            try
            {
                var account = _accounts.GetAccount((SocketUser) user);
                var comander = _accounts.GetAccount(Context.User);

                
                if (comander.OctoPass >= 100 || ((IGuildUser) Context.User).GuildPermissions.ManageMessages)
                {
                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    account.Fuckt = null;

                    for (var i = 0; i < fuckts.Length; i++)
                        if (i != index)
                            account.Fuckt += $"{fuckts[i]}|";

                    


                    await SendMessAsync(
                        $"fact under index {index} was removed from the lil octo notebook ;c");
                }
                else

                {
                    await SendMessAsync(
                        "Boole :< You do not have 10th level tolerance");
                }
            }
            catch
            {
             //   await ReplyAsync(
             //       "boo... An error just appear >_< \nTry to use this command properly: **del [user_ping(or user ID)] [index]**(delete [index] fact of the user)\n" +
             //       "Alias: УдалитьФакт");
            }
        }
    }
}