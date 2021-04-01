using Localizator;
using UnityEngine;

namespace DopeElections.Startup
{
    public abstract class ProgressDisplayingStep : DopeElectionsStartupStep
    {
        private float ProgressStart { get; }
        private float ProgressEnd { get; }

        private float _linearProgress;
        private float _progress;
        private LocalizationKey _progressLabel;
        private LocalizedText _progressLabelText;

        protected float Progress
        {
            get => _linearProgress;
            set
            {
                _linearProgress = value;
                _progress = Mathf.Lerp(ProgressStart, ProgressEnd, value);
            }
        }

        protected LocalizationKey ProgressLabel
        {
            get => _progressLabel;
            set
            {
                _progressLabel = value;
                if (_progressLabelText) _progressLabelText.key = value;
            }
        }

        protected ProgressDisplayingStep(DopeElectionsApp app, float progressStart, float progressEnd) : base(app)
        {
            ProgressStart = progressStart;
            ProgressEnd = progressEnd;
        }

        public override void Run(bool data)
        {
            var sceneController = Object.FindObjectOfType<StartupSceneController>();
            if (sceneController)
            {
                var loadingScreen = sceneController.Views.LoadingScreen;
                loadingScreen.ProgressFeedback = GetProgress;
                if (!loadingScreen.IsOpen) loadingScreen.Open();
                _progressLabelText = loadingScreen.ProgressLabel;
            }

            Progress = ProgressStart;
        }

        private float GetProgress() => _progress;
    }
}