using Essentials;
using UnityEngine;

namespace DopeElections.ScriptedSequences.LandInMainMenu.States
{
    public class ApproachGroundState : LandInMainMenuSequenceState
    {
        private const float AnimationTime = 3;

        private float _t;

        public ApproachGroundState(LandInMainMenuCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;

            var progress = Mathf.Clamp01(_t);
            var hotAirBalloonProgress = Controller.HotAirBalloonMovementCurve.Evaluate(progress);
            Controller.HotAirBalloon.transform.Interpolate(Controller.HotAirBalloonFrom, Controller.HotAirBalloonTo, hotAirBalloonProgress);

            if (_t >= 1) IsCompleted = true;
        }
    }
}