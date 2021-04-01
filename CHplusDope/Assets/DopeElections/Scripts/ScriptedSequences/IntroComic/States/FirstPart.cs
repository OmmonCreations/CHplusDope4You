using DopeElections.Sounds;
using FMODSoundInterface;
using UnityEngine;

namespace DopeElections.ScriptedSequences.IntroComic.States
{
    public class FirstPart : IntroComicSequenceState, ICinematicState
    {
        private const float FadeInTime = 3;
        private float _t;
        
        private bool _fadeInComplete = false;

        public override SkipInputType SkipInputType { get; } = SkipInputType.Custom;
        public override SkipRange SkipRange { get; } = SkipRange.Section;

        public FirstPart(IntroComicSequenceController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.FirstPartCompleted += OnPartCompleted;
            Controller.gameObject.SetActive(true);
            Controller.Background.SetActive(true);
            Controller.FirstPartObject.SetActive(true);
            Controller.FirstPartAnimator.Play("animation");
            ApplyFade(0);
            MusicController.Play(Music.IntroCinematic1);
            SoundController.Play(IntroComicSoundId.Start);
            Controller.BlackMask.BlockInteractions(false);
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (!_fadeInComplete)
            {
                var fadeInProgress = Mathf.Clamp01(_t / FadeInTime);
                ApplyFade(fadeInProgress);
                if (fadeInProgress >= 1) _fadeInComplete = true;
            }
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.FirstPartCompleted -= OnPartCompleted;
            Controller.FirstPartObject.SetActive(false);
        }

        public override void Skip()
        {
            Controller.FirstPartAnimator.Play("animation", 0, 1);
        }

        private void OnPartCompleted()
        {
            IsCompleted = true;
        }

        private void ApplyFade(float value)
        {
            foreach (var s in Controller.FadeInRenderers)
            {
                var color = s.color;
                color.a = value;
                s.color = color;
            }
        }
    }
}