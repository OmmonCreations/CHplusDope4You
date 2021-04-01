namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public class PrepareState : LandOnPlanetCinematicState
    {
        public PrepareState(LandOnPlanetCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.EnvironmentAnchor.gameObject.SetActive(true);
            Controller.Planet.CloudLayer.Hide();
            IsCompleted = true;
        }

        public override void Update()
        {
            
        }
    }
}