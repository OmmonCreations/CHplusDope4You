using StateMachines;

namespace DopeElections.Races.States
{
    public abstract class ImpactObstacleState : State
    {
        protected ImpactObstacleController Controller { get; }
        protected ImpactObstacle Obstacle { get; }

        protected ImpactObstacleState(ImpactObstacleController controller)
        {
            Controller = controller;
            Obstacle = controller.Obstacle;
        }
    }
}