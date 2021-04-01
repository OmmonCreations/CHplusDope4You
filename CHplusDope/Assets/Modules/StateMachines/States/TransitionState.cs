using UnityEngine;

namespace StateMachines
{
    public class TransitionState : State
    {
        public delegate void TransitionEvent(float t);

        public event TransitionEvent OnTransition;

        private AnimationCurve curve;
        private float t;
        private readonly float start;
        private readonly float target;
        private readonly float speed;

        public TransitionState(float time, float start, float target, float start_t = 0) : this(time, start, target,
            AnimationCurve.Linear(0, 0, 1, 1), start_t)
        {
        }

        public TransitionState(float time, float start, float target, AnimationCurve curve, float start_t = 0) : base()
        {
            this.start = start;
            this.target = target;
            this.curve = curve;
            speed = 1 / Mathf.Max(time, 0.0001f);
            t = start_t;
        }

        public override void Update()
        {
            IsCompleted |= t >= 1;
            t += Time.deltaTime * speed;
            if (OnTransition != null) OnTransition(Mathf.Lerp(start, target, curve.Evaluate(Mathf.Clamp01(t))));
        }

        protected override void OnComplete()
        {
            OnTransition(Mathf.Lerp(start, target, 1));
            base.OnComplete();
        }
    }
}