using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate jumps to a target position
    /// </summary>
    public class JumpAction : RaceCandidateAction
    {
        public float Height { get; }
        public override float MovementSmoothing { get; } = 0.01f;

        public JumpAction(Vector2Int @from, Vector2Int to, float time, float height) : base(@from, to, time)
        {
            Height = height;
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.PlayJumpAnimation(Height, Time);
        }
    }
}