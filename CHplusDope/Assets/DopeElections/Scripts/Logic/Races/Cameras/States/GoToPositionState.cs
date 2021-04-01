using CameraSystems;
using UnityEngine;

namespace DopeElections.Races
{
    public class GoToPositionState : RaceCameraState
    {
        private float Position { get; }
        private float TransitionTime { get; }
        private AnimationCurve AnimationCurve { get; }

        private float _startPosition;
        private float _t;

        public GoToPositionState(RaceCameraController controller, float position, float time, AnimationCurve curve) :
            base(controller)
        {
            Position = position;
            TransitionTime = time;
            AnimationCurve = curve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = CameraController.CurrentPosition;
        }

        public override void Update()
        {
            _t += Time.deltaTime / TransitionTime;
            var progress = AnimationCurve.Evaluate(Mathf.Clamp01(_t));
            CameraController.TargetPosition = Mathf.Lerp(_startPosition, Position, progress);
            IsCompleted |= _t >= 1;
        }

        public static GoToPositionState Linear(RaceCameraController controller, float position, float time)
        {
            return new GoToPositionState(controller, position, time, AnimationCurve.Linear(0, 0, 1, 1));
        }

        public static GoToPositionState EaseOut(RaceCameraController controller, float position, float time)
        {
            return new GoToPositionState(controller, position, time,
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 2), new Keyframe(1, 1, 0, 0)));
        }

        public static GoToPositionState JumpOut(RaceCameraController controller, float position, float time)
        {
            return new GoToPositionState(controller, position, time,
                new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 2), new Keyframe(1, 1, 0, 0)));
        }

        public static GoToPositionState EaseIn(RaceCameraController controller, float position, float time)
        {
            return new GoToPositionState(controller, position, time,
                new AnimationCurve(new Keyframe(0f, 0f, 0f, 0), new Keyframe(1, 1, -2, 0)));
        }

        public static GoToPositionState EaseInOut(RaceCameraController controller, float position, float time)
        {
            return new GoToPositionState(controller, position, time, AnimationCurve.EaseInOut(0, 0, 1, 1));
        }
    }
}