using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class JumpState : PartyCandidateAnimationState
    {
        protected override float AnimationTime => Controller.GetJumpTime(Height);

        private float Height { get; }

        public JumpState(PartyCandidateController controller, float height) : base(controller)
        {
            Height = height;
        }

        protected override void PlayAnimation()
        {
            Controller.PlayJumpAnimation(Height);
        }
    }
}