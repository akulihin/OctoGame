namespace OctoGame.Framework.Language
{
    public interface ILocalization
    {
        void LoadLanguages();

        string GetResource(string key, int languageId = 0);

        string GetPooledResource(string key, int languageId = 0);

        string Resolve(string template, int languageId = 0);

        string FromTemplate(string key, int languageId = 0, params object[] objects);

        string FromPooledTemplate(string key, int languageId = 0, params object[] objects);
    }
}
