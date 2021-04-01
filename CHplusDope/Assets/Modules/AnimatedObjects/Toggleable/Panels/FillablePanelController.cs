using StateMachines;
using UnityEngine;
using UnityEngine.UI;

namespace AnimatedObjects
{
    public class FillablePanelController : ToggleablePanelController
    {
        [SerializeField] private Image _image = null;

        protected override bool ControlGameObjectActiveState { get; } = false;

        public float FillAmount
        {
            get => _image.fillAmount;
            set => _image.fillAmount = value;
        }

        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            FillAmount = 1;
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            FillAmount = 0;
        }

        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var from = 0;
            var to = 1;
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                FillAmount = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                FillAmount = Mathf.Lerp(@from, to, b);
                if (b >= 1) TriggerAppeared();
                else
                {
                    TriggerDisappeared();
                }
            };
            return state;
        }
    }
}