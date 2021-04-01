namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public class PlayComicState : LandOnPlanetCinematicState
    {
        public PlayComicState(LandOnPlanetCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.BlackMask.BlockInteractions(false);
            Controller.ComicSequence.ThirdPartCompleted += () => IsCompleted = true;
        }

        public override void Update()
        {
        }
    }
}