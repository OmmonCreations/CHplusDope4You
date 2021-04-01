using StateMachines;

namespace DopeElections.Races
{
    public abstract class RaceCandidateState : State
    {
        protected RaceCandidateController Controller { get; }
        protected RaceCandidate Candidate { get; }
        protected RaceController RaceController { get; }

        protected RaceCandidateState(RaceCandidateController candidate)
        {
            Controller = candidate;
            Candidate = candidate.Candidate;
            RaceController = candidate.RaceController;
        }
    }
}