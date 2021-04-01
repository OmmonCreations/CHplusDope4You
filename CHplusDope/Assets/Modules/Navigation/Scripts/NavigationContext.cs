using UnityEngine;

namespace Navigation
{
    public class NavigationContext
    {
        public TileGridNavMesh NavMesh { get; }
        public PathCalculator Calculator { get; }
        public INavigationAgent Agent { get; }
        public INavigationQuery Query { get; }

        public Vector2Int From => Query.From;
        public Vector2Int To { get; set; }
        public float Timestamp => Query.Timestamp;
        public float DiagonalFactor => NavMesh.DiagonalFactor;

        public ITileContent[,] Tiles => NavMesh.Tiles;

        public NavigationContext(TileGridNavMesh navMesh, PathCalculator calculator, INavigationAgent agent,
            INavigationQuery query)
        {
            NavMesh = navMesh;
            Calculator = calculator;
            Agent = agent;
            To = query.To;
            Query = query;
        }
    }
}