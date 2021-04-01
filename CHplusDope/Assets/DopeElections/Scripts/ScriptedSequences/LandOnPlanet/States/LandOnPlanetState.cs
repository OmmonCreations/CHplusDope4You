using UnityEngine;

namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public class LandOnPlanetState : LandOnPlanetCinematicState
    {
        private float AnimationTime { get; }
        private Transform Transform { get; }
        private Transform From { get; }
        private Transform To { get; }
        private AnimationCurve PositionCurve { get; }
        private AnimationCurve RotationCurve { get; }
        private AnimationCurve ScaleCurve { get; }

        private float _animationTime;
        private float _comicFadeInTime;
        private float _t;
        private bool _comicFadeInStarted = false;

        public LandOnPlanetState(LandOnPlanetCinematicController controller, float time) : base(controller)
        {
            AnimationTime = time;
            Transform = controller.HotAirBalloon.transform;
            From = controller.LandFromAnchor;
            To = controller.LandToAnchor;
            PositionCurve = controller.LandPositionCurve;
            RotationCurve = controller.LandRotationCurve;
            ScaleCurve = controller.LandScaleCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _animationTime = AnimationTime;
            _comicFadeInTime = (AnimationTime - 2) / AnimationTime;

            Controller.CameraSystem.Transition(Controller.CameraTransformation, AnimationTime,
                Controller.CameraTransitionCurve);
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
            if (_t >= _comicFadeInTime && !_comicFadeInStarted)
            {
                _comicFadeInStarted = true;
                Controller.ComicSequence.Play(() => IsCompleted = true, () => Controller.Stop());
            }
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