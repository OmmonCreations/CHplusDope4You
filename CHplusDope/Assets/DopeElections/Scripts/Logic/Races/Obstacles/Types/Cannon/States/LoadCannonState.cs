using UnityEngine;

namespace DopeElections.Races.States
{
    public class LoadCannonState : CannonState
    {
        private float AnimationTime { get; }

        private Quaternion _from;
        private Quaternion _to;

        private float _t;

        public LoadCannonState(CannonObstacleController controller, float animationTime) : base(controller)
        {
            AnimationTime = animationTime;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _from = Controller.BarrelRotation;
            _to = Quaternion.Euler(Controller.BarrelLoadAngle);
            Controller.PlayLoadAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.SmoothStep(0, 1, _t);
            Controller.BarrelRotation = Quaternion.Lerp(_from, _to, progress);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.BarrelRotation = _to;
        }
    }
}