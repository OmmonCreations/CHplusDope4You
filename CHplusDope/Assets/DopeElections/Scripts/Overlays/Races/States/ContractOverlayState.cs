using UnityEngine;

namespace DopeElections.Races
{
    public class ContractOverlayState : RaceOverlayState
    {
        private const float AnimationTime = 0.5f;
        
        private float StartY { get; }
        private float TargetY { get; }
        private AnimationCurve PositionCurve { get; }

        private float _t;
        
        public ContractOverlayState(RaceOverlayController controller) : base(controller)
        {
            StartY = controller.ExpandedY;
            TargetY = controller.ContractedY;
            PositionCurve = controller.ContractPositionCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Overlay.Interactable = false;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = PositionCurve.Evaluate(_t);
            Overlay.RectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(StartY, TargetY, progress));
            // Overlay.Alpha = Mathf.Lerp(1, 0, _t);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Overlay.RectTransform.anchoredPosition = new Vector2(0, TargetY);
            Overlay.Alpha = 0;
            Overlay.RectTransform.gameObject.SetActive(false);
        }
    }
}