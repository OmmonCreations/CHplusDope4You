using StateMachines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.MainMenus
{
    public class ExtraSettingsButton : MonoBehaviour
    {
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private RectTransform _bodyTransform = null;
        [SerializeField] private RectTransform _expandedBodyTransform = null;
        [SerializeField] private Button _button = null;
        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private float _smallSize = 116;
        [SerializeField] private float _largeSize = 1090;
        [SerializeField] private float _expandTime = 0.5f;
        [SerializeField] private float _collapseTime = 0.3f;
        [SerializeField] private AnimationCurve _expandCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _collapseCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private UnityEvent _onExpand = null;
        [SerializeField] private UnityEvent _onCollapse = null;

        private bool _expanded = false;

        private StateMachine StateMachine => _stateMachine;

        private void Awake()
        {
            _button.onClick.AddListener(Toggle);
            _closeBackground.onClick.AddListener(Collapse);
            _closeBackground.gameObject.SetActive(false);
            ApplySize(_smallSize);
            ApplyExpandedSize(_largeSize);
            
        }

        private void Update()
        {
            StateMachine.Run();
        }

        public void CollapseImmediate()
        {
            _expanded = false;
            _closeBackground.gameObject.SetActive(false);
            ApplySize(_smallSize);
        }

        public void Toggle()
        {
            if (_expanded) Collapse();
            else Expand();
        }

        public void Expand()
        {
            _expanded = true;
            _closeBackground.gameObject.SetActive(true);
            StateMachine.State = CreateTransition(0, 1, _expandTime, _expandCurve);
            _onExpand.Invoke();
        }

        public void Collapse()
        {
            _expanded = false;
            _closeBackground.gameObject.SetActive(false);
            StateMachine.State = CreateTransition(1, 0, _collapseTime, _collapseCurve);
            _onCollapse.Invoke();
        }

        private TransitionState CreateTransition(float a, float b, float time, AnimationCurve curve)
        {
            var from = _smallSize;
            var to = _largeSize;
            var transition = new TransitionState(time, 0, 1);
            transition.OnTransition += t =>
            {
                var progress = curve.Evaluate(a + (b - a) * t);
                ApplySize(Mathf.Lerp(from, to, progress));
            };
            transition.OnCompleted += () => ApplySize(Mathf.Lerp(from, to, b));
            return transition;
        }

        private void ApplySize(float size)
        {
            _bodyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            _bodyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }

        private void ApplyExpandedSize(float size)
        {
            _expandedBodyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            _expandedBodyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
    }
}