using StateMachines;

namespace DopeElections.Races
{
    public abstract class RaceOverlayState : State
    {
        protected RaceOverlayController Overlay { get; }

        protected RaceOverlayState(RaceOverlayController controller)
        {
            Overlay = controller;
        }
    }
}