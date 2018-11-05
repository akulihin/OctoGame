using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OctoGame.Framework.Language
{
 public class JsonLocalization : ILocalization
    {
        private readonly IDataStorage _dataStorage;
        private List<JustineLanguage> _languages = new List<JustineLanguage>();

        public JsonLocalization(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            LoadLanguages();
        }

        public void LoadLanguages()
        {
            _languages = _dataStorage.GetLanguages().ToList();
        }

        /// <summary>
        /// Gets a language resource by its resource key and language ID.
        /// </summary>
        /// <param name="key">Language resource key</param>
        /// <param name="languageId">Language ID</param>
        /// <returns>The string value of a language resource.</returns>
        public string GetResource(string key, int languageId = 0)
        {
            var language = _languages.FirstOrDefault(l => l.LanguageId == languageId);
            if (language is null) throw new Exception($"Id '{languageId}' not found.");
            if (!language.Resources.ContainsKey(key)) throw new Exception($"Resource with key '{key}' wasn't found.");
            return language.Resources[key];
        }

        public List<string> GetResourcePool(string key, int languageId = 0)
        {
            var language = _languages.FirstOrDefault(l => l.LanguageId == languageId);
            if (language is null) throw new Exception($"Id '{languageId}' not found.");
            if (!language.ResourcePools.ContainsKey(key)) throw new Exception($"ResourcePool with key '{key}' wasn't found.");
            return language.ResourcePools[key];
        }

        /// <summary>
        /// Returns a string with all language resource keys that are enclosed with [] replaced by their values.
        /// e.g.
        /// "A [STRING_TEST] B" => "A Hello World! B"
        /// Invalid resource keys stay untouched.
        /// </summary>
        /// <param name="template">The string containing language resource keys enclosed in square brackets. [MY_KEY]</param>
        /// <param name="languageId">The ID of a language to be used. (Defaults to 0)</param>
        /// <returns>String with language resource key replaced with their values.</returns>
        public string Resolve(string template, int languageId = 0)
        {
            var regex = new Regex(@"\[([A-Z_@\(\)]*?)\]", RegexOptions.IgnoreCase);
            var matches = regex.Matches(template);
            
            foreach (Match m in matches)
            {
                template = template.Replace(m.Groups[0].Value, GetResourceSafe(m.Groups[1].Value, languageId));
            }

            return template;
        }

        public string FromTemplate(string key, int languageId = 0, params object[] objects)
        {
            var template = GetResource(key, languageId);
            return string.Format(template, objects);
        }

        private string GetResourceSafe(string key, int languageId = 0)
        {
            try
            {
                return GetResource(key, languageId);
            }
            catch (Exception)
            {
                return $"[{key}]";
            }
        }

        public string GetPooledResource(string key, int languageId = 0)
        {
            var pool = GetResourcePool(key, languageId);
            if(!pool.Any()) return string.Empty;
            return GetRandomElement(pool);
        }
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public static T GetRandomElement<T>(List<T> list)
        {
            return list[Random.Next(0, list.Count)];
        }

        public string FromPooledTemplate(string key, int languageId = 0, params object[] objects)
        {
            throw new NotImplementedException();
        }

        private List<string> GetResourcePoolSafe(string key, int languageId = 0)
        {
            try
            {
                return GetResourcePool(key, languageId);
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
