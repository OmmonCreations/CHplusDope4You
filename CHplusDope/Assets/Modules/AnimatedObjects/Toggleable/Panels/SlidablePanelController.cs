using StateMachines;
using UnityEngine;

namespace AnimatedObjects
{
    public class SlidablePanelController : ToggleablePanelController
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private Vector2 _disappearDelta = Vector2.zero;
        
        public RectTransform RectTransform => _rectTransform;
        
        public Vector2 DisappearDelta
        {
            get => _disappearDelta;
            set => _disappearDelta = value;
        }

        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            RectTransform.anchoredPosition = Vector2.zero;
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            RectTransform.anchoredPosition = _disappearDelta;
        }

        protected override void OnShow()
        {
            base.OnShow();
            RectTransform.anchoredPosition = _disappearDelta;
        }

        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var anchoredPosition = _rectTransform.anchoredPosition;
            var from = Vector2.Lerp(_disappearDelta, anchoredPosition, b);
            var to = Vector2.Lerp(Vector2.zero, anchoredPosition, a);
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                RectTransform.anchoredPosition = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                RectTransform.anchoredPosition = Vector2.Lerp(from, to, b);
                if (b >= 1) TriggerAppeared();
                else TriggerDisappeared();
            };
            return state;
        }
    }
}