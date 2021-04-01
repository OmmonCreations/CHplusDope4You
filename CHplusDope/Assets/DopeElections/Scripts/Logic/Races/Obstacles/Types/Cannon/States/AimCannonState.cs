using UnityEngine;

namespace DopeElections.Races.States
{
    public class AimCannonState : CannonState
    {
        private float AnimationTime { get; }

        private Quaternion AimRotation { get; }

        private Quaternion _startRotation;
        private float _t;
        
        public AimCannonState(CannonObstacleController controller, float animationTime) : base(controller)
        {
            AnimationTime = animationTime;
            AimRotation = Quaternion.Euler(controller.BarrelAimAngle);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startRotation = Controller.BarrelRotation;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.SmoothStep(0, 1, _t);
            Controller.BarrelRotation = Quaternion.Lerp(_startRotation, AimRotation, progress);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.BarrelRotation = AimRotation;
        }
    }
}