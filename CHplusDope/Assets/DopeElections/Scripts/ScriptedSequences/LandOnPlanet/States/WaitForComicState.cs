namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public class WaitForComicState : LandOnPlanetCinematicState
    {
        public WaitForComicState(LandOnPlanetCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (!Controller.ComicSequence.IsPlaying) IsCompleted = true;
            else Controller.ComicSequence.ThirdPartCompleted += OnComicSequenceCompleted;
        }

        public override void Update()
        {
            
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.ComicSequence.ThirdPartCompleted -= OnComicSequenceCompleted;
        }

        private void OnComicSequenceCompleted()
        {
            IsCompleted = true;
        }
    }
}