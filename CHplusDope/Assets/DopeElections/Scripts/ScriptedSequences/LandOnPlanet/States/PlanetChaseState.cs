namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public class PlanetChaseState : LandOnPlanetCinematicState
    {
        public PlanetChaseState(LandOnPlanetCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlanetChase.Play(() => IsCompleted = true, Controller.Stop);
        }

        public override void Update()
        {
        }
    }
}