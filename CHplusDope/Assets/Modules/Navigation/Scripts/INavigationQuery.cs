using UnityEngine;

namespace Navigation
{
    public interface INavigationQuery
    {
        Vector2Int From { get; }
        Vector2Int To { get; }
        float Timestamp { get; }
        bool Precise { get; }
    }
}