using UnityEngine;

namespace DopeElections.ScriptedSequences.JourneyToPlanet
{
    public class FlybyState : JourneyToPlanetCinematicState
    {
        private float AnimationTime { get; }
        private Transform Transform { get; }
        private Transform From { get; }
        private Transform To { get; }
        private AnimationCurve PositionCurve { get; }
        private AnimationCurve RotationCurve { get; }
        private AnimationCurve ScaleCurve { get; }

        private Quaternion _fromRotation;
        private Quaternion _toRotation;

        private float _t;

        public override SkipInputType SkipInputType { get; } = SkipInputType.TapAnywhere;
        public override SkipRange SkipRange { get; } = SkipRange.Section;

        public FlybyState(JourneyToPlanetCinematicController controller, float time) : base(controller)
        {
            AnimationTime = time;
            Transform = controller.HotAirBalloon.transform;
            From = controller.FlybyFromAnchor;
            To = controller.FlybyToAnchor;
            PositionCurve = controller.FlybyPositionCurve;
            RotationCurve = controller.FlybyRotationCurve;
            ScaleCurve = controller.FlybyScaleCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Balloon.PlaySwayAnimation();
            Player.PlayIdleAnimation();

            _fromRotation = From.rotation;
            _toRotation = To.rotation;

            var balloonTransform = Balloon.transform;
            balloonTransform.position = From.position;
            balloonTransform.rotation = _fromRotation;
            balloonTransform.localScale = From.localScale;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var positionProgress = PositionCurve.Evaluate(_t);
            var rotationProgress = RotationCurve.Evaluate(_t);
            var scaleProgress = ScaleCurve.Evaluate(_t);
            Transform.position = Vector3.Lerp(From.position, To.position, positionProgress);
            Transform.rotation = Quaternion.LerpUnclamped(_fromRotation, _toRotation, rotationProgress);
            Transform.localScale = Vector3.Lerp(From.localScale, To.localScale, scaleProgress);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnSkip()
        {
            base.OnSkip();
            Controller.FlybySkipped = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Transform.position = To.position;
            Transform.rotation = To.rotation;
            Transform.localScale = To.localScale;
        }
    }
}