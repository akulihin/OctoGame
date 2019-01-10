
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.ActiveSkills
{
   public sealed class MagicActiveTree : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        public double ApSkills(ulong skillId, AccountSettings myAccount, AccountSettings enemyAccount, bool check)
        {
            double dmg = 0;
            return dmg;
        }
    }
}
