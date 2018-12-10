using System;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.DmgReductionHandling
{
    public class Dodge
    {
        private readonly IUserAccounts _accounts;

        public Dodge(IUserAccounts accounts)
        {
            _accounts = accounts;
        }

        public double DodgeHandling(double agi, double dmg, AccountSettings account, AccountSettings enemy)
        {
            var rand = new Random();
            var randDodge = rand.Next(100);
            if (agi > 1)
            {
                if (agi - 1 >= randDodge + 1)
                {
                    dmg = 0;
                    return dmg;
                }

                return dmg;
            }

            if (agi >= randDodge + 1)
            {
                dmg = 0;
                enemy.IsDodged = true;
                _accounts.SaveAccounts(enemy.DiscordId);
                return dmg;
            }

            return dmg;
        }
    }
}
