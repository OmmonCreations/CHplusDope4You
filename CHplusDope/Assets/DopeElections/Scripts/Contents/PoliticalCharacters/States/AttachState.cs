using DopeElections.Candidates;
using UnityEngine;

namespace DopeElections.PoliticalCharacters
{
    public class AttachState : PoliticalCharacterState
    {
        private float JumpHeight { get; }
        private float AnimationTime { get; }
        private AnimationCurve TimeCurve { get; }
        private AnimationCurve ArcCurve { get; }

        private Transform Transform { get; }
        private Transform PreviousParent { get; }
        private Transform NewParent { get; }

        private float _t;

        public AttachState(PoliticalCharacterController controller, ICandidateAnchor anchor, float jumpHeight, float time,
            AnimationCurve timeCurve, AnimationCurve arcCurve) : base(controller)
        {
            JumpHeight = jumpHeight;
            AnimationTime = time;
            TimeCurve = timeCurve;
            ArcCurve = arcCurve;

            var transform = controller.transform;
            Transform = transform;
            PreviousParent = transform.parent;
            NewParent = anchor.CandidateAnchor;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlayJumpAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = TimeCurve.Evaluate(_t);
            var jumpProgress = ArcCurve.Evaluate(_t);
            var y = jumpProgress * JumpHeight;

            var worldPosition = Vector3.Lerp(PreviousParent.position, NewParent.position, progress);
            var worldRotation = Quaternion.Slerp(PreviousParent.rotation, NewParent.rotation, progress);

            var jumpOffset = new Vector3(0, y, 0);

            Transform.position = worldPosition + jumpOffset;
            Transform.rotation = worldRotation;

            IsCompleted |= _t >= 1;
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            if (Transform && NewParent)
            {
                Transform.SetParent(NewParent, false);
                Transform.position = NewParent.position;
                Transform.rotation = NewParent.rotation;
            }
        }
    }
}