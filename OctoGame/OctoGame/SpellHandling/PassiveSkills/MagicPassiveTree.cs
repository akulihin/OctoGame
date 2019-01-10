
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.PassiveSkills
{
   public sealed class MagicPassiveTree : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public void ApPassiveSkills(ulong skillId, AccountSettings account, AccountSettings enemy)
        {

        }
    }
}
