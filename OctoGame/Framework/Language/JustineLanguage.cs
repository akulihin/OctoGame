using System.Collections.Generic;

namespace OctoGame.Framework.Language
{
    public class JustineLanguage
    {
        public uint LanguageId { get; set; }
        public string LanguageName { get; set; }
        public Dictionary<string, string> Resources { get; set; }
        public Dictionary<string, List<string>> ResourcePools { get; set; }
    }
}
