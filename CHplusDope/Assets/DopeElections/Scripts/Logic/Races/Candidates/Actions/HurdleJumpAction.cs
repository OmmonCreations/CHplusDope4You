using System;
using UnityEngine;

namespace DopeElections.Races
{
    public class HurdleJumpAction : RaceCandidateAction
    {
        public JumpType Type { get; }
        public float Height { get; }
        public override float MovementSmoothing { get; } = 0.01f;

        public HurdleJumpAction(Vector2Int @from, Vector2Int to, float time, float height, JumpType type) : base(@from,
            to, time)
        {
            Type = type;
            Height = height;
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            switch (Type)
            {
                case JumpType.Graceful:
                    candidate.PlayHurdleJump100Animation(Height, Time);
                    break;
                case JumpType.Acceptable:
                    candidate.PlayHurdleJump75Animation(Height, Time);
                    break;
                case JumpType.Stumble:
                    candidate.PlayHurdleJump50Animation(Height, Time);
                    break;
                case JumpType.Crash:
                    candidate.PlayHurdleJump25Animation(Height, Time);
                    break;
                default:
                    throw new InvalidOperationException("Unknown jump type " + Type);
            }
        }

        public enum JumpType
        {
            Graceful,
            Acceptable,
            Stumble,
            Crash
        }
    }
}