using System;
using System.Collections;
using AnimatedObjects;
using DopeElections.Localizations;
using Essentials;
using Html;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Startup
{
    public class EulaView : StartupView, IView<Action<bool>>
    {
        public override NamespacedKey Id => StartupViewId.UserConsent;

        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private Button _rejectButton = null;
        [SerializeField] private ToggleableObjectController _animationController = null;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private LocalizedText _rejectText = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private GameObject _confirmArea = null;

        private Action<bool> Callback { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _confirmButton.onClick.AddListener(Confirm);
            _rejectButton.onClick.AddListener(Reject);
            _animationController.HideImmediate();
            _confirmText.key = LKey.Views.UserConsent.Confirm;
            _rejectText.key = LKey.Views.UserConsent.Reject;
            _scrollRect.onValueChanged.AddListener(OnScroll);
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

        public void Open(Action<bool> data)
        {
            Callback = data;
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            _animationController.Show();
            _confirmArea.SetActive(false);
            StartCoroutine(SetScrollPosition());
        }

        protected override StateChangePromise PrepareClose()
        {
            var result = new StateChangePromise();
            _animationController.Hide().Then(result.Fulfill);
            return result;
        }

        private void Confirm()
        {
            if (Callback != null) Callback(true);
            else Close();
        }

        private void Reject()
        {
            if (Callback != null) Callback(false);
            else Close();
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateHtml();
        }

        private void OnScroll(Vector2 position)
        {
            if(position.y <= 0.001f) _confirmArea.SetActive(true);
        }

        private void UpdateHtml()
        {
            if(!this) return;
            var app = DopeElectionsApp.Instance;
            var languageCode = app.Settings.GetValue(Setting.Language);
            var file = $"texts/{languageCode}/text-eula-{languageCode}.html";
            app.InternalStorage.ReadAllText(file, result => _htmlCanvas.Html = result);
        }

        private IEnumerator SetScrollPosition()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}