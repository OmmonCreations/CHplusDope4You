using System.Linq;
using DopeElections.ObstacleCourses;
using FMODSoundInterface;
using Navigation;
using UnityEngine;
using Random = RandomUtils.Random;

namespace DopeElections.Races
{
    public class PoolObstacle : RaceObstacle<PoolObstacleType>, IBlockingObstacle, IDropoutObstacle, IPathPostCompiler
    {
        private static readonly Vector2Int[] MovementDirections =
        {
            // new Vector2Int(-1, 0), // left
            new Vector2Int(-1, 1), // left forward
            new Vector2Int(0, 1), // forward
            new Vector2Int(1, 1), // right forward
            // new Vector2Int(1, 0) // right
        };

        public override Color Color { get; } = new Color(1, 0.5f, 0);
        protected override float SpeedModifier => Type.SpeedModifier;

        public PoolObstacle(RaceObstacleCourse course, PoolObstacleType type, Vector2Int position, Vector2Int size)
            : base(course, type, position, size)
        {
        }

        public override bool CanPass(INavigationAgent agent, Vector2Int position)
        {
            return true;
        }

        protected override Vector2Int[] GetMovementDirections(NavigationContext context) => MovementDirections;

        protected override INavigationAction GetMoveAction(NavigationContext context, Vector2Int @from, Vector2Int to,
            float timestamp)
        {
            var toTile = context.Tiles[to.y, to.x];
            if (toTile == null)
            {
                Debug.LogWarning("Target tile is missing!");
                return default;
            }

            if (to - from == Vector2.up &&
                toTile.GetOccupants(to, timestamp)
                    .OfType<TemporaryOccupant>()
                    .Any(t => t.Action is ToppleAction))
            {
                return new JumpAction(from, from + new Vector2Int(0, 2), 0.25f, 5);
            }

            return base.GetMoveAction(context, from, to, timestamp);
        }

        public INavigationAction GetDropoutAction(INavigationAgent agent, Vector2Int position,
            float timestamp, float time)
        {
            return GetBlockAction(agent, position, timestamp, time);
        }

        public bool TryGetDropoutOpportunity(INavigationAgent agent, Vector2Int fromPosition, float fromTimestamp,
            out BlockOpportunity opportunity)
        {
            return TryGetBlockOpportunity(agent, fromPosition, fromTimestamp, out opportunity);
        }

        public INavigationAction GetBlockAction(INavigationAgent agent, Vector2Int position,
            float timestamp, float time)
        {
            return new ToppleAction(position, time);
        }

        public bool TryGetBlockOpportunity(INavigationAgent agent, Vector2Int fromPosition, float fromTimestamp,
            out BlockOpportunity opportunity)
        {
            var position = Position;
            var size = Size;
            var y = position.y + Random.Range(0, size.y);
            var maxXDelta = Mathf.RoundToInt(Mathf.Abs(y - fromPosition.y) * 0.1f);
            var xDelta = Mathf.Clamp(position.x + Random.Range(0, size.x) - fromPosition.x, -maxXDelta, maxXDelta);
            var x = fromPosition.x + xDelta;

            var blockPosition = new Vector2Int(x, y);
            opportunity = new BlockOpportunity(blockPosition);
            return true;
        }

        public void PostCompile(RawPath path, int ownIndex)
        {
            if (Type.ToppleParticles != null)
            {
                AddToppleParticles(path, ownIndex);
            }

            if (Type.MovementParticles != null)
            {
                AddStepParticles(path, ownIndex);
            }
        }

        private void AddToppleParticles(RawPath path, int ownIndex)
        {
            var actions = path.Actions;
            if (!(actions[ownIndex] is ToppleAction action))
            {
                return;
            }

            action.Started += SpawnToppleParticles;
        }

        private void AddStepParticles(RawPath path, int ownIndex)
        {
            var actions = path.Actions;
            var action = actions[ownIndex];
            if (ownIndex == 0 || !Contains(action.From)) return;

            if (Contains(path.Actions[ownIndex - 1].From) && Contains(path.Actions[ownIndex - 1].To))
            {
                return; // only add step particles to the first action in this pool
            }

            var tiles = path.NavMesh.Tiles;
            var poolEnterAction = actions[ownIndex];
            var poolExitAction = poolEnterAction;
            for (var i = ownIndex + 1; i < actions.Count; i++)
            {
                var otherAction = actions[i];
                if (tiles[otherAction.To.y, otherAction.To.x] == this) continue;
                poolExitAction = otherAction;
                break;
            }

            if (poolEnterAction is NavigationAction enterAction && poolExitAction != null)
            {
                enterAction.Started += agent => SpawnStepParticles(agent, poolExitAction);
            }
        }

        private void SpawnToppleParticles(INavigationAgent agent)
        {
            if (!(agent is RaceCandidateController candidate)) return;
            var transform = candidate.transform;
            var raceTrackTransform = candidate.RaceController.RaceTrackController.Root;
            var parent = raceTrackTransform;
            var reference = raceTrackTransform;
            var effectInstance = candidate.EffectsController.PlayEffect(Type.ToppleParticles, parent, reference);
            var effectTransform = effectInstance.transform;
            effectTransform.position = transform.position;
            effectTransform.rotation = transform.rotation;
            SoundController.Play(Type.SplashSound);
        }

        private void SpawnStepParticles(INavigationAgent agent, INavigationAction exitAction)
        {
            if (!(agent is RaceCandidateController candidate)) return;
            var raceTrackTransform = candidate.RaceController.RaceTrackController.Root;
            var parent = candidate.transform;
            var reference = raceTrackTransform;
            var effectData = new MovementParticleEffect.EffectData(candidate, exitAction);
            candidate.EffectsController.PlayEffect(Type.MovementParticles, parent, reference, effectData);
        }
    }
}