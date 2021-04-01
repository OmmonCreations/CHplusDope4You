using UnityEngine;

namespace DopeElections.PoliticalCharacters
{
    public class JumpState : PoliticalCharacterState
    {
        private string Animation { get; }
        private float Height { get; }
        private float AnimationTime { get; }
        private AnimationCurve ArcCurve { get; }

        private Transform Transform { get; }

        private Vector3 _startPosition;
        private float _t;
        
        public JumpState(PoliticalCharacterController controller, string animation, float height, float time, AnimationCurve arcCurve) : base(controller)
        {
            Animation = animation;
            Height = height;
            AnimationTime = time;
            ArcCurve = arcCurve;
            Transform = controller.transform;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Transform.localPosition;
            Controller.PlayJumpAnimation(Animation, AnimationTime);
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = ArcCurve.Evaluate(_t);
            var y = progress * Height;
            Transform.localPosition = _startPosition + new Vector3(0, y, 0);
            IsCompleted |= _t >= 1;
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            if(Transform) Transform.localPosition = _startPosition;
        }
    }
}