using System;
using DopeElections.Localizations;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Splash
{
    public class SplashView : DopeElectionsView, IView<Action>
    {
        public override NamespacedKey Id => SplashViewId.Splash;

        [SerializeField] private Button _startGameButton = null;
        [SerializeField] private LocalizedText _startGameText = null;
        
        private Action Callback { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startGameText.key = LKey.Views.Splash.Start;
            _startGameButton.onClick.AddListener(StartGame);
        }

        public void Open(Action callback)
        {
            Callback = callback;
            base.Open();
        }

        private void StartGame()
        {
            Callback();
        }
    }
}