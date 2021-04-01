namespace DopeElections.Races
{
    public abstract class QuestionRaceState : QuestionMarathonRaceControllerState
    {
        protected QuestionMarathon Marathon => Race.Marathon;
        protected QuestionRace Race { get; }

        protected QuestionRaceState(QuestionMarathonRaceController raceController, QuestionRace race) : base(
            raceController)
        {
            Race = race;
        }
    }
}