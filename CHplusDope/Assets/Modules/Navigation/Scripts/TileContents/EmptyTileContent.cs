using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public class EmptyTileContent : SingleTileContent
    {
        public override Color Color { get; } = Color.clear;

        public EmptyTileContent(Vector2Int position, float tileSize) : base(position, tileSize)
        {
        }

        protected override bool CanPass(INavigationAgent agent)
        {
            return true;
        }
    }
}