using DopeElections.Menus.Teams.SlotStates;
using MobileInputs.Dragging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.Candidates
{
    public class PrepareDragState : CandidateSlotState
    {
        // min distance passed by the pointer to begin dragging
        private const float MinDragDistance = 50;
        private const float MinDragDistanceSquared = MinDragDistance * MinDragDistance;
        private const float MaxHoldDistance = 10;
        private const float MaxHoldDistanceSquared = MaxHoldDistance * MaxHoldDistance;
        private const float DragDelay = 1f;

        private Vector2 _dragStartPosition;
        private Vector2 _currentDragDelta;
        private float _t;
        private bool _isDrag = false;

        public PrepareDragState(CandidateSlotController slot, Vector2 startPosition) : base(slot)
        {
            _dragStartPosition = startPosition;
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            var dragStartVector = _currentDragDelta;
            if (!Slot.DragHorizontal) dragStartVector.x = 0;
            if (!Slot.DragVertical) dragStartVector.y = 0;
            var currentPassedHoldDistance = _currentDragDelta.sqrMagnitude;
            var currentPassedDragDistance = dragStartVector.sqrMagnitude;
            if (
                (_t >= DragDelay && currentPassedHoldDistance < MaxHoldDistanceSquared) ||
                (currentPassedDragDistance > MinDragDistanceSquared)
            )
            {
                Slot.TriggerDragged();
                IsCompleted = true;
                return;
            }
        }

        private Vector2 GetPointerPosition()
        {
            var result = Pointer.current.position.ReadValue();
            return result;
        }

        internal void OnPointerMove(InputAction.CallbackContext context)
        {
            var pointer = context.ReadValue<PointerInput>();
            _currentDragDelta = pointer.Position - _dragStartPosition;
            var dragTriggerDelta = _currentDragDelta;
            var dragBlockDelta = Vector2.zero;
            if (!Slot.DragHorizontal)
            {
                dragBlockDelta.x = _currentDragDelta.x;
                dragTriggerDelta.x = 0;
            }

            if (!Slot.DragVertical)
            {
                dragBlockDelta.y = _currentDragDelta.y;
                dragTriggerDelta.y = 0;
            }
            if (!_isDrag && dragTriggerDelta.sqrMagnitude > MaxHoldDistanceSquared)
            {
                _isDrag = true;
                Slot.TriggerDragged();
                IsCompleted = true;
            }
            else if (!_isDrag && dragBlockDelta.sqrMagnitude > MaxHoldDistanceSquared)
            {
                Slot.StateMachine.State = null;
                IsCompleted = true;
            }
            // Debug.Log(pointer.Position);
        }

        internal void OnPointerUp(InputAction.CallbackContext context)
        {
            IsCompleted = true;
            if (_isDrag) return;
            
            var pointer = context.ReadValue<PointerInput>();
            _currentDragDelta = pointer.Position - _dragStartPosition;
            var pointerDistance = _currentDragDelta.sqrMagnitude;
            if (pointerDistance < MaxHoldDistanceSquared) Slot.TriggerTapped();

        }

        internal void OnPointerExit()
        {
            if (Slot.DragHorizontal && Mathf.Abs(_currentDragDelta.x) > MaxHoldDistance ||
                Slot.DragVertical && Mathf.Abs(_currentDragDelta.y) > MaxHoldDistance) Slot.TriggerDragged();
        }
    }
}