using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate flies like a projectile to a target position
    /// </summary>
    public class ProjectileAction : RaceCandidateAction
    {
        public float JumpHeight { get; }
        public override float MovementSmoothing { get; } = 0.01f;

        public ProjectileAction(Vector2Int @from, Vector2Int to, float time, float jumpHeight) : base(@from, to, time)
        {
            JumpHeight = jumpHeight;
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.PlayAttachAnimation(candidate, JumpHeight, Time, candidate.Animations.BallisticTimeCurve, candidate.Animations.BallisticArcCurve);
        }
    }
}