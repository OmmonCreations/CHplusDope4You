using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class FloatState : GrabSequencePlayerState
    {
        private Transform Transform { get; }
        private Bounds MovementBox { get; set; }
        private float Speed { get; } = 3;
        private float RotationSpeed { get; } = 50;

        private Vector3 _target;

        public FloatState(GrabSequencePlayerController playerController) : base(playerController)
        {
            Transform = playerController.transform;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var movementBox = PlayerController.MovementBox.bounds;
            var size = movementBox.size;
            var aspectRatio = Screen.width / (float) Screen.height;
            MovementBox = new Bounds(movementBox.center, new Vector3(size.x * aspectRatio, size.y, size.z));

            Transform.localScale = Vector3.one;

            StartRoute();
        }

        public override void Update()
        {
            var position = Vector3.MoveTowards(Transform.position, _target, Time.deltaTime * Speed);
            Transform.position = position;
            Transform.Rotate(Time.deltaTime * RotationSpeed * 0.1f, Time.deltaTime * RotationSpeed * 0.2f,
                Time.deltaTime * RotationSpeed);

            if ((_target - position).sqrMagnitude <= 0) StartRoute();
        }

        private void StartRoute()
        {
            var side = Random.Range(0, 4);
            var t = Random.value;
            var min = MovementBox.min;
            var max = MovementBox.max;
            var z = MovementBox.center.z;
            Vector3 start;
            Vector3 target;
            switch (side)
            {
                case 0:
                    start = new Vector3(Mathf.Lerp(min.x, max.x, t), max.y, z);
                    target = new Vector3(Mathf.Lerp(max.x, min.x, t), min.y, z);
                    break;
                case 1:
                    start = new Vector3(max.x, Mathf.Lerp(min.y, max.y, t), z);
                    target = new Vector3(min.x, Mathf.Lerp(max.y, min.y, t), z);
                    break;
                case 2:
                    start = new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, z);
                    target = new Vector3(Mathf.Lerp(max.x, min.x, t), max.y, z);
                    break;
                case 3:
                    start = new Vector3(min.x, Mathf.Lerp(min.y, max.y, t), z);
                    target = new Vector3(max.x, Mathf.Lerp(max.y, min.y, t), z);
                    break;
                default: return;
            }

            Transform.position = start;
            _target = target;
        }
    }
}