using System;
using AnimatedObjects;
using DopeElections.ExtraInfoTalkers;
using DopeElections.Localizations;
using DopeElections.Progression;
using Essentials;
using Localizator;
using Progression;
using SpeechBubbles;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Progress.ExtraInfos
{
    public class ExtraInfoView : ProgressView, IView<IExtraInfoEntry, ExtraInfoTalkerController>
    {
        public override NamespacedKey Id => ProgressViewId.ExtraInfo;

        [SerializeField] private SpeechBubbleLayer _speechLayer = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private ToggleableObjectController _buttonAnimationController = null;

        private SpeechBubbleController _speechBubble = null;

        private ExtraInfoTalkerController Talker { get; set; }
        private IExtraInfoEntry ExtraInfoEntry { get; set; }
        private bool PlayDisappearAnimation { get; set; }

        private SpeechBubbleLayer SpeechLayer => _speechLayer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _confirmButton.onClick.AddListener(Confirm);
            _confirmText.key = LKey.Views.ExtraInfo.Confirm;
            _buttonAnimationController.HideImmediate();
        }

        private void OnDisable()
        {
            if(_speechBubble) _speechBubble.Remove();
        }

        public void Open(IExtraInfoEntry entry, ExtraInfoTalkerController talker)
        {
            ExtraInfoEntry = entry;
            Talker = talker;

            if (ExtraInfoEntry == null)
            {
                Debug.LogError("This progress step does not have any extra info!");
                return;
            }

            base.Open();
        }

        protected override StateChangePromise PrepareOpen()
        {
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();
            var promise = new StateChangePromise();
            var cameraSystem = Views.CameraSystem;
            cameraSystem.Transition(Talker.CreateCameraTransformation(), 1).Then(() =>
            {
                Talker.Approach().Then(promise.Fulfill);
            });
            return promise;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Views.BlackMask.BlockInteractions(false);
            var localization = Views.Localization;
            var text = localization.GetString(ExtraInfoEntry.Text);
            _speechBubble = SpeechLayer.ShowSpeechBubble(new TextSpeechBubble(Talker, text, new Vector2(0.1f, 0)));
            _buttonAnimationController.HideImmediate();
            _buttonAnimationController.Show();
            Talker.PlayTalkAnimation();
        }

        private void Confirm()
        {
            var sawFirstTime = ExtraInfoEntry.State != ProgressEntry.ProgressState.Completed;

            PlayDisappearAnimation = sawFirstTime;
            if (sawFirstTime)
            {
                ExtraInfoEntry.State = ProgressEntry.ProgressState.Completed;
                DopeElectionsApp.Instance.User.Save();
            }

            Close();
        }

        protected override StateChangePromise PrepareClose()
        {
            if(_speechBubble) _speechBubble.Remove();

            _buttonAnimationController.Hide();
            
            Views.BlackMask.BlockInteractions(true);
            var promise = new StateChangePromise();
            if (PlayDisappearAnimation)
            {
                Talker.Disappear().Then(()=>
                {
                    promise.Fulfill();
                    Views.ProgressionView.Open(true);
                });
            }

            return promise;
        }

        protected override void OnClose()
        {
            base.OnClose();
            _buttonAnimationController.HideImmediate();
        }
    }
}