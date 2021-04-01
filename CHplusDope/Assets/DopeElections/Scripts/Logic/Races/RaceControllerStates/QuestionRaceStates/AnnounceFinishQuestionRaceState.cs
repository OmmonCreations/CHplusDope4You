namespace DopeElections.Races
{
    public class AnnounceFinishQuestionRaceState : QuestionRaceState
    {
        public AnnounceFinishQuestionRaceState(QuestionMarathonRaceController raceController, QuestionRace race) : base(
            raceController, race)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            /*
            var announcer = RaceController.Announcer;
            var state = announcer.Announce(LKey.Announcer.FinishRace, default);
            state.OnCompleted += () => IsCompleted = true;
            */
            IsCompleted = true; // skip this step
        }

        public override void Update()
        {
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RaceController.Continue();
        }
    }
}