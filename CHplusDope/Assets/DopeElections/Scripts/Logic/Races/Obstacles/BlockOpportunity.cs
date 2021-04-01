using Essentials;
using UnityEngine;

namespace DopeElections.Races
{
    public readonly struct BlockOpportunity
    {
        public Vector2Int Position { get; }
        public MinMaxRange TimeRange { get; }

        public BlockOpportunity(Vector2Int position)
        {
            Position = position;
            TimeRange = new MinMaxRange(0, float.MaxValue);
        }
        
        public BlockOpportunity(Vector2Int position, MinMaxRange timeRange)
        {
            Position = position;
            TimeRange = timeRange;
        }
    }
}