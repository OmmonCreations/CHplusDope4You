using System;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Startup
{
    public class LoadingScreenView : StartupView, IView
    {
        public override NamespacedKey Id => StartupViewId.LoadingScreen;

        [SerializeField] private Image _loadingBarFill = null;
        [SerializeField] private LocalizedText _progressLabelText = null;

        private float _currentProgress;
        private float _targetProgress;
        private float _velocity;

        public LocalizedText ProgressLabel => _progressLabelText;
        public Func<float> ProgressFeedback { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetProgress(0);
        }

        public new void Open()
        {
            base.Open();
        }

        private void LateUpdate()
        {
            if (ProgressFeedback != null)
            {
                _targetProgress = ProgressFeedback();
            }

            var currentProgress = Mathf.SmoothDamp(_currentProgress, _targetProgress, ref _velocity, 0.1f);
            SetProgress(currentProgress);
        }

        private void SetProgress(float progress)
        {
            _currentProgress = progress;
            _loadingBarFill.fillAmount = progress;
        }
    }
}