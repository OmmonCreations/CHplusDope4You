using Essentials;
using UnityEngine;

namespace DopeElections.ScriptedSequences.LandInMainMenu.States
{
    public class JumpOnPedestalState : LandInMainMenuSequenceState
    {
        private const float AnimationTime = 0.6f;

        private float _t;
        
        public JumpOnPedestalState(LandInMainMenuCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.Player.transform.SetParent(Controller.PlayerTo, true);
            Controller.Player.PlayJumpAnimation(AnimationTime);
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;

            var progress = Mathf.Clamp01(_t);
            var playerProgress = Controller.PlayerMovementCurve.Evaluate(progress);
            var jumpHeight = Controller.PlayerJumpCurve.Evaluate(progress);
            Controller.Player.transform.Interpolate(Controller.HotAirBalloon.CharacterAnchor, Controller.PlayerTo, playerProgress);
            Controller.Player.transform.position += new Vector3(0, jumpHeight);

            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.Player.transform.Reset();
        }
    }
}