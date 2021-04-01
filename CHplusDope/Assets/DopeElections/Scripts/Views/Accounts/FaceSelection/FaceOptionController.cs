using Essentials;
using StateMachines;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DopeElections.Accounts
{
    public class FaceOptionController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public delegate void SelectEvent(NamespacedKey faceId);

        public event SelectEvent Selected = delegate { };

        [SerializeField] private string _faceId = null;
        [SerializeField] private RectTransform _bodyTransform = null;
        [SerializeField] private CanvasGroup _bodyGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private Vector2 _fadeDelta = new Vector2(0, 0);

        [Header("Animations")] [SerializeField]
        private AnimationCurve _tapAnimation = AnimationCurve.Linear(0, 1, 1, 1);

        public NamespacedKey FaceId => NamespacedKey.TryParse(_faceId, out var id) ? id : default;
        public RectTransform DropArea { get; set; }
        private StateMachine StateMachine => _stateMachine;

        private float _dragDistance;

        public float Alpha
        {
            get => _bodyGroup.alpha;
            set => _bodyGroup.alpha = value;
        }

        private void Update()
        {
            StateMachine.Run();
        }

        public void FadeIn(float time, float delay)
        {
            Alpha = 0;
            var idleState = new DelayedActionState(() => FadeIn(time), delay);
            StateMachine.State = idleState;
        }

        private void FadeIn(float time)
        {
            var transition = new TransitionState(time, 0, 1);
            transition.OnTransition += t =>
            {
                var progress = 1 - Mathf.Pow(1 - t, 2);
                Alpha = t;
                _bodyTransform.anchoredPosition = Vector2.Lerp(_fadeDelta, Vector2.zero, progress);
            };
            transition.OnCompleted += () =>
            {
                Alpha = 1;
                _bodyTransform.anchoredPosition = Vector2.zero;
            };
            StateMachine.State = transition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragDistance = 0;
            PlayTapAnimation();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragDistance += eventData.delta.magnitude;
            _bodyTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_dragDistance < 10 || PointerInDropArea())
            {
                Select();
            }

            _bodyTransform.anchoredPosition = Vector2.zero;
        }

        public void Select()
        {
            Selected(FaceId);
        }

        private bool PointerInDropArea()
        {
            var dropArea = DropArea;
            var pointer = Pointer.current.position.ReadValue();
            var rect = dropArea.rect;
            var dropAreaPoint =
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dropArea, pointer, Camera.current, out var p)
                    ? p
                    : new Vector2(-1, -1);
            return rect.Contains(dropAreaPoint);
        }

        private void PlayTapAnimation()
        {
            var animation = new TransitionState(0.2f, 0, 1);
            animation.OnTransition += t =>
            {
                var progress = _tapAnimation.Evaluate(t);
                _bodyTransform.localScale = Vector3.one * progress;
            };
            animation.OnFinished += () => _bodyTransform.localScale = Vector3.one;
            StateMachine.State = animation;
        }
    }
}