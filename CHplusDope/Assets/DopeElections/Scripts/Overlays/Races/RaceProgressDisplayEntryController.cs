using System;
using StateMachines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.Races
{
    public class RaceProgressDisplayEntryController : MonoBehaviour
    {
        [SerializeField] private RectTransform _completedStateRectTransform = null;
        [SerializeField] private CanvasGroup _completedStateGroup = null;
        [SerializeField] private CanvasGroup _activeStateGroup = null;
        [SerializeField] private LayoutElement _layoutElement = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private Button _button = null;

        [Header("Animations")] [SerializeField]
        private AnimationCurve _completeSizeCurve = AnimationCurve.Constant(0, 1, 1);

        [SerializeField] private AnimationCurve _completedAlphaCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private bool _active;
        private bool _completed;

        public float Width
        {
            get => _layoutElement.preferredWidth;
            set => _layoutElement.preferredWidth = value;
        }

        public bool IsActive => _active;
        public bool IsCompleted => _completed;

        private StateMachine StateMachine => _stateMachine;
        public UnityEvent onClick => _button.onClick;

        private void Update()
        {
            StateMachine.Run();
        }

        public void SetActive(bool active)
        {
            if (active == _active) return;
            if (!active)
            {
                SetActiveImmediate(false);
                return;
            }

            _active = true;

            _activeStateGroup.gameObject.SetActive(true);
            _activeStateGroup.alpha = 0;

            var transition = new TransitionState(0.2f, 0, 1);
            transition.OnTransition += t => { _activeStateGroup.alpha = t; };
            transition.OnFinished += () => { _activeStateGroup.alpha = 1; };
            StateMachine.State = transition;
        }

        public void SetCompleted(bool completed, Action callback = null)
        {
            if (completed == _completed) return;
            if (!completed)
            {
                SetCompletedImmediate(false);
                if (callback != null) callback();
                return;
            }

            _completed = true;
            
            _completedStateGroup.gameObject.SetActive(true);
            _completedStateGroup.alpha = 0;
            _completedStateRectTransform.localScale = Vector3.one * _completeSizeCurve.Evaluate(0);

            var transition = new TransitionState(0.3f, 0, 1);
            transition.OnTransition += t =>
            {
                var size = _completeSizeCurve.Evaluate(t);
                var alpha = _completedAlphaCurve.Evaluate(t);
                _completedStateRectTransform.localScale = new Vector3(size, size, size);
                _completedStateGroup.alpha = alpha;
            };
            transition.OnFinished += () =>
            {
                _completedStateGroup.alpha = 1;
                _completedStateRectTransform.localScale = Vector3.one;
            };
            if (callback != null) transition.OnFinished += () => callback();

            StateMachine.State = transition;
        }

        public void SetActiveImmediate(bool active)
        {
            _active = active;
            _activeStateGroup.alpha = active ? 1 : 0;
            _activeStateGroup.gameObject.SetActive(active);
        }

        public void SetCompletedImmediate(bool completed)
        {
            _completed = completed;
            _completedStateGroup.gameObject.SetActive(completed);
            _completedStateGroup.alpha = completed ? 1 : 0;
            _completedStateRectTransform.localScale = Vector3.one;
        }

        public void Remove() => Destroy(gameObject);
    }
}