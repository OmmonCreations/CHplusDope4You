using UnityEngine;

namespace DopeElections.Races.States
{
    public class ImpactObstacleIdleState : ImpactObstacleState
    {
        private float IdleTime { get; }

        private float _t;
        
        public ImpactObstacleIdleState(ImpactObstacleController controller, float time) : base(controller)
        {
            IdleTime = time;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlayIdleAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (_t >= IdleTime) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.Smash();
        }
    }
}