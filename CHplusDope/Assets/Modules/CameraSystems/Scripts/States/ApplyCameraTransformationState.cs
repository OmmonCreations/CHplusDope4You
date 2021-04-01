using Essentials;
using UnityEngine;

namespace CameraSystems
{
    public class ApplyCameraTransformationState : CameraState
    {
        public delegate void TransitionEvent(float progress);

        public event TransitionEvent OnProgress;

        private readonly CameraTransformation _transformation;
        private float _time;

        private CameraTransformation _startTransformation;

        private float _t = 0;
        private AnimationCurve _animation;

        public ApplyCameraTransformationState(CameraSystem cameraSystem, CameraTransformation transformation,
            float time)
            : this(cameraSystem, transformation, time, AnimationCurve.EaseInOut(0, 0, 1, 1))
        {
        }

        public ApplyCameraTransformationState(CameraSystem cameraSystem, CameraTransformation transformation,
            float time,
            AnimationCurve animation) : base(cameraSystem)
        {
            _transformation = transformation;
            _time = time > 0 ? time : 0.000000000001f;
            _animation = animation;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startTransformation = CameraSystem.CurrentTransform;
        }

        private void OnTouchDown(Touch touch)
        {
            _time = Mathf.Min(_time, 0.2f);
        }

        public override void Update()
        {
            _t += Time.deltaTime / _time;
            var progress = _animation.Evaluate(Mathf.Clamp01(_t));
            CameraSystem.CurrentTransform = CameraTransformation.Lerp(_startTransformation, _transformation, progress);

            IsCompleted = IsCompleted || _t >= 1;

            if (OnProgress != null) OnProgress(progress);
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            if (OnProgress != null) OnProgress(1);
        }
    }
}