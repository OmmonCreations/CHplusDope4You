using System;
using StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatedObjects
{
    public abstract class ToggleableObjectController : AnimatedObjectController
    {
        public delegate void TransitionStartEvent(float time);

        public delegate void TransitionCompleteEvent();

        public event TransitionStartEvent Appears = delegate { };
        public event TransitionCompleteEvent Appeared = delegate { };
        public event TransitionStartEvent Disappears = delegate { };
        public event TransitionCompleteEvent Disappeared = delegate { };

        [SerializeField] private float _appearTime = 0.5f;
        [SerializeField] private float _disappearTime = 0.5f;
        [SerializeField] private AnimationCurve _appearCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _disappearCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private UnityEvent _onAppears = null;
        [SerializeField] private UnityEvent _onDisappears = null;
        [SerializeField] private UnityEvent _onAppeared = null;
        [SerializeField] private UnityEvent _onDisappeared = null;

        private bool _visible;

        protected virtual bool ControlGameObjectActiveState { get; } = true;

        public bool IsVisible => _visible;

        public float AppearTime
        {
            get => _appearTime;
            set => _appearTime = value;
        }

        public float DisappearTime
        {
            get => _disappearTime;
            set => _disappearTime = value;
        }

        public void ShowImmediate(bool show)
        {
            if (show) ShowImmediate();
            else HideImmediate();
        }

        public void ShowImmediate()
        {
            StateMachine.State = null;
            _visible = true;
            if (ControlGameObjectActiveState) gameObject.SetActive(true);
            OnShowImmediate();
            TriggerAppeared();
        }

        public void HideImmediate()
        {
            StateMachine.State = null;
            _visible = false;
            if (ControlGameObjectActiveState) gameObject.SetActive(false);
            OnHideImmediate();
            TriggerDisappeared();
        }

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerShow() => Show();

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerShow(float delay) => Show(delay);

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerHide() => Hide();

        /// <summary>
        /// For Unity inspector references, these must have a return type void
        /// </summary>
        public void TriggerHide(float delay) => Hide(delay);

        public TransitionState Show(bool show, float delay = 0)
        {
            if (show) return Show(delay);
            return Hide(delay);
        }

        public TransitionState Show(float delay = 0)
        {
            _visible = true;
            StateMachine.State = null;
            var transition = CreateTransitionState(0, 1, AppearTime, _appearCurve);
            if (delay > 0)
            {
                HideImmediate();
                if (ControlGameObjectActiveState) gameObject.SetActive(true);
                OnShow();
                StateMachine.State = new DelayedActionState(() => ExecuteShow(transition), delay);
            }
            else
            {
                ExecuteShow(transition);
                OnShow();
            }

            return transition;
        }

        public TransitionState Hide(float delay = 0)
        {
            _visible = false;
            StateMachine.State = null;
            var transition = CreateTransitionState(1, 0, DisappearTime, _disappearCurve);
            if (delay > 0)
            {
                ShowImmediate();
                if (ControlGameObjectActiveState) gameObject.SetActive(true);
                OnHide();
                StateMachine.State = new DelayedActionState(() => ExecuteHide(transition), delay);
            }
            else
            {
                ExecuteHide(transition);
                OnHide();
            }

            return transition;
        }

        private void ExecuteShow(TransitionState state)
        {
            if (ControlGameObjectActiveState) gameObject.SetActive(true);
            StateMachine.State = state;
            OnExecuteShow();
            Appears(AppearTime);
            _onAppears.Invoke();
        }

        private void ExecuteHide(TransitionState state)
        {
            if (ControlGameObjectActiveState) gameObject.SetActive(true);
            StateMachine.State = state;
            OnExecuteHide();
            Disappears(DisappearTime);
            _onDisappears.Invoke();
        }

        protected void TriggerAppeared()
        {
            Appeared();
            _onAppeared.Invoke();
        }

        protected void TriggerDisappeared()
        {
            Disappeared();
            _onDisappeared.Invoke();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnShowImmediate()
        {
        }

        protected virtual void OnHideImmediate()
        {
        }

        protected virtual void OnExecuteShow()
        {
        }

        protected virtual void OnExecuteHide()
        {
        }

        protected abstract TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve);
    }
}