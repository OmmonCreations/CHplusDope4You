using UnityEngine;

namespace DopeElections.ScriptedSequences.JourneyToPlanet
{
    public class ApproachPlanetState : JourneyToPlanetCinematicState
    {
        private float AnimationTime { get; }
        private Transform Transform { get; }
        private Transform From { get; }
        private Transform To { get; }
        private AnimationCurve PositionCurve { get; }
        private AnimationCurve RotationCurve { get; }
        private AnimationCurve ScaleCurve { get; }

        private float _animationTime;
        private float _t;

        public ApproachPlanetState(JourneyToPlanetCinematicController controller, float time) : base(controller)
        {
            AnimationTime = time;
            Transform = controller.HotAirBalloon.transform;
            From = controller.ApproachFromAnchor;
            To = controller.ApproachToAnchor;
            PositionCurve = controller.ApproachPositionCurve;
            RotationCurve = controller.ApproachRotationCurve;
            ScaleCurve = controller.ApproachScaleCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Planet.Show();
            _animationTime = AnimationTime * (Controller.FlybySkipped ? 0.5f : 1);
        }

        public override void Update()
        {
            _t += Time.deltaTime / _animationTime;
            var positionProgress = PositionCurve.Evaluate(_t);
            var rotationProgress = RotationCurve.Evaluate(_t);
            var scaleProgress = ScaleCurve.Evaluate(_t);
            Transform.position = Vector3.Lerp(From.position, To.position, positionProgress);
            Transform.rotation = Quaternion.Lerp(From.rotation, To.rotation, rotationProgress);
            Transform.localScale = Vector3.Lerp(From.localScale, To.localScale, scaleProgress);
            if (_t >= 1) IsCompleted = true;
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