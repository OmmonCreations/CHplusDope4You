using UnityEngine;

namespace Navigation
{
    public sealed class TemporaryOccupant : Occupant
    {
        public float OccupationStart { get; }
        public float OccupationEnd { get; }
        public INavigationAction Action { get; }

        public TemporaryOccupant(INavigationAgent agent, float start, float end, INavigationAction action) : base(agent)
        {
            OccupationStart = start;
            OccupationEnd = end;
            Action = action;
        }

        public override bool BlocksAt(float time) => time >= OccupationStart && time <= OccupationEnd;

        public override float GetRemainingBlockTime(float timestamp, float time)
        {
            return OccupationEnd <= timestamp || OccupationStart >= timestamp + time
                ? 0
                : Mathf.Max(OccupationEnd - timestamp, 0);
        }
    }
}