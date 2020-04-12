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

        [Command("write")]
        [Alias("write fact", "факт", "записать", "write down", "fact")]
        [Summary("Write a fun fact about specified user")]
        public async Task WriteFuckt(IGuildUser user, [Remainder] string message)
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

        [Command("fact")]
        [Alias("факт")]
        [Summary("Show a random fact about this user, if any")]
        public async Task ReadFuckt(SocketUser user)
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

        [Command("факт")]
        [Alias("fact")]
        [Summary("Show specific fact about this user, if any")]
        public async Task ReadFucktIndex(SocketUser user, int index)
        {

                var account = _accounts.GetAccount(user);
                var comander = _accounts.GetAccount(Context.User);

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


        [Command("ВсеФакты")]
        [Alias("Все Факты", "allfact", "allfacts", "all fact", "all facts")]
        [Summary("Show all facts about this user, if any")]
        public async Task ShowAllFactsAboutUser()
        {

                var account = _accounts.GetAccount(Context.User);

                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);


                    var mess = "";
                    for (var i = 0; i < fuckts.Length; i++) mess += $"index: {i} | {fuckts[i]}\n";

                    var embed = new EmbedBuilder();
                    embed.WithFooter("lil octo notebook");
                    embed.WithTitle("All the facts about you:");
                    embed.WithDescription($"{mess}\n**del [index]** to delete the fact");

                    await SendMessAsync( embed);

        }





        [Command("DeleteFact")]
        [Alias("Удалить Факт", "del", "УдалитьФакт")]
        [Summary("Deletes your specific fact fact")]
        public async Task DeleteTheFucktUser(int index)
        {

            var account = _accounts.GetAccount(Context.User);
                var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    account.Fuckt = null;

                    for (var i = 0; i < fuckts.Length; i++)
                        if (i != index)
                            account.Fuckt += $"{fuckts[i]}|";

                    


                    await SendMessAsync(
                        $"fact under index {index} was removed from the lil octo notebook ;c");
            }


        [Command("УдалитьФакт")]
        [Alias("Удалить Факт", "del")]
        [Summary("Deletes someone's specific fact fact")]
        public async Task DeleteTheFuckt(IGuildUser user, int index)
        {

                var account = _accounts.GetAccount((SocketUser) user);
                var comander = _accounts.GetAccount(Context.User);

                

                    var fuckts = account.Fuckt.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    account.Fuckt = null;

                    for (var i = 0; i < fuckts.Length; i++)
                        if (i != index)
                            account.Fuckt += $"{fuckts[i]}|";

                    


                    await SendMessAsync(
                        $"fact under index {index} was removed from the lil octo notebook ;c");
                }
    }
}