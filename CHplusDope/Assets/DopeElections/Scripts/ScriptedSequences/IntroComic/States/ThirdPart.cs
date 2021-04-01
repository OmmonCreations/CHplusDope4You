using FMODSoundInterface;

namespace DopeElections.ScriptedSequences.IntroComic.States
{
    public class ThirdPart : IntroComicSequenceState, ICinematicState
    {
        private const float TransitionTo3dTimestamp = 2375 / 60f;
        private const float AnimationTime = 2670 / 60f;

        public override SkipInputType SkipInputType { get; } = SkipInputType.Custom;

        private bool _transitionTo3dTriggered;

        public ThirdPart(IntroComicSequenceController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.Background.SetActive(false);
            Controller.ThirdPartCompleted += OnPartCompleted;
            Controller.ThirdPartObject.SetActive(true);
            Controller.ThirdPartAnimator.Play("animation");
            SoundController.Play(IntroComicSoundId.AfterInteraction);
        }

        public override void Update()
        {
            var state = Controller.ThirdPartAnimator.GetCurrentAnimatorStateInfo(0);
            var normalizedTime = state.normalizedTime;
            if (!_transitionTo3dTriggered && normalizedTime * AnimationTime >= TransitionTo3dTimestamp)
            {
                _transitionTo3dTriggered = true;
                Controller.TriggerTransitionTo3dStarted();
            }

            if (normalizedTime >= 1) Controller.TriggerThirdPartComplete();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.ThirdPartCompleted -= OnPartCompleted;
            Controller.ThirdPartObject.SetActive(false);
        }

        public override void Skip()
        {
            Controller.TriggerThirdPartComplete();
        }

        private void OnPartCompleted()
        {
            IsCompleted = true;
        }
    }
}