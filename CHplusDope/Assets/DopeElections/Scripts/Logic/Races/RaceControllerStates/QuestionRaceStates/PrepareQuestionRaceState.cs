namespace DopeElections.Races
{
    public class PrepareQuestionRaceState : QuestionRaceState
    {
        public PrepareQuestionRaceState(QuestionMarathonRaceController raceController, QuestionRace race) : base(
            raceController, race)
        {
        }

        public override void Update()
        {
            IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RaceController.ShowQuestion(Race);
        }
    }
}