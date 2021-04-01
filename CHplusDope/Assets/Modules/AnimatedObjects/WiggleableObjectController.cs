using StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatedObjects
{
    public abstract class WiggleableObjectController : AnimatedObjectController
    {
        public delegate void WiggleStartEvent(float time);

        public delegate void WiggleEndEvent();

        public event WiggleStartEvent WiggleStarted = delegate { };
        public event WiggleEndEvent WiggleEnded = delegate { };

        [SerializeField] private float _defaultWiggleTime = 0.5f;
        [SerializeField] private float _strength = 20;
        [SerializeField] private UnityEvent _onWiggleStarted = null;
        [SerializeField] private UnityEvent _onWiggleEnded = null;
        [SerializeField] private AnimationCurve _wiggleCurve = AnimationCurve.Constant(0, 1, 1);

        public float DefaultWiggleTime
        {
            get => _defaultWiggleTime;
            set => _defaultWiggleTime = value;
        }

        public float Strength
        {
            get => _strength;
            set => _strength = value;
        }

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerWiggleDelayed(float delay) => Wiggle(delay);

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerWiggle() => Wiggle(0);

        public State Wiggle() => Wiggle(_defaultWiggleTime, 0);
        
        public State Wiggle(float delay)
        {
            return Wiggle(DefaultWiggleTime, delay);
        }

        public State Wiggle(float time, float delay)
        {
            var transition = CreateWiggleState(time, _wiggleCurve);
            if (delay > 0)
            {
                StateMachine.State = new DelayedActionState(() => ExecuteWiggle(transition, time), delay);
            }
            else
            {
                ExecuteWiggle(transition, time);
            }

            return transition;
        }

        private void ExecuteWiggle(State state, float time)
        {
            WiggleStarted(time);
            StateMachine.State = state;
            OnWiggle();
            _onWiggleStarted.Invoke();
        }

        protected virtual void OnWiggle()
        {
        }

        protected void TriggerWiggleEnded()
        {
            WiggleEnded();
            _onWiggleEnded.Invoke();
        }

        protected abstract State CreateWiggleState(float time, AnimationCurve curve);
    }
}