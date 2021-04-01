using UnityEngine;

namespace CameraSystems
{
    public class CameraIdleState : CameraState
    {
        private const float SmoothTime = 2f;
        
        private const float XJitter = 10;
        private const float YJitter = 10;
        
        private CameraTransformation _original;

        private float _startX;
        private float _startY;
        
        private float _targetX;
        private float _targetY;

        private float _tX;
        private float _tY;

        private float _timeX;
        private float _timeY;
        
        public CameraIdleState(CameraSystem cameraSystem) : base(cameraSystem)
        {
            
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _original = CameraSystem.CurrentTransform;

            _targetX = _original.eulerAngles.x;
            _targetY = _original.eulerAngles.y;

            ChooseTargetX();
            ChooseTargetY();
        }

        public override void Update()
        {
            _tX += Time.deltaTime / _timeX;
            _tY += Time.deltaTime / _timeY;
            
            var current = CameraSystem.CurrentTransform;
            var currentZ = current.eulerAngles.z;

            var x = Mathf.SmoothStep(_startX, _targetX, _tX);
            var y = Mathf.SmoothStep(_startY, _targetY, _tY);

            var transform = current.Clone();
            transform.eulerAngles = new Vector3(x,y,currentZ);
            CameraSystem.CurrentTransform = transform;
            
            if(_tX>=1) ChooseTargetX();
            if(_tY>=1) ChooseTargetY();
        }

        private void ChooseTargetX()
        {
            _startX = _targetX;
            _targetX = _original.eulerAngles.x + Random.Range(-XJitter, XJitter);
            _timeX = Mathf.Max(Mathf.Abs(_targetX - _startX)*SmoothTime,1f);

            _tX = 0;
        }

        private void ChooseTargetY()
        {
            _startY = _targetY;
            _targetY = _original.eulerAngles.y + Random.Range(-YJitter, YJitter);
            _timeY = Mathf.Max(Mathf.Abs(_targetY - _startY)*SmoothTime,1f);

            _tY = 0;
        }
    }
}