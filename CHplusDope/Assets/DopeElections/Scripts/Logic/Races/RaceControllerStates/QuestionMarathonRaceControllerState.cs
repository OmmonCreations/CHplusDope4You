using StateMachines;

namespace DopeElections.Races
{
    public abstract class QuestionMarathonRaceControllerState : State
    {
        protected QuestionMarathonRaceController RaceController { get; }

        protected QuestionMarathonRaceControllerState(QuestionMarathonRaceController raceController)
        {
            RaceController = raceController;
        }
    }
}