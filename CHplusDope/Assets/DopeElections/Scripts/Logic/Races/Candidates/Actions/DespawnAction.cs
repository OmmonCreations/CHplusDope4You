using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate disappears from the track without any effect
    /// </summary>
    public class DespawnAction : RaceCandidateAction
    {
        public DespawnAction(Vector2Int position) : base(position, position, 0)
        {
            
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.HideImmediate();
        }
    }
}