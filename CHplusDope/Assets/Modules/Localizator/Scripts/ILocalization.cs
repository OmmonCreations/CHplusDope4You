namespace Localizator
{
    public interface ILocalization
    {
        string GetString(LocalizationKey key);
        bool TryGetString(LocalizationKey key, out string value);
        void SetString(LocalizationKey key, string value);
    }
}