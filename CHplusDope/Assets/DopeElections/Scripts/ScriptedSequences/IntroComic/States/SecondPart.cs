using DopeElections.Sounds;
using FMODSoundInterface;

namespace DopeElections.ScriptedSequences.IntroComic.States
{
    public class SecondPart : IntroComicSequenceState, ICinematicState
    {
        public SecondPart(IntroComicSequenceController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.SecondPartCompleted += OnPartCompleted;
            Controller.SecondPartObject.SetActive(true);
            Controller.SecondPartAnimator.Play("animation");
            MusicController.Play(Music.IntroCinematic2);
            SoundController.Play(IntroComicSoundId.Interaction);
        }

        public override void Update()
        {
            var state = Controller.SecondPartAnimator.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 1) Controller.TriggerSecondPartComplete();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.SecondPartCompleted -= OnPartCompleted;
            Controller.SecondPartObject.SetActive(false);
        }

        private void OnPartCompleted()
        {
            IsCompleted = true;
        }
    }
}