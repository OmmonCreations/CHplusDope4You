using System;
using System.Linq;
using DopeElections.Accounts;
using DopeElections.Localizations;
using Essentials;
using Newtonsoft.Json.Linq;
using Popups;
using Progression;
using RSG;

namespace DopeElections.Progression.Questionnaire
{
    public class TranslationCheckEntry : ProgressEntry
    {
        private static readonly string[] MissingQuestionTexts = new[]
        {
            "Es sind keine Übersetzungen für diese Sprache verfügbar!",
            "Aucune traduction disponible pour cette langue!",
            "Nessuna traduzione disponibile in questa lingua!",
            "No translation in this language available!"
        };

        private string _language;
        private bool _checked = false;

        public string Language
        {
            get => _language;
            set => ApplyLanguage(value);
        }

        public TranslationCheckEntry(NamespacedKey id) : base(id)
        {
        }

        private void ApplyLanguage(string language)
        {
            if (_language == language) return;
            _language = language;
            _checked = false;
        }

        protected override void OnLoad(JToken data)
        {
            base.OnLoad(data);
            _language = data["l"] != null ? (string) data["l"] : null;
            _checked = data["check"] != null && (bool) data["check"];
        }

        protected override JToken SaveData()
        {
            if (_language == null) return null;
            var result = new JObject {["l"] = _language};
            if (_checked) result["check"] = true;
            return result;
        }

        /// <summary>
        /// Checks if all questions have a translation in the currently selected language
        /// </summary>
        /// <returns><code>true</code> if something changed; otherwise <code>false</code></returns>
        public IPromise<bool> PerformTranslationCheck()
        {
            var app = DopeElectionsApp.Instance;
            var user = app.User;
            var questionnaire = user.Questionnaire;

            Language = app.Settings.GetValue(Setting.Language);

            return new Promise<bool>((resolve, reject) =>
            {
                if (_checked)
                {
                    resolve(false);
                    return;
                }

                if (!questionnaire.Questions.Select(q => q.text).Intersect(MissingQuestionTexts).Any())
                {
                    _checked = true;
                    resolve(true);
                    return;
                }

                app.Popups.ShowPopup(
                    new ConfirmPopup(
                        LKey.Views.Startup.TranslationIncompleteAlert.Title,
                        LKey.Views.Startup.TranslationIncompleteAlert.Text,
                        LKey.Views.Startup.TranslationIncompleteAlert.ChangeRegion,
                        LKey.Views.Startup.TranslationIncompleteAlert.ChangeLanguage
                    ).Then(() =>
                    {
                        DopeElectionsRouter.GoToAccount(AccountViewId.LocationSelection);
                        // _checked = true;
                        // resolve(true);
                    }).Else(() => { DopeElectionsRouter.GoToAccount(AccountViewId.Settings); }));
            });
        }
    }
}