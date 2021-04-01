using UnityEngine;

namespace CameraSystems
{
    [System.Serializable]
    public class CameraMotion
    {
        private const float DefaultTime = 5f;

        [SerializeField] private string _name = "unnamed motion";
        [SerializeField] private CameraTransformation _from;
        [SerializeField] private CameraTransformation _to;
        [SerializeField] private AnimationCurve _motionCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private float _time = 5f;
        [SerializeField] private float _fadeIn = 0.5f;
        [SerializeField] private float _fadeOut = 0.5f;
        
        public string Name
        {
            get { return _name; }
        }
        
        public CameraTransformation From
        {
            get { return _from; }
        }
        public CameraTransformation To
        {
            get { return _to; }
        }
        public AnimationCurve MotionCurve
        {
            get { return _motionCurve; }
        }
        public float Time
        {
            get { return _time; }
        }
        public float FadeIn
        {
            get { return _fadeIn; }
        }
        public float FadeOut
        {
            get { return _fadeOut; }
        }

        public CameraMotion(CameraTransformation from, CameraTransformation to, float time = DefaultTime) : this(from, to,
            AnimationCurve.Linear(0, 0, 1, 1), time) {}
        
        public CameraMotion(CameraTransformation from, CameraTransformation to, AnimationCurve curve, float time = DefaultTime) : this(from, to,
            curve, time, 0, 0) { }
        
        public CameraMotion(CameraTransformation from, CameraTransformation to, float time, float fadeIn, float fadeOut) : this(from, to,
            AnimationCurve.Linear(0, 0, 1, 1), time, fadeIn, fadeOut) {}
        public CameraMotion(CameraTransformation from, CameraTransformation to, AnimationCurve curve, float fadeIn, float fadeOut) : this(from, to,
            curve, DefaultTime, fadeIn, fadeOut) {}

        public CameraMotion(CameraTransformation from, CameraTransformation to, AnimationCurve curve, float time, float fadeIn,
            float fadeOut)
        {
            _from = from;
            _to = to;
            _motionCurve = curve;
            _time = time;
            _fadeIn = fadeIn;
            _fadeOut = fadeOut;
        }

    }
}