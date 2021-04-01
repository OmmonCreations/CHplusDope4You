using DopeElections.Sounds;
using FMODSoundInterface;

namespace DopeElections.ScriptedSequences.EndingComic.States
{
    public class PlayCinematicState : EndingCinematicState, ICinematicState
    {
        public override SkipInputType SkipInputType { get; } = SkipInputType.Custom;
        public override SkipRange SkipRange { get; } = SkipRange.Section;

        public PlayCinematicState(EndingCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.Completed += OnCinematicFinished;
            Controller.CinematicDirector.Play();
            MusicController.Play(Music.EndingCinematic);
        }

        public override void Update()
        {
            
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.Completed -= OnCinematicFinished;
        }

        protected override void OnSkip()
        {
            Controller.Skip();
            base.OnSkip();
        }

        private void OnCinematicFinished()
        {
            IsCompleted = true;
        }
    }
}