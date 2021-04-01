using StateMachines;

namespace DopeElections.Races.RaceTracks
{
    public abstract class RaceTrackState : State
    {
        protected RaceTrackController RaceTrackController { get; }
        protected RaceTrack RaceTrack { get; }

        protected RaceTrackState(RaceTrackController controller)
        {
            RaceTrackController = controller;
            RaceTrack = controller.RaceTrack;
        }
    }
}