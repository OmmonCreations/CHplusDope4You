using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace MobileInputs.Dragging
{
    [RequireComponent(typeof(RectTransform))]
    public class DragHandle : MonoBehaviour
    {
        [SerializeField] private RectTransform _draggable = null;
        [SerializeField] private Vector2 _magneticDelta = Vector2.zero;
        [SerializeField] private bool _resetAfterDrag = true;
        [SerializeField] private UnityEvent _onDragStarted = new UnityEvent();
        [SerializeField] private UnityEvent _onDragEnded = new UnityEvent();

        private InteractionSystem _interactionSystem;
        private Canvas _canvas;

        private bool _isDragOrigin;
        private bool _dragStarted;
        private Vector2 _startScreenPoint;
        private Vector2 _startPosition;

        private Vector2 _dragDelta;
        private Vector2 _currentMagneticDelta;

        private Vector2 _currentMagneticDeltaVelocity;

        protected Vector2 StartPosition => _startPosition;

        private void Awake()
        {
            _interactionSystem = FindObjectOfType<InteractionSystem>();
            _canvas = GetComponentInParent<Canvas>().rootCanvas;
            HookEvents();
            OnAwake();
        }

        private void Update()
        {
            if (!_isDragOrigin || !_dragStarted) return;
            _currentMagneticDelta = Vector2.SmoothDamp(_currentMagneticDelta, _magneticDelta,
                ref _currentMagneticDeltaVelocity, 0.1f);
            var clampedDelta = ClampDelta(_dragDelta + _currentMagneticDelta);
            if (!_isDragOrigin) return;
            var position = _startPosition + clampedDelta;
            _draggable.anchoredPosition = position;
        }

        private void OnDestroy()
        {
            ReleaseHooks();

            OnDestroyed();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnDestroyed()
        {
        }

        private void HookEvents()
        {
            if (_interactionSystem)
            {
                _interactionSystem.OnDragStart += OnDragStart;
                _interactionSystem.OnDrag += OnDrag;
                _interactionSystem.OnDrop += OnDrop;
                _interactionSystem.OnPointerUp += OnPointerUp;
            }
        }

        private void ReleaseHooks()
        {
            if (_interactionSystem)
            {
                _interactionSystem.OnDragStart -= OnDragStart;
                _interactionSystem.OnDrag -= OnDrag;
                _interactionSystem.OnDrop -= OnDrop;
                _interactionSystem.OnPointerUp -= OnPointerUp;
            }
        }

        public void StartDrag()
        {
            ResetState();
            _isDragOrigin = true;
        }

        private void OnDragStart(IInteractable dragged, InputAction.CallbackContext context)
        {
            if (!_isDragOrigin) return;
            var pointerInput = context.ReadValue<PointerInput>();
            _startScreenPoint = pointerInput.Position;
            _startPosition = _draggable ? _draggable.anchoredPosition : Vector2.zero;
            _dragDelta = Vector2.zero;
            _dragStarted = true;
            enabled = true;
            _onDragStarted.Invoke();
            OnDragStarted();
        }

        private void OnDrag(IInteractable dragged, InputAction.CallbackContext context)
        {
            if (!_isDragOrigin) return;
            var pointerInput = context.ReadValue<PointerInput>();
            _dragDelta = (pointerInput.Position - _startScreenPoint) * (_canvas ? _canvas.scaleFactor : 1);
        }

        private void OnDrop(IInteractable dragged, IDropTarget target, InputAction.CallbackContext context)
        {
            if (!_isDragOrigin) return;
            _isDragOrigin = false;
        }

        private void OnPointerUp(IInteractable target, InputAction.CallbackContext context)
        {
            if (!_isDragOrigin) return;
            TriggerDragEnd();
        }

        protected void CancelDrag()
        {
            TriggerDragEnd();
        }

        protected virtual Vector2 ClampDelta(Vector2 delta)
        {
            return delta;
        }

        protected virtual void OnDragStarted()
        {
        }

        private void TriggerDragEnd()
        {
            FinishDrag();
            _onDragEnded.Invoke();
            enabled = false;
            ResetState();
        }

        protected virtual void FinishDrag()
        {
            if (_resetAfterDrag) _draggable.anchoredPosition = _startPosition;
        }

        private void ResetState()
        {
            _isDragOrigin = false;
            _dragStarted = false;
            _currentMagneticDelta = Vector2.zero;
            _currentMagneticDeltaVelocity = Vector2.zero;
            OnResetState();
        }

        protected virtual void OnResetState()
        {
        }
    }
}