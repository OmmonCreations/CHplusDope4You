using System.Linq;
using AppSettings;
using UnityEngine;

namespace Localizator.Settings
{
    public sealed class LanguageSetting : SettingType<string>
    {
        public delegate void LanguageChangedEvent(Language language);

        public event LanguageChangedEvent LanguageChanged = delegate { };

        public override string DefaultValue => EvaluateDefaultLanguage();
        public Language[] AvailableLanguages { get; }

        public LanguageSetting(string key, params Language[] available) : base(key)
        {
            AvailableLanguages = available;
        }

        protected override SettingValue<string> CreateValueInstance()
        {
            var result = new LanguageValue();
            result.ValueChanged += OnLanguageChanged;
            return result;
        }

        private void OnLanguageChanged(string code)
        {
            LanguageChanged(AvailableLanguages.FirstOrDefault(l => l.code == code));
        }

        private string EvaluateDefaultLanguage()
        {
            var systemLanguage = Application.systemLanguage;
            var exactLanguageMatch = AvailableLanguages.FirstOrDefault(l => l.systemLanguage == systemLanguage);
            if (exactLanguageMatch != default) return exactLanguageMatch.code;
            return AvailableLanguages.FirstOrDefault().code;
        }
    }
}