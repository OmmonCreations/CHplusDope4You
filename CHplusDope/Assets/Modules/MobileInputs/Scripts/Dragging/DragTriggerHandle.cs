using UnityEngine;
using UnityEngine.Events;

namespace MobileInputs.Dragging
{
    public class DragTriggerHandle : DragHandle
    {
        public delegate void TriggerEvent();

        public event TriggerEvent Triggered = delegate { };
        public event TriggerEvent Cancelled = delegate { };

        [SerializeField] private Vector2 _direction = Vector2.up;
        [SerializeField] private float _threshold = 0;
        [SerializeField] private bool _useMaxDistance = false;
        [SerializeField] private float _maxDistance = -1;
        [SerializeField] private bool _resetWhenTriggered = true;
        [SerializeField] private bool _triggerOnMaxDistance = true;
        [SerializeField] private UnityEvent _onTriggered = new UnityEvent();
        [SerializeField] private UnityEvent _onCancelled = new UnityEvent();

        public UnityEvent onTriggered => _onTriggered;
        public UnityEvent onCancelled => _onCancelled;

        private float _distance;
        private bool _triggered;

        protected override void OnResetState()
        {
            base.OnResetState();
            _distance = 0;
            _triggered = false;
        }

        protected override Vector2 ClampDelta(Vector2 delta)
        {
            var normalizedDirection = _direction.normalized;
            var alignment = Mathf.Clamp01(Vector2.Dot(delta.normalized, normalizedDirection));
            var distance = Mathf.Min(
                delta.magnitude,
                _useMaxDistance && !_triggerOnMaxDistance ? _maxDistance : float.MaxValue
            ) * alignment;
            _distance = distance;
            if (distance > _maxDistance && _triggerOnMaxDistance)
            {
                TriggerSuccess();
                CancelDrag();
            }
            return normalizedDirection * distance;
        }

        protected override void FinishDrag()
        {
            var triggered = _distance >= _threshold;
            if (triggered && !_triggered) TriggerSuccess();
            else if (!triggered && !_triggered) TriggerCancel();

            if (!triggered || _resetWhenTriggered) base.FinishDrag();
            _triggered = false;
        }

        public void TriggerSuccess()
        {
            _triggered = true;
            _onTriggered.Invoke();
            Triggered();
        }

        public void TriggerCancel()
        {
            _triggered = true;
            _onCancelled.Invoke();
            Cancelled();
        }
    }
}