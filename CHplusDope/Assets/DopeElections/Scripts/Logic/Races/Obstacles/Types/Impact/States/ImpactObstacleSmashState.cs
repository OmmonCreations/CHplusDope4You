using UnityEngine;

namespace DopeElections.Races.States
{
    public class ImpactObstacleSmashState : ImpactObstacleState
    {
        private float ImpactDelay { get; }
        private float Duration { get; }

        private bool _impactTriggered = false;

        private float _t;
        
        public ImpactObstacleSmashState(ImpactObstacleController controller, float impactDelay, float duration) : base(controller)
        {
            ImpactDelay = impactDelay;
            Duration = impactDelay + duration;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlaySmashAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (_t >= ImpactDelay && !_impactTriggered)
            {
                _impactTriggered = true;
                TriggerImpact();
            }

            if (_t >= Duration) IsCompleted = true;
        }

        private void TriggerImpact()
        {
            Controller.PlayImpactAnimation();
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.Idle();
        }
    }
}