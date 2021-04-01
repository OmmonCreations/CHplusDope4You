using System.Linq;
using DopeElections.Candidates;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate attaches to an anchor to subsequently play some sort of interaction
    /// </summary>
    public class AttachToAnchorAction : RaceCandidateAction
    {
        public float JumpHeight { get; }
        public override float MovementSmoothing { get; } = 0.01f;

        public AttachToAnchorAction(Vector2Int @from, Vector2Int to, float time, float jumpHeight) : base(@from, to,
            time)
        {
            JumpHeight = jumpHeight;
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            var courseController = candidate.ObstacleCourseController;
            var course = courseController.Course;
            var obstacle = course.Tiles[To.y, To.x];
            if (!(courseController.ObstacleControllers
                .FirstOrDefault(c => c.Obstacle == obstacle) is ICandidateAnchor anchor))
            {
                Debug.LogWarning("AttachToAnchorAction did not find a candidate anchor at " + To + "!");
                return;
            }

            candidate.PlayAttachAnimation(anchor, JumpHeight, Time, AnimationCurve.Linear(0, 0, 1, 1),
                candidate.Animations.JumpArcCurve);
        }

        protected override void OnStopped(RaceCandidateController candidate)
        {
            base.OnStopped(candidate);
            candidate.PlayAttachAnimation(candidate, 0, 0.01f, AnimationCurve.Constant(0, 1, 1),
                AnimationCurve.Constant(0, 1, 1));
        }
    }
}