using System.Collections.Generic;
using System.Linq;


namespace OctoGame.LocalPersistentData.GameSpellsAccounts
{
    public sealed class SpellUserAccounts : ISpellAccounts
    {


        private  readonly List<SpellSetting> _accounts;
        private readonly SpellDataStorage _spellDataStorage;


        private  readonly string _accountsFile = @"OctoGameDataBase/SpellBook.json";

         public SpellUserAccounts(SpellDataStorage spellDataStorage)
         {
             _spellDataStorage = spellDataStorage;
             if (SpellDataStorage.SaveExists(_accountsFile))
            {
                _accounts = _spellDataStorage.LoadAccountSettings(_accountsFile).ToList();
            }
            else
            {
                _accounts = new List<SpellSetting>();
                SaveAccounts();
            }
         }

        public  void SaveAccounts()
        {
            _spellDataStorage.SaveAccountSettings(_accounts, _accountsFile);
        }

        public  SpellSetting GetAccount(ulong spellId)
        {
            return GetOrCreateAccount(spellId);
        }

        public  SpellSetting GetOrCreateAccount(ulong spellId)
        {
            var result = from a in _accounts
                where a.SpellId == spellId
                select a;


            var account = result.FirstOrDefault();
            if (account == null)
                account = CreateUserAccount(spellId);


            return account;
        }

        public  SpellSetting CreateUserAccount(ulong id)
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