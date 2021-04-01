using StateMachines;

namespace CameraSystems
{
    public abstract class CameraState : State
    {
        protected CameraSystem CameraSystem { get; }
        
        protected CameraState(CameraSystem cameraSystem)
        {
            CameraSystem = cameraSystem;
        }
    }
}