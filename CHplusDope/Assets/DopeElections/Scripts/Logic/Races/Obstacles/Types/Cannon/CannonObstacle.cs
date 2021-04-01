using DopeElections.ObstacleCourses;
using Effects;
using Navigation;
using UnityEngine;
using Random = RandomUtils.Random;

namespace DopeElections.Races
{
    public class CannonObstacle : RaceObstacle<CannonObstacleType>, IPathPostCompiler
    {
        public delegate void CannonEvent(INavigationAgent agent);

        public event CannonEvent LoadStarted = delegate { };
        public event CannonEvent Shot = delegate { };

        private INavigationAgent User { get; }
        public float LoadTime { get; }
        public float AimTime { get; }

        public CannonObstacle(RaceObstacleCourse course, CannonObstacleType type, Vector2Int position, Vector2Int size,
            INavigationAgent user)
            : base(course, type, position, size)
        {
            User = user;
            LoadTime = type.LoadTime + Random.Range(-type.LoadTimeRandomization / 2, type.LoadTimeRandomization / 2);
            AimTime = type.LoadTime + Random.Range(-type.AimTimeRandomization / 2, type.AimTimeRandomization / 2);
        }

        public override Color Color => Color.green;

        public override bool CanPass(INavigationAgent agent, Vector2Int position)
        {
            return agent == User;
        }

        protected override INavigationAction GetMoveAction(NavigationContext context, Vector2Int @from, Vector2Int to,
            float timestamp)
        {
            var agent = context.Agent;
            if (agent != User) return null;
            var anchorTile = Contains(to) ? to : Position;
            var agentTarget = context.To;
            var yDistance = (agentTarget - from).y;
            var time = yDistance / (agent.Speed * 1.5f);
            return new CompositeAction(
                new AttachToAnchorAction(from, anchorTile, LoadTime, 3f),
                new IdleAction(anchorTile, AimTime + Type.ShootDelay),
                new ProjectileAction(anchorTile, agentTarget, time, yDistance / 2f)
            );
        }

        public void PostCompile(RawPath path, int ownIndex)
        {
            var loadAction = path.Actions[ownIndex];
            var shootAction = ownIndex + 2 < path.Actions.Count ? path.Actions[ownIndex + 2] as NavigationAction : null;
            if (loadAction is AttachToAnchorAction attachAction)
            {
                attachAction.Started += candidate => LoadStarted(candidate);
            }

            if (shootAction != null)
            {
                var trailEffectType = Type.TrailEffect;
                EffectInstance trailEffect = null;
                shootAction.Started += agent =>
                {
                    Shot(agent);

                    var candidate = agent as RaceCandidateController;
                    var parent = candidate != null ? candidate.CandidateTransform : null;
                    var reference = candidate != null ? candidate.RaceController.RaceTrackController.Root : null;
                    trailEffect = candidate != null && trailEffectType != null
                        ? candidate.EffectsController.PlayEffect(trailEffectType, parent, reference)
                        : null;
                };
                shootAction.Stopped += agent =>
                {
                    if (trailEffect != null) trailEffect.Remove();
                };
            }
        }
    }
}