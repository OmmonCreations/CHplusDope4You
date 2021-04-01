using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate appears on the track
    /// </summary>
    public class RespawnAction : RaceCandidateAction
    {
        public RespawnAction(Vector2Int position) : base(position, position, 0)
        {
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            var courseController = candidate.ObstacleCourseController;
            candidate.Position = courseController.GetRaceTrackVector(To);
            candidate.Spawn();
        }
    }
}