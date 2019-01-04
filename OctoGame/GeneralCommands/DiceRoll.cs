using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OctoGame.DiscordFramework.CustomLibrary;
using OctoGame.Helpers;

namespace OctoGame.GeneralCommands
{
    public class DiceRollCommands : ModuleBaseCustom
    {
        private readonly SecureRandom _secureRandom;
        private readonly CustomCalculator _customCalculator;

        public DiceRollCommands(SecureRandom secureRandom, CustomCalculator customCalculator)
        {
            _secureRandom = secureRandom;
            _customCalculator = customCalculator;
        }

        [Command("roll")]
        [Alias("Роллл", "Ролл")]
        [Summary("Rolling a dice multiple times")]
        public async Task Roll(int number, int times)
        {
            try
            {
                var mess = "";
                if (times > 100)
                {
                    await SendMessAsync(
                        "Boole! We are not going to roll that many times!");


                    return;
                }

                if (number > 999999999)
                {
                    await SendMessAsync(
                        "Boole! This numbers is way too big for us :c");


                    return;
                }

                for (var i = 0; i < times; i++)
                {
                    var randomIndexRoll = _secureRandom.Random(1, number);
                    mess += $"It's a {randomIndexRoll}!\n";
                }

                var embed = new EmbedBuilder();
                embed.WithFooter("lil octo notebook");
                embed.WithTitle($"Roll {times} times:");
                embed.WithDescription($"{mess}");

                await SendMessAsync( embed);
            }
            catch
            {
                //   await ReplyAsync(
                //       "boo... An error just appear >_< \nTry to use this command properly: **roll [times] [max_value_of_roll]**\n" +
                //       "Alias: Роллл, Ролл");
            }
        }

        [Command("roll")]
        [Alias("Роллл", "Ролл")]
        [Summary("Rolling a dice 1 time")]
        public async Task Roll(int number)
        {
            try
            {
                var randomIndexRoll = _secureRandom.Random(1, number);

                await SendMessAsync( $"It's a {randomIndexRoll}!");
            }
            catch
            {
                //  await ReplyAsync(
                //      "boo... An error just appear >_< \nTry to use this command properly: **roll [max_value_of_roll]**\n" +
                //      "Alias: Роллл, Ролл");
            }
        }

        [Command("roll")]
        [Summary("Rolling a dice like `1d4` or `2d4 + 3d2` etc.")]
        public async Task CalculateStuf([Remainder] string textFormula)
        {
            var low = textFormula.ToLower();
            low = low.Replace(" ", string.Empty);

            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);

            embed.WithFooter("lil octo notebook");
            var results = "steps:\n";
            var answer = 0;
            var numberString = "";
            var numberString2 = "";
            var sign = 'k';
            var check = 0;
            var doi = 0;
            var count = 0;
            var boole = 0;
            var isSuccess = 0;

            for (var i = 0; i < low.Length; i++)
            {
                isSuccess++;
                if (isSuccess >= 200)
                {
                    await ReplyAsync("буль!!");
                    return;
                }

                string reminder;
                if (low[0] == '-')
                {
                    Console.WriteLine($"hello== {low}");
                    //-79+3d1+7d1+99
                    reminder = "";
                    var kek = "";

                    for (var m = 1; m < low.Length; m++)
                    {
                        if (boole == 0)
                        {
                            if (!int.TryParse($"{low[m]}", out _))
                            {
                                boole = 1;
                                m++;
                            }
                            else
                            {
                                kek += $"{low[m]}";
                            }
                        }

                        if (boole == 1) reminder += $"{low[m]}";
                    }

                    low = reminder + $"-{kek}";
                    // Console.WriteLine($"bye== {low}");
                    i = -1;
                    answer = 0;
                    numberString = "";
                    numberString2 = "";
                    sign = 'k';
                    check = 0;
                    doi = 0;
                    count = 0;
                    boole = 0;
                }
                else
                {
                    if (int.TryParse($"{low[i]}", out _) || i == low.Length - 1 || i == low.Length || low[0] == '-')
                    {
                        switch (check)
                        {
                            case 0:
                                if (low[0] == '-')
                                    numberString += "-";
                                numberString += low[i];
                                break;
                            case 1:
                                numberString2 += low[i];
                                break;
                        }

                        if (check != 2 && i != low.Length - 1) continue;

                        if (doi < 2)
                        {
                            int.TryParse(numberString, out var num1);
                            int.TryParse(numberString2, out var num2);

                            var result = _customCalculator.Calculator(num1, num2, sign);
                            answer = result.Item1;

                            if (i == low.Length - 1 && check == 1)
                            {
                                if (result.Item2.Length > 0)
                                    embed.AddField($"It's a {answer}!", $"{results} **{result.Item2}**");
                                else
                                    embed.AddField($"It's a {answer}!", $"{results}");


                                await SendMessAsync( embed);

                                return;
                            }


                            reminder = $"{answer}{low[i - 1]}";

                            for (var k = i; k < low.Length; k++) reminder += $"{low[k]}";

                            low = reminder;

                            count++;
                            if (result.Item2.Length > 0)
                                results += $"{count})**|({result.Item2})|** {reminder} \n";
                            else
                                results += $"{count}) {reminder} \n";


                            i = -1;
                            numberString = "";
                            numberString2 = "";
                            check = 0;
                            sign = 'k';
                        }
                        else
                        {
                            reminder = "";


                            for (var k = answer.ToString().Length + 1; k < low.Length; k++)
                                reminder += $"{low[k]}";

                            reminder += $"{low[answer.ToString().Length]}{answer}";

                            low = reminder;

                            i = -1;
                            numberString = "";
                            numberString2 = "";
                            check = 0;
                            sign = 'k';
                            doi = 0;
                        }
                    }
                    else if (low[i] == 'd')
                    {
                        check++;
                        doi++;
                        if (check <= 1)
                            sign = 'd';
                    }
                    else if (low[i] == '+')
                    {
                        check++;
                        if (check <= 1)
                            sign = '+';
                    }
                    else if (low[i] == '-')
                    {
                        check++;
                        if (check <= 1)
                            sign = '-';
                    }
                    else if (low[i] == '*')
                    {
                        check++;
                        if (check <= 1)
                            sign = '*';
                    }
                    else if (low[i] == '/')
                    {
                        check++;
                        if (check <= 1)
                            sign = '/';
                    }
                    else
                    {
                        check++;
                    }
                }
            }

            await SendMessAsync( $"It's a {answer}!");
        }
    }
}