using DopeElections.Candidates;
using StateMachines;

namespace DopeElections.Menus.Teams.SlotStates
{
    public abstract class CandidateSlotState : State
    {
        protected CandidateSlotController Slot { get; }

        protected CandidateSlotState(CandidateSlotController slot)
        {
            Slot = slot;
        }
    }
}