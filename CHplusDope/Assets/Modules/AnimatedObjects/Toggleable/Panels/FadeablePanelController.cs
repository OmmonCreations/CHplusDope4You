using StateMachines;
using UnityEngine;

namespace AnimatedObjects
{
    public class FadeablePanelController : ToggleablePanelController
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        
        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            Alpha = 1;
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            Alpha = 0;
        }

        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var from = 0;
            var to = 1;
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                Alpha = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                Alpha = Mathf.Lerp(@from, to, b);
                if (b >= 1) TriggerAppeared();
                else
                {
                    gameObject.SetActive(false);
                    TriggerDisappeared();
                }
            };
            return state;
        }
    }
}