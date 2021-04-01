using DopeElections.Races.RaceTracks;
using StateMachines;

namespace DopeElections.Races
{
    public abstract class RaceCameraState : State
    {
        protected RaceController RaceController => CameraController.RaceController;
        protected RaceTrackController RaceTrack => CameraController.RaceController.RaceTrackController;
        protected RaceCameraController CameraController { get; }

        protected RaceCameraState(RaceCameraController controller)
        {
            CameraController = controller;
        }
    }
}