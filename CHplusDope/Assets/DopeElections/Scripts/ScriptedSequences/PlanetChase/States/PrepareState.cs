namespace DopeElections.ScriptedSequences.PlanetChase
{
    public class PrepareState : PlanetChaseCinematicControllerState
    {
        public PrepareState(PlanetChaseCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.Prepare();
            Controller.CameraSystem.Transition(Controller.CameraTo,
                ShowSpeechBubbleState.ReadTime + ChaseAroundPlanetState.AnimationTime,
                Controller.CameraMovementCurve);

            IsCompleted = true;
        }

        public override void Update()
        {
        }
    }
}