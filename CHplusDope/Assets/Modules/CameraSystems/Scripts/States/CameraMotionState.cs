using UnityEngine;

namespace CameraSystems
{
    public class CameraMotionState : CameraState
    {
        private CameraMotion Motion { get; }

        private float _t;
        private readonly float _percentageFadeIn;
        private readonly float _percentageFadeOut;
        
        public CameraMotionState(CameraSystem cameraSystem, CameraMotion motion) : base(cameraSystem)
        {
            Motion = motion;
            _percentageFadeIn = motion.FadeIn / motion.Time;
            _percentageFadeOut = motion.FadeOut / motion.Time;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CameraSystem.CurrentTransform = Motion.From;
            CameraSystem.FadeToBlack = _percentageFadeIn > 0 ? 1 : 0;
        }

        public override void Update()
        {
            _t += Time.deltaTime / Motion.Time;
            var progress = Motion.MotionCurve.Evaluate(Mathf.Clamp01(_t));
            var transformation = CameraTransformation.Lerp(Motion.From, Motion.To, progress);
            CameraSystem.CurrentTransform = transformation;

            var fadeToBlack = 0f;
            if (_percentageFadeIn > 0 && _t < _percentageFadeIn)
            {
                fadeToBlack = Mathf.Max(fadeToBlack, 1-Mathf.Clamp01(_t / _percentageFadeIn));
            }

            if (_percentageFadeOut > 0 && (1 - _t) < _percentageFadeOut)
            {
                fadeToBlack = Mathf.Max(fadeToBlack, 1-Mathf.Clamp01((1-_t) / _percentageFadeOut));
            }

            CameraSystem.FadeToBlack = fadeToBlack;
            
            IsCompleted = _t >= 1;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                IsCompleted = true;
            }
        }

        protected override void OnComplete()
        {
            CameraSystem.CurrentTransform = Motion.To;
            CameraSystem.FadeToBlack = _percentageFadeOut > 0 ? 1 : 0;
            base.OnComplete();
        }
    }
}