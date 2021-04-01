using UnityEngine;

namespace Navigation
{
    public static class Directions
    {
        public static readonly Vector2Int[] All =
        {
            new Vector2Int(1, 0), // east
            new Vector2Int(1, -1), // south east
            new Vector2Int(0, -1), // south
            new Vector2Int(-1, -1), // south west
            new Vector2Int(-1, 0), // west
            new Vector2Int(-1, 1), // north west
            new Vector2Int(0, 1), // north
            new Vector2Int(1, 1), // north east
        };

        public static readonly Vector2Int[] Cardinal =
        {
            new Vector2Int(1, 0), // east
            new Vector2Int(0, -1), // south
            new Vector2Int(-1, 0), // west
            new Vector2Int(0, 1), // north
        };

        public static readonly Vector2Int[] Diagonal =
        {
            new Vector2Int(1, 1), // north east
            new Vector2Int(1, -1), // south east
            new Vector2Int(-1, -1), // south west
            new Vector2Int(-1, 1), // north west
        };
    }
}