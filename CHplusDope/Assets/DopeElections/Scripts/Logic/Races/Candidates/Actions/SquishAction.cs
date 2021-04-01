using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate gets squished from above
    /// </summary>
    public class SquishAction : RaceCandidateAction
    {
        public SquishAction(Vector2Int @from, Vector2Int to, float time) : base(@from, to, time)
        {
            
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.PlaySquishAnimation();
        }
    }
}