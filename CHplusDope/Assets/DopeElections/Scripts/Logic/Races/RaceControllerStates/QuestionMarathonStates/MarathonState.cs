namespace DopeElections.Races
{
    public abstract class MarathonState : QuestionMarathonRaceControllerState
    {
        public QuestionMarathon Marathon { get; }

        protected MarathonState(QuestionMarathonRaceController raceController, QuestionMarathon marathon) : base(
            raceController)
        {
            Marathon = marathon;
        }
    }
}