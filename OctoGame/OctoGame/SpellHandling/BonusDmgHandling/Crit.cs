using System;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.BonusDmgHandling
{
   public class Crit
    {
        public double CritHandling(double agi, double dmg, AccountSettings account)
        {
            var rand = new Random();
            var randCrit = rand.Next(100);

            if (agi >= randCrit + 1)
            {
                dmg = dmg * account.CritDmg;
                return dmg;
            }

            return dmg;
        }
    }
}
