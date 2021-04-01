using StateMachines;

namespace DopeElections.ScriptedSequences
{
    public interface ICinematicState
    {
        event State.StateEvent Started;
        event State.StateEvent Skipped;
        event State.StateEvent OnFinished;
        SkipInputType SkipInputType { get; }
        SkipRange SkipRange { get; }
    }
}