using UnityEngine;

namespace DopeElections.Races
{
    public class ExpandOverlayState : RaceOverlayState
    {
        private const float AnimationTime = 0.5f;

        private float StartY { get; }
        private float TargetY { get; }
        private AnimationCurve PositionCurve { get; }

        private float _t;

        public ExpandOverlayState(RaceOverlayController controller) : base(controller)
        {
            StartY = controller.ContractedY;
            TargetY = controller.ExpandedY;
            PositionCurve = controller.ExpandPositionCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Overlay.Alpha = 1;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = PositionCurve.Evaluate(_t);
            Overlay.RectTransform.anchoredPosition = new Vector2(0, StartY + (TargetY - StartY) * progress);
            // Overlay.Alpha = Mathf.Lerp(0, 1, _t);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Overlay.RectTransform.anchoredPosition = new Vector2(0, TargetY);
            Overlay.Alpha = 1;
            Overlay.Interactable = true;
        }
    }
}