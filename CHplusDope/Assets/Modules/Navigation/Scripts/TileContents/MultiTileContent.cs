using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public abstract class MultiTileContent : AbstractTileContent, ITileContent
    {
        protected MultiTileContent(Vector2Int position, Vector2Int size, float tileSize)
            : base(position, size, tileSize)
        {
            
        }
    }
}