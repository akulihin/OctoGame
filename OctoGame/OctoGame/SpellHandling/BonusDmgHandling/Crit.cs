using System;
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.BonusDmgHandling
{
   public sealed class Crit : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public double CritHandling(double agi, double dmg, AccountSettings account)
        {
            var rand = new Random();
            var randCrit = rand.Next(100);

            if (agi >= randCrit + 1)
            {
                dmg = dmg * account.CriticalDamage;
                return dmg;
            }

            return dmg;
        }
    }
}
