using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame.OctoGame.GamePlayFramework
{
  public  class UpdateFightPage
    {
        private readonly OctoGameUpdateMess _octoGameUpdateMess;
        private readonly Global _global;

        public UpdateFightPage(OctoGameUpdateMess octoGameUpdateMess, Global global)
        {
            _octoGameUpdateMess = octoGameUpdateMess;
            _global = global;
        }

        public async Task UpdateMainPageForAllPlayers(AccountSettings account)
        {
            foreach (var u in _global.OctopusGameMessIdList[account.MessageIdInList])
            {
                if(u.PlayerDiscordAccount != null && u.GamingWindowFromBot != null)
                    await _octoGameUpdateMess.MainPage(u.PlayerDiscordAccount.Id, u.GamingWindowFromBot);
            }
        }

        public async Task UpdateIfWinOrContinue(int status, ulong userId, int i)
        {
            if (status == 1)
                foreach (var v in _global.OctopusGameMessIdList[i])
                {
                      if (v.PlayerDiscordAccount.Id == userId)
                      await _octoGameUpdateMess.VictoryPage(v.PlayerDiscordAccount.Id,
                        v.GamingWindowFromBot);
                }

        }

    }
}
