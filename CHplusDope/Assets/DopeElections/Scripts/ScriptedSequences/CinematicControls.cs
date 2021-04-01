using System;
using AnimatedObjects;
using DopeElections.Layouts;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.ScriptedSequences
{
    public class CinematicControls : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Button _toggleControlsButton = null;
        [SerializeField] private Button _skipButton = null;
        [SerializeField] private LocalizedText _skipText = null;
        [SerializeField] private ToggleableObjectController _controlsAnimationController = null;
        [SerializeField] private DelayedActionTrigger _delayedActionTrigger = null;
        [SerializeField] private float _visibleTime = 2;

        private CinematicController Controller { get; set; }

        public CanvasGroup CanvasGroup => _canvasGroup;
        public Button ToggleControlsButton => _toggleControlsButton;
        public Button SkipButton => _skipButton;
        public LocalizedText SkipText => _skipText;
        public ToggleableObjectController AnimationController => _controlsAnimationController;

        public bool Interactable
        {
            get => CanvasGroup.interactable;
            set
            {
                CanvasGroup.interactable = value;
                CanvasGroup.blocksRaycasts = value;
                _toggleControlsButton.interactable = value;
            }
        }

        private void Awake()
        {
            SkipButton.onClick.AddListener(Skip);
            ToggleControlsButton.onClick.AddListener(ShowControls);
            SkipText.key = LKey.Components.Cinematic.Skip;
            AnimationController.Appeared += TriggerHideControlsDelayed;
            _delayedActionTrigger.onTrigger.AddListener(() =>
            {
                if (!AnimationController.IsVisible) return;
                AnimationController.Hide();
            });
        }

        public void Initialize(CinematicController controller)
        {
            Controller = controller;
            AnimationController.HideImmediate();
        }

        private void ShowControls()
        {
            if (!AnimationController.IsVisible)
            {
                AnimationController.Show();
            }

            TriggerHideControlsDelayed();
        }

        private void TriggerHideControlsDelayed()
        {
            _delayedActionTrigger.Trigger(_visibleTime);
        }

        private void Skip()
        {
            Interactable = false;
            AnimationController.HideImmediate();
            if (Controller) Controller.Skip();
        }

        internal void Bind(ICinematicState state)
        {
            switch (state.SkipInputType)
            {
                case SkipInputType.Custom:
                    BindCustom(state);
                    break;
                default:
                    BindNone(state);
                    break;
            }
        }

        private void BindCustom(ICinematicState state)
        {
            state.Started += () =>
            {
                Interactable = true;
                // if (!ControlsAnimationController.IsVisible) ShowControls();
            };
            state.OnFinished += () =>
            {
                Interactable = false;
                if (AnimationController.IsVisible) AnimationController.HideImmediate();
            };
        }

        private void BindNone(ICinematicState state)
        {
            state.Started += () =>
            {
                Interactable = false;
                if (AnimationController.IsVisible) AnimationController.HideImmediate();
            };
        }
    }
}