using StateMachines;
using UnityEngine;

namespace AnimatedObjects.Transforms
{
    public class PoppableTransformController : ToggleableTransformController
    {
        protected virtual Vector3 FromSize { get; } = Vector3.zero;
        protected virtual Vector3 ToSize { get; } = Vector3.one;
        
        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            Transform.localScale = ToSize;
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            Transform.localScale = FromSize;
        }

        protected override void OnShow()
        {
            base.OnShow();
            Transform.localScale = FromSize;
        }

        protected override void OnExecuteShow()
        {
            base.OnExecuteShow();
            Transform.localScale = FromSize;
        }

        protected override void OnExecuteHide()
        {
            base.OnExecuteHide();
            Transform.localScale = ToSize;
        }

        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var from = FromSize;
            var to = ToSize;
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                Transform.localScale = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                Transform.localScale = Vector3.Lerp(@from, to, b);
                if (b >= 1) TriggerAppeared();
                else
                {
                    if(ControlGameObjectActiveState) gameObject.SetActive(false);
                    TriggerDisappeared();
                }
            };
            return state;
        }
    }
}