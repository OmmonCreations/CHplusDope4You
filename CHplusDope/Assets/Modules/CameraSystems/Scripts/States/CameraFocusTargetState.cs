using StateMachines;
using UnityEngine;

namespace CameraSystems
{
    public class CameraFocusTargetState : CameraState, ITargetedTask
    {
        private float TransitionTime { get; set; }
        private const float TargetFOV = 30;
        private float TargetDistance { get; }
        private const float TargetRotationX = 35;

        public ISpatialTargetable Target { get; }

        private readonly Transform _anchor;
        private readonly Transform _pivot;
        private readonly Transform _cameraTransform;

        private readonly Vector3 _startPosition;
        private readonly Quaternion _startY;
        private readonly Quaternion _startX;
        private readonly float _startDistance;
        private readonly float _startFOV;

        private Vector3 _targetPosition;
        private Quaternion _targetY;
        private Quaternion _targetX;
        private float _targetDistance;
        private float _targetFOV;

        private float _t = 0;

        public CameraFocusTargetState(CameraSystem cameraSystem, IFocusable target) : this(cameraSystem, target,
            target.PreferredFocusDistance, 1)
        {
        }

        public CameraFocusTargetState(CameraSystem cameraSystem, IFocusable target, float distance) : this(cameraSystem,
            target, distance, 1)
        {
        }

        public CameraFocusTargetState(CameraSystem cameraSystem, IFocusable target, float distance, float time) : base(
            cameraSystem)
        {
            TransitionTime = time;
            TargetDistance = distance >= 0 ? distance : target.PreferredFocusDistance;

            Target = target;

            _anchor = cameraSystem.Anchor;
            _pivot = cameraSystem.Pivot;
            _cameraTransform = cameraSystem.CameraTransform;

            _startPosition = _anchor.position;
            _startX = Quaternion.Euler(_pivot.localEulerAngles.x, 0, 0);
            _startY = Quaternion.Euler(0, _anchor.localEulerAngles.y, 0);
            _startDistance = -_cameraTransform.localPosition.z;
            _startFOV = cameraSystem.FieldOfView;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            ApplyPosition(1);
        }

        private void OnTouchDown(Touch touch)
        {
            TransitionTime = 0.2f;
        }

        public override void Update()
        {
            RecalculateTarget();

            _t += Time.deltaTime / TransitionTime;

            var progress = Mathf.SmoothStep(0, 1, _t);
            ApplyPosition(progress);

            if (_t >= 1) IsCompleted = true;
        }

        private void RecalculateTarget()
        {
            var targetEulerAngles = Target.Rotation.eulerAngles;
            _targetPosition = Target.Position + new Vector3(0, Target.Height / 2, 0);
            _targetY = Quaternion.Euler(0, targetEulerAngles.y + 180, 0);
            _targetX = Quaternion.Euler(TargetRotationX - targetEulerAngles.x, 0, 0);
            _targetDistance = TargetDistance;
            _targetFOV = TargetFOV;
        }

        private void ApplyPosition(float progress)
        {
            _anchor.position = Vector3.Lerp(_startPosition, _targetPosition, progress);
            _anchor.localRotation = Quaternion.Lerp(_startY, _targetY, progress);
            _pivot.localRotation = Quaternion.Lerp(_startX, _targetX, progress);
            _cameraTransform.localPosition = new Vector3(0, 0, -Mathf.Lerp(_startDistance, _targetDistance, progress));
            CameraSystem.FieldOfView = Mathf.Lerp(_startFOV, _targetFOV, progress);
        }
    }
}