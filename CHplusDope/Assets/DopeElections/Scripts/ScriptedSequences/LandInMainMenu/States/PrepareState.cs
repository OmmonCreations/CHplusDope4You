using Essentials;

namespace DopeElections.ScriptedSequences.LandInMainMenu.States
{
    public class PrepareState : LandInMainMenuSequenceState
    {
        public PrepareState(LandInMainMenuCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.BlackMask.FadeToClear(1);
            
            Controller.HotAirBalloon.gameObject.SetActive(true);
            
            Controller.Player.transform.SetParent(Controller.HotAirBalloon.CharacterAnchor, false);
            Controller.Player.transform.Reset();
            Controller.Player.PlayIdleAnimation();

            Controller.HotAirBalloon.transform.SetParent(Controller.HotAirBalloonAnchor, false);
            Controller.HotAirBalloon.transform.Reset();
            Controller.HotAirBalloon.transform.Copy(Controller.HotAirBalloonFrom);

            Controller.CameraSystem.CurrentTransform = Controller.CameraFrom;
            Controller.CameraSystem.Transition(Controller.CameraTo,
                7,
                Controller.CameraMovementCurve);

            Controller.BlackMask.FadeToClear(1);

            IsCompleted = true;
        }

        public override void Update()
        {
            
        }
    }
}