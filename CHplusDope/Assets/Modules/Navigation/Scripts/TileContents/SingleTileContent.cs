using UnityEngine;

namespace Navigation
{
    public abstract class SingleTileContent : AbstractTileContent
    {
        protected SingleTileContent(Vector2Int position, float tileSize) : base(position, Vector2Int.one, tileSize)
        {
            
        }

        public sealed override bool CanPass(INavigationAgent agent, Vector2Int position)
        {
            return CanPass(agent);
        }

        protected abstract bool CanPass(INavigationAgent agent);
    }
}