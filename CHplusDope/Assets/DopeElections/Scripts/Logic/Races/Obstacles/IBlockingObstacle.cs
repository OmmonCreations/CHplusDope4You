using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public interface IBlockingObstacle : ITileContent
    {
        bool TryGetBlockOpportunity(INavigationAgent agent, Vector2Int from, float fromTimestamp, out BlockOpportunity opportunity);

        INavigationAction GetBlockAction(INavigationAgent agent, Vector2Int position,
            float timestamp, float time);
    }
}