using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public abstract class PartyCandidateAnimationState : PartyCandidateState
    {
        protected abstract float AnimationTime { get; }

        private float _animationTime;
        private float _t;

        public PartyCandidateAnimationState(PartyCandidateController controller) : base(controller)
        {
        }

        protected sealed override void OnInitialize()
        {
            base.OnInitialize();
            _animationTime = AnimationTime;
            PlayAnimation();
        }

        public sealed override void Update()
        {
            _t += Time.deltaTime;
            if (_t >= _animationTime) IsCompleted = true;
        }

        protected abstract void PlayAnimation();
    }
}