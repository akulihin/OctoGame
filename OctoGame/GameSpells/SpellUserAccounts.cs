using System.Collections.Generic;
using System.Linq;

namespace OctoBot.Games.OctoGame.GameSpells
{
    public static class SpellUserAccounts
    {
        
        private static List<SpellSetting> _accounts;

        
        private static string _accountsFile = @"OctoGameDataBase/SpellBook.json";

        static SpellUserAccounts()
        {
            if (SpellDataStorage.SaveExists(_accountsFile))
                _accounts = SpellDataStorage.LoadAccountSettings(_accountsFile).ToList();
            else
            {
                _accounts = new List<SpellSetting>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            SpellDataStorage.SaveAccountSettings(_accounts, _accountsFile);

        }

        public static SpellSetting GetAccount(ulong spellId)
        {
            return GetOrCreateAccount(spellId);
        }

        private static SpellSetting GetOrCreateAccount(ulong spellId)
        {
            var result = from a in _accounts
                where a.SpellId == spellId
                select a;


            var account = result.FirstOrDefault();
            if (account == null)
                account = CreateUserAccount(spellId);


            return account;
        }
        private static SpellSetting CreateUserAccount(ulong id)
        {
            var newAccount = new SpellSetting
            {
                // Username = "буль"б
                SpellId = id
            
            };

            _accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;

        }
    }
}
