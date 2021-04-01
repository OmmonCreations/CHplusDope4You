using StateMachines;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public abstract class GrabSequencePlayerState : State
    {
        protected GrabSequencePlayerController PlayerController { get; }

        protected GrabSequencePlayerState(GrabSequencePlayerController playerController)
        {
            PlayerController = playerController;
        }
    }
}