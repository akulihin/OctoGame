using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OctoGame.Helpers
{
      public sealed class  SecureRandom : IServiceTransient
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

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
}