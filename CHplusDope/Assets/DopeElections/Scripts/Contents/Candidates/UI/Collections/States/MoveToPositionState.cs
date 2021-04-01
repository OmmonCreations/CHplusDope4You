using DopeElections.Menus.Teams.SlotStates;
using UnityEngine;

namespace DopeElections.Candidates
{
    public class MoveToPositionState : CandidateSlotState
    {
        private const float MaxAccelerationDistance = 2500;
        private const float MaxAnimationTime = 1f;

        private RectTransform RectTransform { get; }
        private Vector2 TargetPosition { get; }
        private AnimationCurve Curve { get; }
        private float Delay { get; }

        private float _animationTime;
        private float _slotSize;
        private Vector2 _startVector;

        private float _t;

        public MoveToPositionState(CandidateSlotController slot,
            Vector2 position, float delay = 0)
            : this(slot, position, AnimationCurve.EaseInOut(0, 0, 1, 1), delay)
        {
        }

        public MoveToPositionState(CandidateSlotController slot,
            Vector2 position, AnimationCurve curve,
            float delay = 0) :
            base(slot)
        {
            RectTransform = slot.RectTransform;
            TargetPosition = position;
            Curve = curve;
            Delay = delay;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var distance = (TargetPosition - _startVector).magnitude;
            _slotSize = Slot.SlotSize;
            _startVector = RectTransform.anchoredPosition;
            _animationTime = Mathf.Max(
                0.2f,
                Mathf.Pow(Mathf.Clamp01(distance / MaxAccelerationDistance), 1.5f) * MaxAnimationTime
            );
            _t = -Delay / _animationTime;
        }

        public override void Update()
        {
            _t += Time.deltaTime / _animationTime;
            var progress = Curve.Evaluate(Mathf.Clamp01(_t));
            var vector = Vector2.Lerp(_startVector, TargetPosition, progress);
            RectTransform.anchoredPosition = vector;
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RectTransform.anchoredPosition = TargetPosition;
        }
    }
}