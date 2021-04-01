using UnityEngine;

namespace DopeElections.ScriptedSequences.Splash
{
    public class TitleAnimationState : SplashCinematicState
    {
        private float AnimationTime { get; }

        private RectTransform LogoTransform { get; }
        private RectTransform SubtitleTransform { get; }
        private RectTransform PlayButtonTransform { get; }
        private CanvasGroup LogoGroup { get; }
        private CanvasGroup SubtitleGroup { get; }
        private CanvasGroup PlayButtonGroup { get; }

        private AnimationCurve LogoSizeCurve { get; }
        private AnimationCurve LogoOpacityCurve { get; }
        private AnimationCurve SubtitleSizeCurve { get; }
        private AnimationCurve SubtitleOpacityCurve { get; }
        private AnimationCurve PlayButtonSizeCurve { get; }
        private AnimationCurve PlayButtonOpacityCurve { get; }

        private float _t;

        public TitleAnimationState(SplashCinematicController controller, float animationTime) : base(controller)
        {
            AnimationTime = animationTime;

            LogoTransform = controller.LogoTransform;
            SubtitleTransform = controller.SubtitleTransform;
            PlayButtonTransform = controller.PlayButtonTransform;
            LogoGroup = controller.LogoGroup;
            SubtitleGroup = controller.SubtitleGroup;
            PlayButtonGroup = controller.PlayButtonGroup;

            LogoSizeCurve = controller.LogoSizeCurve;
            LogoOpacityCurve = controller.LogoOpacityCurve;
            SubtitleSizeCurve = controller.SubtitleSizeCurve;
            SubtitleOpacityCurve = controller.SubtitleOpacityCurve;
            PlayButtonSizeCurve = controller.PlayButtonSizeCurve;
            PlayButtonOpacityCurve = controller.PlayButtonOpacityCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            LogoGroup.alpha = 0;
            SubtitleGroup.alpha = 0;
            PlayButtonGroup.alpha = 0;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            UpdateLogo();
            UpdateSubtitle();
            UpdatePlayButton();
            if (_t >= 1) IsCompleted = true;
        }

        private void UpdateLogo()
        {
            var sizeProgress = LogoSizeCurve.Evaluate(_t);
            var opacityProgress = LogoOpacityCurve.Evaluate(_t);
            LogoTransform.localScale = Vector3.one * sizeProgress;
            LogoGroup.alpha = opacityProgress;
        }

        private void UpdateSubtitle()
        {
            var sizeProgress = SubtitleSizeCurve.Evaluate(_t);
            var opacityProgress = SubtitleOpacityCurve.Evaluate(_t);
            SubtitleTransform.localScale = Vector3.one * sizeProgress;
            SubtitleGroup.alpha = opacityProgress;
        }

        private void UpdatePlayButton()
        {
            var sizeProgress = PlayButtonSizeCurve.Evaluate(_t);
            var opacityProgress = PlayButtonOpacityCurve.Evaluate(_t);
            PlayButtonTransform.localScale = Vector3.one * sizeProgress;
            PlayButtonGroup.alpha = opacityProgress;
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            LogoGroup.alpha = 1;
            SubtitleGroup.alpha = 1;
            PlayButtonGroup.alpha = 1;
            LogoTransform.localScale = Vector3.one;
            SubtitleTransform.localScale = Vector3.one;
            PlayButtonTransform.localScale = Vector3.one;
        }
    }
}