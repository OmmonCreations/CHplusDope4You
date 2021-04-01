using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class DanceState : PartyCandidateAnimationState
    {
        protected override float AnimationTime => Random.Range(5f, 10f);
        
        public DanceState(PartyCandidateController controller) : base(controller)
        {
        }

        protected override void PlayAnimation()
        {
            Controller.PlayDanceAnimation();
        }
    }
}