using AnimatedObjects;
using DopeElections.Localizations;
using DopeElections.MainMenus;
using DopeElections.Sounds;
using Essentials;
using FMODSoundInterface;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Progress.Congratulations
{
    public class CongratulationsView : ProgressView, IView
    {
        public override NamespacedKey Id => ProgressViewId.Congratulations;

        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _messageText = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _titleText.key = LKey.Views.Congratulations.Title;
            _messageText.key = LKey.Views.Congratulations.RaceRoyaleComplete;
            _confirmText.key = LKey.Views.Congratulations.Confirm;

            _confirmButton.onClick.AddListener(Continue);
            _animationController.HideImmediate();
        }

        public new void Open()
        {
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Views.BlackMask.BlockInteractions(false);
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();
            _animationController.Show();
            SoundController.Play(Sound.Sfx.Special.Congratulations);
        }

        private void Continue()
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() =>
            {
                DopeElectionsRouter.GoToMainMenu(MainMenuViewId.EndingCinematic);
            });
        }
    }
}