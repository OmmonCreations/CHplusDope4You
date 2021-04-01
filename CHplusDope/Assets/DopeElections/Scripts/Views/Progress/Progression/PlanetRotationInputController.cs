using MobileInputs;
using MobileInputs.Dragging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.Progress
{
    public class PlanetRotationInputController : MonoBehaviour, IInteractable
    {
        [Header("Prefab References")] [SerializeField]
        private PlanetProgressController _progressController = null;

        [SerializeField] private float _sensitivity = 1f;

        [Header("Scene References")] [SerializeField]
        private InteractionSystem _interactionSystem = null;

        private bool _dragging = false;
        private float _startRotation;
        private float _currentRotation;
        private float _targetRotation;
        private float _velocity;

        private Vector2 _dragStartScreenPoint;

        public InteractionSystem InteractionSystem => _interactionSystem;
        private PlanetProgressController ProgressController => _progressController;
        public float MinAngle => _progressController.FromRotationAngle;
        public float MaxAngle => _progressController.ToRotationAngle;

        private bool _canMove = true;

        private void Start()
        {
            HookEvents();
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        private void Update()
        {
            if (_canMove && Mathf.Abs(_targetRotation - _currentRotation) > 0)
            {
                var normalizedRotation = Mathf.SmoothDamp(_currentRotation, _targetRotation, ref _velocity, 0.2f);
                ProgressController.SetRotation(normalizedRotation);
                _currentRotation = normalizedRotation;
            }
        }

        private void HookEvents()
        {
            if (InteractionSystem)
            {
                InteractionSystem.OnPointerDown += OnPointerDown;
                InteractionSystem.OnDragStart += OnDragStart;
                InteractionSystem.OnDrag += OnDrag;
            }
        }

        private void ReleaseHooks()
        {
            if (InteractionSystem)
            {
                InteractionSystem.OnPointerDown -= OnPointerDown;
                InteractionSystem.OnDragStart -= OnDragStart;
                InteractionSystem.OnDrag -= OnDrag;
            }
        }

        public void EnableMovement()
        {
            _canMove = true;
        }

        public void DisableMovement()
        {
            _startRotation = _currentRotation;
            _targetRotation = _currentRotation;
            _velocity = 0;
            _canMove = false;
        }

        public void JumpTo(float normalizedPosition)
        {
            _startRotation = normalizedPosition;
            _currentRotation = normalizedPosition;
            _targetRotation = normalizedPosition;
            ProgressController.SetRotation(normalizedPosition);
        }

        public void GoTo(float normalizedPosition)
        {
            _targetRotation = normalizedPosition;
        }

        public void OnPointerDown(IInteractable interactable, InputAction.CallbackContext context)
        {
            if (interactable == null)
            {
                _dragging = false;
                return;
            }

            _dragging = true;
            var input = context.ReadValue<PointerInput>();
            _dragStartScreenPoint = input.Position;
            _startRotation = ProgressController.NormalizedRotation;
        }

        public void OnDragStart(IInteractable interactable, InputAction.CallbackContext context)
        {
            if (!_dragging) return;
            var input = context.ReadValue<PointerInput>();
            var delta = input.Position - _dragStartScreenPoint;
            UpdateTargetRotation(delta);
        }

        public void OnDrag(IInteractable interactable, InputAction.CallbackContext context)
        {
            if (!_dragging) return;
            var input = context.ReadValue<PointerInput>();
            var delta = input.Position - _dragStartScreenPoint;
            UpdateTargetRotation(delta);
        }

        private void UpdateTargetRotation(Vector2 delta)
        {
            var xDelta = delta.x * (_sensitivity / Screen.width);
            var angle = _startRotation + xDelta;

            var clampedInputRotation = Mathf.Clamp01(angle);
            _targetRotation = clampedInputRotation;
        }
    }
}