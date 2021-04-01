using StateMachines;
using UnityEngine;

namespace AnimatedObjects.Wiggleable
{
    public class WiggleableTransformController : WiggleableObjectController
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Vector3 _direction = new Vector3(0, 0, 1);

        public Transform Transform => _transform;
        
        protected override State CreateWiggleState(float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            state.OnTransition += t =>
            {
                var angle = curve.Evaluate(t) * Strength;
                Transform.localEulerAngles = _direction * angle;
            };
            state.OnCompleted += () =>
            {
                Transform.localEulerAngles = Vector3.zero;
                TriggerWiggleEnded();
            };
            return state;
        }
    }
}