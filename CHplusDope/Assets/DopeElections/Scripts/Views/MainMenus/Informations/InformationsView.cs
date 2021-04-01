using AnimatedObjects;
using DopeElections.Localizations;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.MainMenus
{
    public class InformationsView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.Informations;

        [SerializeField] private Button _backButton = null;
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private InformationsSubviews _viewsContainer = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        public InformationsSubviews Subviews => _viewsContainer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _titleText.key = LKey.Views.Informations.Title;
            _backButton.onClick.AddListener(Back);
            _viewsContainer.Initialize();
            _animationController.HideImmediate();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            // Views.Background.Show();

            var subviews = Subviews;
            if (!subviews.HowToPlayView.IsOpen && !subviews.CreditsView.IsOpen) subviews.HowToPlayView.Open();

            _animationController.Show();
        }

        protected override StateChangePromise PrepareClose()
        {
            var result = new StateChangePromise();
            _animationController.Hide().Then(result.Fulfill);
            return result;
        }

        private void Back()
        {
            Views.OverviewView.Open();
        }
    }
}