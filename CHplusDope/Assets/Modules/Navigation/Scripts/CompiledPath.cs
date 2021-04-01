using System.Linq;
using UnityEngine;

namespace Navigation
{
    public class CompiledPath
    {
        public TileGridNavMesh NavMesh { get; }
        public Vector2Int[] Points { get; }
        public ITileContent[] Tiles { get; }
        public float[] Timestamps { get; }
        public INavigationAction[] Actions { get; }

        public float Timestamp { get; }
        public float Time { get; }

        public CompiledPath(TileGridNavMesh navMesh, Vector2Int[] points, ITileContent[] tiles,
            float[] timestamps, INavigationAction[] actions)
        {
            NavMesh = navMesh;
            Points = points;
            Tiles = tiles;
            Timestamps = timestamps;
            Actions = actions;

            Timestamp = timestamps[0];
            Time = actions.Sum(a => a.Time);
        }

        public override string ToString()
        {
            return string.Join("\n", Actions.Select(a => "- " + a));
        }
    }
}