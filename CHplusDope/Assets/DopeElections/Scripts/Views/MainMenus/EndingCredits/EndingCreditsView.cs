using System;
using System.Collections;
using AnimatedObjects;
using DopeElections.Localizations;
using Essentials;
using Html;
using Localizator;
using MobileInputs;
using StateMachines;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.MainMenus
{
    public class EndingCreditsView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.EndingCredits;

        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _contentArea = null;
        [SerializeField] private float _speed = 100;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private ToggleableObjectController _bodyAnimationController = null;
        [SerializeField] private ToggleableObjectController _continueButtonAnimationController = null;
        [SerializeField] private Button _continueButton = null;
        [SerializeField] private LocalizedText _continueText = null;

        [Header("Scene References")] [SerializeField]
        private InteractionSystem _interactionSystem = null;

        private StateMachine StateMachine { get; set; }

        internal ScrollRect ScrollRect => _scrollRect;
        internal RectTransform ContentArea => _contentArea;
        internal InteractionSystem InteractionSystem => _interactionSystem;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _continueText.key = LKey.Views.EndingCredits.Continue;
            _continueButton.onClick.AddListener(Continue);
            StateMachine = gameObject.AddComponent<StateMachine>();
            _bodyAnimationController.HideImmediate();
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

        private void Update()
        {
            if (StateMachine) StateMachine.Run();
        }

        protected override StateChangePromise PrepareOpen()
        {
            _continueButtonAnimationController.HideImmediate();
            return base.PrepareOpen();
        }

        public new void Open()
        {
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Views.BlackMask.FadeToClear();
            gameObject.SetActive(true);
            StartCoroutine(ResetScrollPosition());
            _bodyAnimationController.Show().Then(() => StartAutoScroll().Then(ShowActions));
        }

        protected override StateChangePromise PrepareClose()
        {
            var promise = new StateChangePromise();
            _bodyAnimationController.Hide().Then(promise.Fulfill);
            return promise;
        }

        private void Continue()
        {
            Views.FinalView.Open();
        }

        private State StartAutoScroll()
        {
            var autoScroll = new AutoScrollState(this, _speed);
            StateMachine.State = autoScroll;
            return autoScroll;
        }

        private void ShowActions()
        {
            _continueButtonAnimationController.Show();
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateHtml();
        }

        private void UpdateHtml()
        {
            var app = DopeElectionsApp.Instance;
            var languageCode = app.Settings.GetValue(Setting.Language);
            var file = $"texts/{languageCode}/text-endingCredits-{languageCode}.html";
            app.InternalStorage.ReadAllText(file, result => _htmlCanvas.Html = result);
        }

        private IEnumerator ResetScrollPosition()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}