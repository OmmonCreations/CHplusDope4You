using DopeElections.ObstacleCourses;
using Essentials;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public class ImpactObstacle : RaceObstacle<ImpactObstacleType>, IBlockingObstacle, IDropoutObstacle
    {
        public TrackAnchor Anchor { get; }
        public float Delay { get; }

        public ImpactObstacle(RaceObstacleCourse course, ImpactObstacleType type, Vector2Int position, Vector2Int size,
            Data data) : base(course, type, position, size)
        {
            Anchor = data.Anchor;
            Delay = data.Delay;
        }

        public override Color Color => Color.magenta;

        public override bool CanPass(INavigationAgent agent, Vector2Int position)
        {
            return true;
        }

        protected override INavigationAction GetMoveAction(NavigationContext context, Vector2Int @from, Vector2Int to,
            float timestamp)
        {
            if (!IsHit(context, to, timestamp, out var hitTime))
            {
                // return normal move action like on empty tile
                return base.GetMoveAction(context, @from, to, timestamp);
            }

            // try to create a move action that starts after the hit
            var moveAction = base.GetMoveAction(context, @from, to, timestamp + hitTime);
            if (moveAction == null) return null; // skip if that fails

            var hitAction = new SquishAction(@from, @from, hitTime);
            return new CompositeAction(hitAction, moveAction);
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

        public bool TryGetBlockOpportunity(INavigationAgent agent, Vector2Int @from, float fromTimestamp,
            out BlockOpportunity opportunity)
        {
            var position = Position;
            var size = Size;
            if (from.y > position.y + size.y)
            {
                opportunity = default;
                return false;
            }

            var yDistance = position.y - from.y;
            var defaultTileClearTime = Course.Configuration.TileSize / agent.Speed;

            var areaEnterTimestamp = fromTimestamp + defaultTileClearTime * yDistance;
            var areaLeaveTimestamp = areaEnterTimestamp + size.y * defaultTileClearTime;
            var insideAreaTime = areaLeaveTimestamp - areaEnterTimestamp;

            var delay = Delay;
            var interval = Type.Frequency;
            var duration = Type.Duration;

            var impactCountOnEnter = Mathf.FloorToInt((areaEnterTimestamp - delay) / interval);
            var nextImpact = delay + (impactCountOnEnter + 1) * interval;
            if (nextImpact < areaEnterTimestamp || nextImpact > areaLeaveTimestamp)
            {
                opportunity = default;
                return false;
            }

            var relativeY = Mathf.RoundToInt((size.y - 1) * ((nextImpact - areaEnterTimestamp) / insideAreaTime));
            var impactY = position.y + relativeY;
            var impactX = Mathf.Clamp(from.x, position.x, position.x + size.x - 1);

            var impactPosition = new Vector2Int(impactX, impactY);
            var impactTime = new MinMaxRange(nextImpact, nextImpact + duration);
            opportunity = new BlockOpportunity(impactPosition, impactTime);

            return true;
        }

        public INavigationAction GetBlockAction(INavigationAgent agent, Vector2Int position, float timestamp,
            float time)
        {
            return new SquishAction(position, position, time);
        }

        private bool IsHit(NavigationContext context, Vector2Int position, float timestamp, out float hitTime)
        {
            var delay = Delay;
            var interval = Type.Frequency;
            var duration = Type.Duration;
            var enterInterval = Mathf.FloorToInt((timestamp - delay) / interval);
            var timeSinceLastImpact = timestamp - delay - (enterInterval * interval);
            var result = timeSinceLastImpact <= duration;
            // Debug.Log(timestamp + " " + (result ? "is" : "is not") + " a hit. Delay: " + delay + ", Interval: " +
            //           interval + ", Duration: " + duration);
            hitTime = Mathf.Max(Type.StunDuration, duration - timeSinceLastImpact);
            return result;
        }

        public enum TrackAnchor
        {
            Left,
            Center,
            Right
        }

        public class Data
        {
            public TrackAnchor Anchor { get; }
            public float Delay { get; }

            public Data(TrackAnchor anchor, float delay)
            {
                Anchor = anchor;
                Delay = delay;
            }
        }
    }
}