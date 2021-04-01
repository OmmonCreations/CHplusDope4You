using Effects;
using UnityEngine;

namespace DopeElections.Races
{
    public class CameraShakeState : RaceCameraState
    {
        private Transform CameraTransform { get; }
        private Vector3 _lPosition; //localPosition
        private readonly Shake _shake;

        private float _cameraDistance;
        
        public CameraShakeState(RaceCameraController cameraController, Shake shake) : base(cameraController)
        {
            CameraTransform = cameraController.Camera.transform;
            _shake = shake;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var cameraPosition = CameraTransform.localPosition;
            _cameraDistance = Mathf.Abs(cameraPosition.z);
            _lPosition = cameraPosition;
        }

        public override void Update()
        {
            _shake.Run();
            var shakePosition = _shake.Position;
            CameraTransform.localPosition = _lPosition + shakePosition;
            IsCompleted = _shake.IsCompleted;
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            CameraTransform.localPosition = -Vector3.forward * _cameraDistance;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            CameraTransform.localPosition = -Vector3.forward * _cameraDistance;
        }
    }
}