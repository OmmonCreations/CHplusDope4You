using UnityEngine;

namespace DopeElections.Races.States
{
    public class ShootCannonState : CannonState
    {
        private float ShootDelay { get; }

        private float _t;

        public ShootCannonState(CannonObstacleController controller, float animationTime) : base(controller)
        {
            ShootDelay = animationTime;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlayShootAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (_t >= ShootDelay) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.PlayShootEffect();
        }
    }
}