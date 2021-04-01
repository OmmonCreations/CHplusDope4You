using StateMachines;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public abstract class GrabArmState : State
    {
        protected GrabArmController Controller { get; }

        protected GrabArmState(GrabArmController controller)
        {
            Controller = controller;
        }
    }
}