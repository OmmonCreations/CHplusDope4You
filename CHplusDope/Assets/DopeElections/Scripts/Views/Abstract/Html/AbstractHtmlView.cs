using System;
using System.Collections;
using AnimatedObjects;
using DopeElections.Localizations;
using Html;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections
{
    public abstract class AbstractHtmlView : DopeElectionsView
    {
        [SerializeField] private HtmlViewComponents _components = null;

        protected abstract LocalizationKey ConfirmKey { get; }
        protected HtmlViewComponents Components => _components;

        protected Button CloseButton => Components.CloseButton;
        protected Button CloseBackground => Components.CloseBackground;
        protected HtmlCanvas HtmlCanvas => Components.HtmlCanvas;
        protected LocalizedText ConfirmText => Components.ConfirmText;
        protected ScrollRect ScrollRect => Components.ScrollRect;
        protected ToggleableObjectController AnimationController => Components.AnimationController;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CloseButton.onClick.AddListener(Close);
            CloseBackground.onClick.AddListener(Close);
            ConfirmText.key = ConfirmKey;
            AnimationController.HideImmediate();
        }

        private void OnEnable()
        {
            Setting.Language.LanguageChanged += OnLanguageChanged;
            UpdateHtml();
        }

        private void OnDisable()
        {
            Setting.Language.LanguageChanged -= OnLanguageChanged;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            AnimationController.Show();
            StartCoroutine(SetScrollPosition()); // yikes but oh well
            ScrollRect.verticalNormalizedPosition = 1;
        }

        protected override StateChangePromise PrepareClose()
        {
            var result = new StateChangePromise();
            AnimationController.Hide().Then(result.Fulfill);
            return result;
        }

        // Yes I know this is a Coroutine and it is super ugly but in this special case more convenient than doing
        // the same in another way. The error potential is small in this particular case since it only runs for
        // one frame.
        private IEnumerator SetScrollPosition()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollRect.content);
            Canvas.ForceUpdateCanvases();
            ScrollRect.verticalNormalizedPosition = 1;
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateHtml();
        }

        private void UpdateHtml()
        {
            var app = DopeElectionsApp.Instance;
            var languageCode = app.Settings.GetValue(Setting.Language);
            var file = GetHtmlFile(languageCode);
            app.InternalStorage.ReadAllText(file, result => HtmlCanvas.Html = result);
        }

        protected abstract string GetHtmlFile(string languageCode);
    }
}