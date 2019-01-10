using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoGame.DiscordFramework;

namespace OctoGame.LocalPersistentData.GameSpellsAccounts
{
    public sealed class SpellDataStorage : IServiceSingleton
    {

        private readonly LoginFromConsole _log;

        public SpellDataStorage(LoginFromConsole log)
        {
            _log = log;
        }

        public async Task InitializeAsync()
            => await Task.CompletedTask;

        public void SaveAccountSettings(IEnumerable<SpellSetting> accounts, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                _log.Critical($"Failed to WRITE (Save Game Spells Account): {e.Message}");
             
            }
        }

        //Get AccountSettings

        public IEnumerable<SpellSetting> LoadAccountSettings(string filePath)
        {
            if (!File.Exists(filePath)) return null;
   
                var json = File.ReadAllText(filePath);

                return JsonConvert.DeserializeObject<List<SpellSetting>>(json);
  
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}