using System.Collections.Generic;
using System.Threading.Tasks;

namespace OctoGame.Helpers
{
    public sealed class CustomCalculator : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly SecureRandom _secureRandom;

        public CustomCalculator(SecureRandom secureRandom)
        {
            _secureRandom = secureRandom;
        }

        //Calculator for Dice Roll, Can handle (2d4 + 4d2 + 100 + 1d4) ort simple A*B or A/B
        public (int, string ) Calculator(int times, int num, char sign)
        {
            switch (sign)
            {
                case 'm':
                    break;
                case 'l':
                    break;
                case '+':
                    return (times + num, "");
                case '-':
                    return (times - num, "");
                case '*':
                    return (times * num, "");
                case '/':
                    return (times / num, "");
                case 'd':
                    var result = new List<int>();

                    var answer = 0;

                    if (times <= 0 || num <= 0)
                        return (0, "error");
                    if (times == 1)
                        return (_secureRandom.Random(1, num), "");

                    var resultString = "";
                    if (times <= 1000 && num <= 120)
                    {
                        for (var i = 0; i < times; i++)
                        {
                            var lol = _secureRandom.Random(1, num);
                            result.Add(lol);
                        }

                        for (var i = 0; i < result.Count; i++)
                        {
                            answer += result[i];
                            resultString += $"{result[i]}";
                            if (i < result.Count - 1)
                                resultString += $" + ";
                        }
                    }

                    return (answer, resultString);
            }

            return (0, "error");
        }
    }
}