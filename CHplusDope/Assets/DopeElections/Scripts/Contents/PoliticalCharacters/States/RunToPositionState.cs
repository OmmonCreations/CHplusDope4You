using UnityEngine;

namespace DopeElections.PoliticalCharacters
{
    public class RunToPositionState : PoliticalCharacterState
    {
        private Transform Transform { get; }
        private Vector3 Target { get; }
        private float Speed { get; }

        public RunToPositionState(PoliticalCharacterController controller, Vector3 target, float speed) : base(controller)
        {
            Transform = controller.transform;
            Target = target;
            Speed = speed;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.MovementSpeed = Speed;
            Controller.PlayRunningAnimation();
        }

        public override void Update()
        {
            var previousPosition = Transform.position;
            var currentPosition = Vector3.MoveTowards(previousPosition, Target, Speed * Time.deltaTime);
            Transform.position = currentPosition;
            if ((currentPosition - previousPosition).sqrMagnitude > 0)
            {
                Transform.rotation = Quaternion.LookRotation(currentPosition - previousPosition, Vector3.up);
            }
            if ((Target - currentPosition).sqrMagnitude <= 0.01f) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.PlayIdleAnimation();
        }
    }
}