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

namespace DopeElections.MainMenus.Final
{
    public class FinalView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.Final;

        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _contentArea = null;
        [SerializeField] private float _speed = 100;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private Button _leaderboardButton = null;
        [SerializeField] private Button _donateButton = null;
        [SerializeField] private ToggleableObjectController _bodyAnimationController = null;
        [SerializeField] private ToggleableObjectController _leaderboardButtonAnimationController = null;
        [SerializeField] private ToggleableObjectController _donateButtonAnimationController = null;
        [SerializeField] private LocalizedText _leaderboardText = null;
        [SerializeField] private LocalizedText _donateText = null;

        [Header("Scene References")] [SerializeField]
        private InteractionSystem _interactionSystem = null;

        private StateMachine StateMachine { get; set; }
        
        internal ScrollRect ScrollRect => _scrollRect;
        internal RectTransform ContentArea => _contentArea;
        internal InteractionSystem InteractionSystem => _interactionSystem;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _leaderboardText.key = LKey.Views.Final.Leaderboard;
            _donateText.key = LKey.Views.Final.Donate;
            _leaderboardButton.onClick.AddListener(GoToLeaderboard);
            _donateButton.onClick.AddListener(GoToDonate);
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

        public new void Open()
        {
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _leaderboardButtonAnimationController.HideImmediate();
            _donateButtonAnimationController.HideImmediate();
            StartCoroutine(ResetScrollPosition());
            _bodyAnimationController.Show().Then(() => StartAutoScroll().Then(ShowActions));
        }

        protected override void OnClose()
        {
            base.OnClose();
            Views.EndingCinematicView.CinematicController.Environment.SetActive(false);
        }

        private State StartAutoScroll()
        {
            var autoScroll = new AutoScrollState(this, _speed);
            StateMachine.State = autoScroll;
            return autoScroll;
        }

        private void ShowActions()
        {
            _leaderboardButtonAnimationController.Show(0.15f);
            _donateButtonAnimationController.Show(0.2f);
        }

        private void GoToDonate()
        {
            Application.OpenURL("https://projektchplus.ch/");
        }

        private void GoToLeaderboard()
        {
            _bodyAnimationController.Hide().Then(() =>
            {
                Views.BlackMask.FadeToBlack(3).Then(()=>
                {
                    Close();
                    DopeElectionsRouter.GoToLeaderboard();
                });
            });
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateHtml();
        }

        private void UpdateHtml()
        {
            var app = DopeElectionsApp.Instance;
            var languageCode = app.Settings.GetValue(Setting.Language);
            var file = $"texts/{languageCode}/text-final-{languageCode}.html";
            app.InternalStorage.ReadAllText(file, result => _htmlCanvas.Html = result);
        }

        private IEnumerator ResetScrollPosition()
        {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}