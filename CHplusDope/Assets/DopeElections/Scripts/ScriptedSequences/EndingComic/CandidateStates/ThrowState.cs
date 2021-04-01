using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class ThrowState : PartyCandidateAnimationState
    {
        protected override float AnimationTime => Controller.GetJumpTime(Height);

        private float Height { get; }

        public ThrowState(PartyCandidateController controller, float height) : base(controller)
        {
            Height = height;
        }

        protected override void PlayAnimation()
        {
            Controller.PlayThrowAnimation(Height);
        }
    }
}