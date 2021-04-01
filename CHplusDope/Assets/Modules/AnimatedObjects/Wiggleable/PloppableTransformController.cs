using StateMachines;
using UnityEngine;

namespace AnimatedObjects.Wiggleable
{
    public class PloppableTransformController : WiggleableObjectController
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Vector3 _direction = new Vector3(0, 0, 1);

        public Transform Transform => _transform;
        
        protected override State CreateWiggleState(float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            state.OnTransition += t =>
            {
                var size = curve.Evaluate(t) * Strength;
                Transform.localScale = Vector3.one + _direction * size;
            };
            state.OnCompleted += () =>
            {
                Transform.localScale = Vector3.one;
                TriggerWiggleEnded();
            };
            return state;
        }
    }
}