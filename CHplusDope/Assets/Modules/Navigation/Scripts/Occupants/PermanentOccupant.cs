namespace Navigation
{
    public sealed class PermanentOccupant : Occupant
    {
        public PermanentOccupant(INavigationAgent agent) : base(agent)
        {
        }

        public override bool BlocksAt(float time) => true;

        public override float GetRemainingBlockTime(float timestamp, float time) => float.MaxValue;
    }
}