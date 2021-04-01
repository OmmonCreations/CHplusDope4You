using DopeElections.Answer;
using StateMachines;

namespace DopeElections.Candidates.States
{
    public abstract class CandidateState : State
    {
        protected CandidateController Controller { get; }
        protected Candidate Candidate { get; }
        
        protected CandidateState(CandidateController controller)
        {
            Controller = controller;
            Candidate = controller.Candidate;
        }
    }
}