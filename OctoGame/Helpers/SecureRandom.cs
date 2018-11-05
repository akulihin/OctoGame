using System;
using System.Security.Cryptography;

namespace OctoGame.Helpers
{
      public class  SecureRandom
    {
        private readonly RNGCryptoServiceProvider _csp;

        public  SecureRandom()
        {
            _csp = new RNGCryptoServiceProvider();
        }

        public int Random(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");
            maxExclusiveValue += 1;
            var diff = (long)maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            var buffer = new byte[bytesNumber];
            _csp.GetBytes(buffer);
            return buffer;
        }
    }

    public class SecureRandomStatic
    {
        private static readonly RNGCryptoServiceProvider Generator = new RNGCryptoServiceProvider();

        public static int Random(int min, int max)
        {

            var randomNumber = new byte[8];
            Generator.GetBytes(randomNumber);
            var asc2ConvertBytes = Convert.ToDouble(randomNumber[0]);

            var multy = Math.Max(0, asc2ConvertBytes / 255d);
            var range = max - min + 1;
            var randomInRange = Math.Floor(multy * range);

            if (randomInRange > max)
                randomInRange = max;

            return (int) (min + randomInRange);
        }

        public static int Random(int max)
        {
            var randomNumber = new byte[1];

            Generator.GetBytes(randomNumber);

            var asc2ConvertBytes = Convert.ToDouble(randomNumber[0]);

            var multy = Math.Max(0, asc2ConvertBytes / 255d);
            var range = max - 0 + 1;
            var randomInRange = Math.Floor(multy * range);

            var returnValue = (int) (0 + randomInRange);
            if (returnValue >= max)
                returnValue = max;
            return returnValue;
        }
    }
}