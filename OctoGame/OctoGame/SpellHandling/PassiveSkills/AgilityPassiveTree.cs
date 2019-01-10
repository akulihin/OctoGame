using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;

namespace OctoGame.OctoGame.SpellHandling.PassiveSkills
{
   public sealed class AgilityPassiveTree : IService
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        public void AgiPassiveSkills(ulong skillId, AccountSettings account, AccountSettings enemy)
        {

        }

    }
}
