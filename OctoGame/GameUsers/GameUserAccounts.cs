using System.Collections.Generic;
using System.Linq;
using Discord;

namespace OctoBot.Games.OctoGame.GameUsers
{
    public static class GameUserAccounts
    {

        private static List<GameAccountSettings> _accounts;


        
        private static string _accountsFile = @"OctoGameDataBase/GameAccounts.json";

        static GameUserAccounts()
        {
            if (GameDataStorage.SaveExists(_accountsFile))
                _accounts = GameDataStorage.LoadAccountSettings(_accountsFile).ToList();
            else
            {
                _accounts = new List<GameAccountSettings>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            GameDataStorage.SaveAccountSettings(_accounts, _accountsFile);

        }

        public static GameAccountSettings GetAccount(IUser user)
        {
            return GetOrCreateAccount(user.Id, user.Username);
        }

        private static GameAccountSettings GetOrCreateAccount(ulong id, string name)
        {
            var result = from a in _accounts
                where a.Id == id
                select a;


            var account = result.FirstOrDefault();
            if (account == null)
                account = CreateUserAccount(id, name);


            return account;
        }
        private static GameAccountSettings CreateUserAccount(ulong id, string name)
        {
            var newAccount = new GameAccountSettings
            {
                // Username = "буль"б
                Id = id,
                UserName = name
            };

            _accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;

        }


    }
}
