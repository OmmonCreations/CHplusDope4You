using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate starts falling
    /// </summary>
    public class StumbleAction : RaceCandidateAction
    {
        private float FallTime { get; }
        
        public StumbleAction(Vector2Int @from, Vector2Int to, float time, float fallTime) : base(@from, to, time)
        {
            FallTime = fallTime;
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            if (candidate.CurrentAction is StumbleAction) return;
            candidate.PlayFallAnimation(FallTime);
        }
    }
}