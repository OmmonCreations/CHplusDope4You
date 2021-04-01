using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate disappears from the track with a poof effect
    /// </summary>
    public class HideAction : RaceCandidateAction
    {
        public HideAction(float time) : base(Vector2Int.zero, Vector2Int.zero, time)
        {
        }
        public HideAction(Vector2Int position, float time) : base(position, position, time)
        {
        }
        public HideAction(Vector2Int from, Vector2Int to, float time) : base(from, to, time)
        {
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.Hide();
        }
    }
}