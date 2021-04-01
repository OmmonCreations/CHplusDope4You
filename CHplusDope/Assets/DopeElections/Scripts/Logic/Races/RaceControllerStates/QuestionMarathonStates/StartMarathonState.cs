using DopeElections.Sounds;
using FMODSoundInterface;

namespace DopeElections.Races
{
    public class StartMarathonState : MarathonState
    {
        public StartMarathonState(QuestionMarathonRaceController raceController, QuestionMarathon marathon) : base(
            raceController, marathon)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var overlay = RaceController.OverlayController;
            overlay.Show();
            MusicController.Play(Music.Race);
        }

        public override void Update()
        {
            IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RaceController.PrepareQuestion(new QuestionRace(Marathon, Marathon.CurrentQuestion,
                Marathon.CurrentQuestionIndex));
        }
    }
}