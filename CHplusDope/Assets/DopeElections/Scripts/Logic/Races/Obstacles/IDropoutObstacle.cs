using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public interface IDropoutObstacle
    {
        bool TryGetDropoutOpportunity(INavigationAgent agent, Vector2Int from, float fromTimestamp, out BlockOpportunity opportunity);

        INavigationAction GetDropoutAction(INavigationAgent agent, Vector2Int position,
            float timestamp, float time);
    }
}