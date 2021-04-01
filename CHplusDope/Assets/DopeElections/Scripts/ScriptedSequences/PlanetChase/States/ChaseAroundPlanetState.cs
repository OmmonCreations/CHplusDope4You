using UnityEngine;

namespace DopeElections.ScriptedSequences.PlanetChase
{
    public class ChaseAroundPlanetState : PlanetChaseCinematicControllerState
    {
        public const float AnimationTime = 5;

        private float _t;

        private int _progressStepCount;
        private int _currentProgressStep = -1;
        private bool _fadeOutStarted = false;

        public ChaseAroundPlanetState(PlanetChaseCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.Player.PlayIdleAnimation();
            Controller.HotAirBalloon.PlayMoveFastAnimation();
            foreach (var candidate in Controller.Candidates)
            {
                candidate.PlayRunningAnimation();
                candidate.MovementSpeed = 10;
            }

            _progressStepCount = Controller.ProgressSteps.Length;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.Clamp01(_t);
            Controller.HotAirBalloonPivot.rotation = Quaternion.Lerp(
                Controller.HotAirBalloonFrom.rotation,
                Controller.HotAirBalloonTo.rotation,
                Controller.PlayerMovementCurve.Evaluate(progress)
            );
            Controller.CandidatesPivot.rotation = Quaternion.Lerp(
                Controller.CandidatesFrom.rotation,
                Controller.CandidatesTo.rotation,
                Controller.CandidatesMovementCurve.Evaluate(progress)
            );
            var progressStepIndex =
                Mathf.Clamp(Mathf.FloorToInt(Controller.ProgressStepsCurve.Evaluate(progress) * _progressStepCount), 0,
                    _progressStepCount - 1);
            if (progressStepIndex > _currentProgressStep)
            {
                Controller.ProgressSteps[progressStepIndex].Show();
                _currentProgressStep = progressStepIndex;
            }

            if (_t * AnimationTime > 3 && !_fadeOutStarted)
            {
                _fadeOutStarted = true;
                Controller.BlackMask.FadeToBlack(2).Then(() => IsCompleted = true);
            }
        }
    }
}