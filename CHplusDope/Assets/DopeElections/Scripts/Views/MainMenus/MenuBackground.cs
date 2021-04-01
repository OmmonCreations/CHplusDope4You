using StateMachines;
using UnityEngine;

namespace DopeElections.MainMenus
{
    public class MenuBackground : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private AnimationCurve _fadeInCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _fadeOutCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _sizeFadeInCurve = AnimationCurve.Linear(0, 1.2f, 1, 1f);
        [SerializeField] private AnimationCurve _sizeFadeOutCurve = AnimationCurve.Linear(0, 1, 1, 1.2f);

        private bool _initialized = false;
        private float _size = 1;
        private bool _show;

        public StateMachine StateMachine => _stateMachine;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public float Size
        {
            get => _size;
            set => ApplySize(value);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        public void Show() => Show(true);

        public void Hide() => Show(false);

        public void Show(bool show)
        {
            if (!_initialized)
            {
                _initialized = true;
                ShowImmediate(show);
                return;
            }

            if (show == _show) return;
            _show = show;

            var start = 0;
            var target = 1;
            var opacityCurve = show ? _fadeInCurve : _fadeOutCurve;
            var sizeCurve = show ? _sizeFadeInCurve : _sizeFadeOutCurve;
            var transition = new TransitionState(0.2f, start, target, 0);
            transition.OnTransition += progress =>
            {
                Alpha = opacityCurve.Evaluate(progress);
                Size = sizeCurve.Evaluate(progress);
            };
            if (!show) transition.OnCompleted += () => gameObject.SetActive(false);
            StateMachine.State = transition;
            gameObject.SetActive(true);
        }

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);

        public void ShowImmediate(bool show)
        {
            _show = show;
            Alpha = show ? 1 : 0;
            Size = 1;
            gameObject.SetActive(show);
        }

        private void ApplySize(float size)
        {
            _size = size;
            _transform.localScale = new Vector3(size, size, size);
        }
    }
}