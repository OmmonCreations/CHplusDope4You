using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
    public interface ITileContent
    {
        bool CanPass(INavigationAgent agent, Vector2Int position);
        IEnumerable<INavigationAction> GetNavigationActions(NavigationContext context, Vector2Int position, float timestamp);
        bool HasOccupants(Vector2Int position, float timestamp);
        IEnumerable<Occupant> GetOccupants(Vector2Int position, float timestamp);
        float GetOccupiedTime(Vector2Int position, float timestamp, float time);
        void AddOccupant(Occupant occupant, Vector2Int position);
        void RemoveOccupant(INavigationAgent agent);
        Color Color { get; }
    }
}