﻿using System;
using DopeElections.Localizations;
using Essentials;
using Html;
using Localizator;
using UnityEngine;
using Views;

namespace DopeElections.MainMenus.Subviews.OurSystem
{
    public class OurSystemView : DopeElectionsView, IView
    {
        public override NamespacedKey Id => MainMenuViewId.OurSystem;

        [SerializeField] private LocalizedText _headerText = null;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _headerText.key = LKey.Views.OurSystem.Title;
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

        public new void Open()
        {
            base.Open();
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateHtml();
        }

        private void UpdateHtml()
        {
            var app = DopeElectionsApp.Instance;
            var languageCode = app.Settings.GetValue(Setting.Language);
            var file = $"texts/{languageCode}/text-ourSystem-{languageCode}.html";
            app.InternalStorage.ReadAllText(file, result => _htmlCanvas.Html = result);
        }
    }
}