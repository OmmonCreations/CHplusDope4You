using StateMachines;
using UnityEngine;

namespace AnimatedObjects
{
    public class PoppablePanelController : ToggleablePanelController
    {
        [SerializeField] private RectTransform _rectTransform = null;
        
        
        protected virtual Vector3 FromSize { get; } = Vector3.zero;
        protected virtual Vector3 ToSize { get; } = Vector3.one;
        protected override bool ControlGameObjectActiveState { get; } = true;
        
        public RectTransform RectTransform => _rectTransform;
        
        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            RectTransform.localScale = Vector3.one;
            if(ControlGameObjectActiveState) gameObject.SetActive(true);
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            RectTransform.localScale = Vector3.zero;
            if(ControlGameObjectActiveState) gameObject.SetActive(false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            RectTransform.localScale = FromSize;
        }

        protected override void OnExecuteShow()
        {
            base.OnExecuteShow();
            RectTransform.localScale = FromSize;
        }

        protected override void OnExecuteHide()
        {
            base.OnExecuteHide();
            RectTransform.localScale = ToSize;
        }


        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var from = FromSize;
            var to = ToSize;
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                RectTransform.localScale = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                RectTransform.localScale = Vector3.Lerp(@from, to, b);
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