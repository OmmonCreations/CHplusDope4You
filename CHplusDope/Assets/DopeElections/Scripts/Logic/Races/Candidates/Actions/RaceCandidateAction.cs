using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public abstract class RaceCandidateAction : NavigationAction
    {
        public virtual float MovementSmoothing { get; } = 0.1f;

        protected RaceCandidateAction(Vector2Int @from, Vector2Int to, float time) : base(@from, to, time)
        {
        }

        protected override void OnStarted(INavigationAgent agent)
        {
            base.OnStarted(agent);
            if (agent is RaceCandidateController candidate) OnStarted(candidate);
        }

        protected virtual void OnStarted(RaceCandidateController candidate)
        {
        }

        protected override void OnStopped(INavigationAgent agent)
        {
            base.OnStopped(agent);
            if (agent is RaceCandidateController candidate) OnStopped(candidate);
        }

        protected virtual void OnStopped(RaceCandidateController candidate)
        {
        }

        public override string ToString()
        {
            return GetType().Name + " (" + From + " to " + To + " in " + Time + "s)";
        }
    }
}