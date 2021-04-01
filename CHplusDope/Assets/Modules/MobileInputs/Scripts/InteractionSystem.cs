// #define INPUT_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using Essentials;
using MobileInputs.Common;
using MobileInputs.Dragging;
using MobileInputs.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MobileInputs
{
    public class InteractionSystem : MonoBehaviour
    {
        public delegate void InteractionEvent(IInteractable interactable, InputAction.CallbackContext context);

        public delegate void DropEvent(IInteractable interactable, IDropTarget dropTarget,
            InputAction.CallbackContext context);

        public event InteractionEvent OnPointerDown = delegate { };
        public event InteractionEvent OnPointerMove = delegate { };
        public event InteractionEvent OnPointerUp = delegate { };
        public event InteractionEvent OnTap = delegate { };

        public event InteractionEvent OnDragStart = delegate { };
        public event InteractionEvent OnDrag = delegate { };
        public event DropEvent OnDrop = delegate { };

        private static RaycastHit[] _hits = new RaycastHit[10];

        [SerializeField] private Camera _eventCamera = null;
        [SerializeField] private LayerMask _interactionMask = default;

        private PointerControls _pointerControls;

        private IInteractable _current;
        private Vector2 _startPosition;
        private bool _pressing = false;
        private bool _dragging = false;
        private bool _isUiInteraction = false;
        private InputAction.CallbackContext _lastContext;

        public bool IgnoreUI { get; set; }
        public InputAction.CallbackContext LastContext => _lastContext;

        public Camera EventCamera
        {
            get => _eventCamera;
            set => _eventCamera = value;
        }

        public IInteractable CurrentTarget => _current;
        public bool IsUiInteraction => _isUiInteraction;

        private void Awake()
        {
            _pointerControls = new PointerControls();
            _pointerControls.pointer.point.started += Input;
            _pointerControls.pointer.point.performed += Input;
            _pointerControls.pointer.point.canceled += Input;

            if (!_eventCamera)
            {
                _eventCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            if (_pointerControls != null) _pointerControls.Enable();
        }

        private void OnDisable()
        {
            if(_pressing) PointerUp(LastContext);
            if (_pointerControls != null) _pointerControls.Disable();
        }

        private void OnDestroy()
        {
            if (_pointerControls != null)
            {
                _pointerControls.pointer.point.started -= Input;
                _pointerControls.pointer.point.performed -= Input;
                _pointerControls.pointer.point.canceled -= Input;
                _pointerControls.Dispose();
            }
        }

        private void Input(InputAction.CallbackContext context)
        {
            var control = context.control;
            var device = control.device;

            var isMouseInput = device is Mouse;
            var isPenInput = !isMouseInput && device is Pen;

            _lastContext = context;
            // Read our current pointer values.
            var drag = context.ReadValue<PointerInput>();
            if (isMouseInput)
                drag.InputId = Helpers.LeftMouseInputId;
            else if (isPenInput)
                drag.InputId = Helpers.PenInputId;

            if (drag.Contact && !_pressing)
            {
                PointerDown(context);
            }
            else if (drag.Contact && _pressing)
            {
                PointerMove(context);
            }
            else if (_pressing)
            {
                PointerUp(context);
            }
        }

        private void PointerDown(InputAction.CallbackContext context)
        {
            _isUiInteraction = IsPointerOverUIObject(context);

            _startPosition = context.ReadValue<PointerInput>().Position;
            _current = (!_isUiInteraction || IgnoreUI) ? GetTarget<IInteractable>(_startPosition) : null;
            if (_current is IPointerDownListener l) l.OnPointerDown(context);
#if INPUT_DEBUG
            var targetName = (_current != null ? _current.GetType().Name : "-");
            Debug.Log($"Pointer Down ({_startPosition.x},{_startPosition.y})! " +
                      $"(Target: {targetName}, UI: {_isUiInteraction}, IgnoreUI: {IgnoreUI})");
#endif
            OnPointerDown(_current, context);
            _pressing = true;
        }

        private void PointerMove(InputAction.CallbackContext context)
        {
            var currentPosition = context.ReadValue<PointerInput>().Position;
            if (_current is IPointerMoveListener l) l.OnPointerMove(context);
            OnPointerMove(_current, context);
            if (!_dragging && (currentPosition - _startPosition).sqrMagnitude > 2500)
            {
                if (_current is IDragStartListener dragStartListener)
                {
                    dragStartListener.OnDragStart(context);
                }

                OnDragStart(_current, context);

                _dragging = true;
#if INPUT_DEBUG
                var targetName = (_current != null ? _current.GetType().Name : "-");
                Debug.Log($"Drag Start ({_startPosition.x},{_startPosition.y})! " +
                          $"(Target: {targetName}, UI: {_isUiInteraction}, IgnoreUI: {IgnoreUI})");
#endif
            }
            else if (_dragging)
            {
                if (_current is IDragListener dragListener) dragListener.OnDrag(context);
                OnDrag(_current, context);
            }
        }

        private void PointerUp(InputAction.CallbackContext context)
        {
            var current = _current;

            // Pointer Up
            if (current is IPointerUpListener l) l.OnPointerUp(context);
            OnPointerUp(current, context);
#if INPUT_DEBUG
            var targetName = (current != null ? current.GetType().Name : "-");
            Debug.Log($"Pointer Up ({_startPosition.x},{_startPosition.y})! " +
                      $"(Target: {targetName}, UI: {_isUiInteraction}, IgnoreUI: {IgnoreUI})");
#endif
            // Tap
            if (!_dragging)
            {
                if (current is ITapListener tapListener) tapListener.OnTap(context);
                OnTap(current, context);
#if INPUT_DEBUG
                Debug.Log($"Tap ({_startPosition.x},{_startPosition.y})! " +
                          $"(Target: {targetName}, UI: {_isUiInteraction}, IgnoreUI: {IgnoreUI})");
#endif
            }

            // Drag End (Drop)
            if (_dragging)
            {
                var assumedTarget = GetTarget(context.ReadValue<PointerInput>().Position, current as IDropTarget);

                var actualTarget = assumedTarget != null && assumedTarget.AllowDrop(current) ? assumedTarget : null;

                if (current is IDropListener dropListener)
                {
                    dropListener.OnDrop(context, actualTarget);
                }

                OnDrop(current, actualTarget, context);
#if INPUT_DEBUG
                var dropName = (actualTarget != null ? actualTarget.GetType().Name : null);
                Debug.Log($"Drop ({_startPosition.x},{_startPosition.y})! " +
                          $"(Dragged: {targetName}, Dropped on: {dropName}, UI: {_isUiInteraction}, IgnoreUI: {IgnoreUI})");
#endif
            }

            _current = null;
            _pressing = false;
            _dragging = false;
        }

        private T GetTarget<T>(Vector2 screenPosition, T ignoreTarget = default) where T : IInteractable
        {
            var ray = EventCamera.ScreenPointToRay(screenPosition);
            var hitCount = Physics.RaycastNonAlloc(ray, _hits, 1000000, _interactionMask,
                QueryTriggerInteraction.Ignore);
            if (hitCount == 0) return default;
            return _hits.Where(h => h.collider != null).Select(h => h.collider.gameObject.GetComponent<T>())
                .FirstOrDefault(i => !Equals(i, ignoreTarget));
        }

        private static bool IsPointerOverUIObject(InputAction.CallbackContext context)
        {
            var pointer = context.ReadValue<PointerInput>();
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current) {position = pointer.Position};
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void SetTarget(IInteractable interactable)
        {
            ClearTarget();
            _current = interactable;
            _pressing = true;
            _dragging = true;
            if (interactable is IDragStartListener dragStartListener) dragStartListener.OnDragStart(_lastContext);
        }

        public void ClearTarget()
        {
            var previous = _current;
            if (previous != null)
            {
                PointerUp(_lastContext);
            }
        }
    }
}