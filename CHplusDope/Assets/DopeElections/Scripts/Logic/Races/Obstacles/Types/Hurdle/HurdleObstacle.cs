using DopeElections.ObstacleCourses;
using FMODSoundInterface;
using Navigation;
using UnityEngine;

namespace DopeElections.Races.Hurdle
{
    public class HurdleObstacle : RaceObstacle<HurdleObstacleType>, IBlockingObstacle
    {
        private static readonly Vector2Int[] MovementDirections =
        {
            new Vector2Int(0, 1) // only allow crossing in one direction
        };

        public HurdleObstacle(RaceObstacleCourse course, HurdleObstacleType type, Vector2Int position, Vector2Int size)
            : base(course, type, position, size)
        {
        }

        public override Color Color => Color.gray;

        public override bool CanPass(INavigationAgent agent, Vector2Int position)
        {
            return true;
        }

        protected override Vector2Int[] GetMovementDirections(NavigationContext context) => MovementDirections;

        protected override INavigationAction GetMoveAction(NavigationContext context, Vector2Int @from, Vector2Int to,
            float timestamp)
        {
            var size = Size;
            var time = TileSize * 1.9f / context.Agent.Speed;
            var jumpType = HurdleJumpAction.JumpType.Acceptable;
            var controller = context.Agent as RaceCandidateController;
            
            var result = new HurdleJumpAction(@from, @from + new Vector2Int(0, size.y + 1), time, Type.JumpHeight,
                jumpType);
            result.Started += OnAgentCleared;
            return result;
        }

        public bool TryGetBlockOpportunity(INavigationAgent agent, Vector2Int @from, float fromTimestamp,
            out BlockOpportunity opportunity)
        {
            opportunity = new BlockOpportunity(Position);
            return true;
        }

        public INavigationAction GetBlockAction(INavigationAgent agent, Vector2Int position, float timestamp,
            float time)
        {
            var result = new HurdleJumpAction(position, position, time, Type.CrashHeight,
                HurdleJumpAction.JumpType.Crash);
            result.Started += OnAgentCrashed;
            return result;
        }

        private void OnAgentCleared(INavigationAgent agent)
        {
            var controller = agent as RaceCandidateController;
            if (controller)
            {
                SoundController.Play(Type.ClearedSound, controller.gameObject);
            }
        }

        private void OnAgentCrashed(INavigationAgent agent)
        {
            var controller = agent as RaceCandidateController;
            if (controller)
            {
                SoundController.Play(Type.DroppedSound, controller.gameObject);
            }
        }
    }
}