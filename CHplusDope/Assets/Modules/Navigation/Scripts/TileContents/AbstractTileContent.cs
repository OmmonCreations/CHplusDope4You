using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public abstract class AbstractTileContent : ITileContent
    {
        public Vector2Int Position { get; }
        public Vector2Int Size { get; }
        public float TileSize { get; }
        public abstract Color Color { get; }

        protected OccupantMap Occupants { get; } = new OccupantMap();

        protected virtual float SpeedModifier { get; } = 1;

        protected AbstractTileContent(Vector2Int position, Vector2Int size, float tileSize)
        {
            Position = position;
            Size = size;
            TileSize = tileSize;
        }

        public abstract bool CanPass(INavigationAgent agent, Vector2Int position);

        public virtual IEnumerable<INavigationAction> GetNavigationActions(NavigationContext context,
            Vector2Int position, float timestamp)
        {
            var navMesh = context.NavMesh;
            var maxSize = navMesh.Size;
            var maxX = maxSize.x - 1;
            var maxY = maxSize.y - 1;
            foreach (var d in GetMovementDirections(context))
            {
                var relative = position + d;
                if (relative.x < 0 || relative.y < 0 || relative.x > maxX || relative.y > maxY) continue;
                var action = GetMoveAction(context, position, relative, timestamp);
                if (action == null) continue;
                yield return action;
            }
        }

        protected virtual INavigationAction GetMoveAction(NavigationContext context,
            Vector2Int from, Vector2Int to, float timestamp)
        {
            var agent = context.Agent;
            var toTile = context.Tiles[to.y, to.x];
            if (toTile == null)
            {
                Debug.LogWarning("Target tile is missing!");
                return null;
            }

            var diagonalFactor = (to - from).sqrMagnitude > 1 ? context.DiagonalFactor : 1;
            var clearTime = TileSize * diagonalFactor / (agent.Speed * SpeedModifier);
            INavigationAction moveAction = new MoveAction(from, to, clearTime);

            // var occupantCount = toTile.GetOccupants(to, timestamp).Count();
            var occupiedTime = toTile.GetOccupiedTime(to, timestamp, clearTime);
            if (occupiedTime >= float.MaxValue)
            {
                return null;
            }

            return occupiedTime > 0 ? new CompositeAction(new IdleAction(from, occupiedTime), moveAction) : moveAction;
        }

        protected virtual Vector2Int[] GetMovementDirections(NavigationContext context)
        {
            return context.NavMesh.MovementDirections;
        }

        public void AddOccupant(Occupant occupant, Vector2Int position)
        {
            Occupants[occupant] = position;
        }

        public void RemoveOccupant(INavigationAgent agent)
        {
            var occupant = Occupants.FirstOrDefault(e => e.Key.Agent == agent).Key;
            if (occupant == null) return;

            Occupants.Remove(occupant);
        }

        public IEnumerable<Occupant> GetOccupants(Vector2Int position, float timestamp)
        {
            return Occupants.Where(e => e.Value == position && e.Key.BlocksAt(timestamp)).Select(e => e.Key);
        }

        public bool HasOccupants(Vector2Int position, float timestamp)
        {
            return Occupants.Any(e => e.Value == position && e.Key.BlocksAt(timestamp));
        }

        public float GetOccupiedTime(Vector2Int position, float timestamp, float time)
        {
            return Occupants.Where(e => e.Key is TemporaryOccupant && e.Value == position)
                .Select(o => o.Key.GetRemainingBlockTime(timestamp, time))
                .DefaultIfEmpty(0)
                .Max();
        }

        public bool Contains(Vector2Int position)
        {
            var delta = position - Position;
            var size = Size;
            return delta.x >= 0 && delta.y >= 0 && delta.x < size.x && delta.y < size.y;
        }

        protected class OccupantMap : Dictionary<Occupant, Vector2Int>
        {
        }
    }
}