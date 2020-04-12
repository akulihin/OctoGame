using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctoGame.DiscordFramework.Extensions;
using OctoGame.LocalPersistentData.ServerAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoBot
{
    public class TextToEmojiCommands : ModuleBaseCustom
    {

        private readonly UserAccounts _accounts;
        private readonly ServerAccounts _serverAccounts;

        public TextToEmojiCommands(UserAccounts accounts, ServerAccounts serverAccounts)
        {
            _accounts = accounts;
            _serverAccounts = serverAccounts;
        }

        [Command("Emotify")]
        [Alias("emoji", "emotion", "emo")]
        [Summary("Convert text to Emojis")]
        public async Task Emotify([Remainder] string args)
        {
            string[] convertorArray = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};
            var pattern = new Regex("^[a-zA-Z]*$", RegexOptions.Compiled);
            args = args.ToLower();

            var convertedText = "";
            foreach (var c in args)
                switch (c.ToString())
                {
                    case "\\":
                        convertedText += "\\";
                        break;
                    case "\n":
                        convertedText += "\n";
                        break;
                    default:
                        if (pattern.IsMatch(c.ToString()))
                            convertedText += $":regional_indicator_{c}:";
                        else if (char.IsDigit(c)) convertedText += $":{convertorArray[(int) char.GetNumericValue(c)]}:";
                        else convertedText += $"{c}";
                        break;
                }
            await SendMessAsync(
                $"{convertedText}");
        }

        //var mumu = Emote.Parse("<:mumu:445277916872310785>");
        //await socketMsg.AddReactionAsync(mumu); 
        [Command("EmoteSay")]
        [Alias("emojiM", "emotionM", "emoM")]
        [Summary("Add this text to this message as Emojis")]
        public async Task EmoteSay(ulong chanelId, ulong messId, [Remainder] string args)
        {
            string[] numArray = {"0⃣", "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣"};
            string[] letterArray =
            {
                "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮", "🇯", "🇰", "🇱", "🇲", "🇳", "🇴", "🇵", "🇶",
                "🇷", "🇸", "🇹", "🇺", "🇻", "🇼", "🇽", "🇾", "🇿"
            };

            var patternLett = new Regex("^[a-z]*$", RegexOptions.Compiled);
            var patternNum = new Regex("^[0-9]*$", RegexOptions.Compiled);
            args = args.ToLower();
            var charArray = args.ToCharArray();
            var socketMsg = Context.Guild.GetTextChannel(chanelId).GetCachedMessage(messId) as SocketUserMessage;

            for (var i = 0; i < charArray.Length; i++)
                if (patternLett.IsMatch(charArray[i].ToString()))
                {
                    var letter = Convert.ToInt32(charArray[i]) % 32 - 1;
                    var emo = new Emoji($"{letterArray[letter]}");
                    if (socketMsg != null) await socketMsg.AddReactionAsync(emo);
                }
                else if (patternNum.IsMatch(charArray[i].ToString()))
                {
                    var emo = new Emoji($"{numArray[(int) char.GetNumericValue(charArray[i])]}");
                    if (socketMsg != null) await socketMsg.AddReactionAsync(emo);
                }
        }
    }
}