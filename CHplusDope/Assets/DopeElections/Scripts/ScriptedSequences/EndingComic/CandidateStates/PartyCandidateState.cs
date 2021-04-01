using StateMachines;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public abstract class PartyCandidateState : State
    {
        protected PartyCandidateController Controller { get; }
    
        protected PartyCandidateState(PartyCandidateController controller)
        {
            Controller = controller;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.PlayNextAction();
        }
    }
}