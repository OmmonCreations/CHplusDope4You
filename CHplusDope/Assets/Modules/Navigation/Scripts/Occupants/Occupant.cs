namespace Navigation
{
    public abstract class Occupant
    {
        public INavigationAgent Agent { get; }

        protected Occupant(INavigationAgent agent)
        {
            Agent = agent;
        }
        
        public abstract bool BlocksAt(float timestamp);
        public abstract float GetRemainingBlockTime(float timestamp, float time);
    }
}