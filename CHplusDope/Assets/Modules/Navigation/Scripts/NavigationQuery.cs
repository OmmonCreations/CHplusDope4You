using UnityEngine;

namespace Navigation
{
    public class NavigationQuery : INavigationQuery
    {
        public Vector2Int From { get; }
        public Vector2Int To { get; }
        public float Timestamp { get; }
        public bool Precise { get; }

        public NavigationQuery(Vector2Int from, Vector2Int to, float timestamp, bool precise)
        {
            From = from;
            To = to;
            Timestamp = timestamp;
            Precise = precise;
        }
    }
}