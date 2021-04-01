using StateMachines;

namespace DopeElections.Races.States
{
    public abstract class CannonState : State
    {
        protected CannonObstacleController Controller { get; }

        protected CannonState(CannonObstacleController controller)
        {
            Controller = controller;
        }
    }
}