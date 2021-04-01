using UnityEngine;

namespace Navigation
{
    public class TileGridNavMesh
    {
        public float TileSize { get; }
        public Vector2Int Size { get; private set; }
        public Vector2Int[] MovementDirections { get; set; }
        public float DiagonalFactor { get; set; } = 1.05f;
        
        private ITileContent[,] _tiles;
        private PathCalculator _pathCalculator;
        // private PathCalculationJob _pathCalculationJob;

        public ITileContent[,] Tiles
        {
            get => _tiles;
            set => ApplyTiles(value);
        }

        public TileGridNavMesh(float tileSize)
        {
            TileSize = tileSize;
        }

        public RawPath CalculatePath(INavigationAgent agent, INavigationQuery query)
        {
            var pathCalculator = _pathCalculator;
            if (pathCalculator == null) return null;
            return pathCalculator.CalculatePath(agent, query);
        }

        private void ApplyTiles(ITileContent[,] tiles)
        {
            Size = new Vector2Int(tiles.GetLength(1), tiles.GetLength(0));
            _tiles = tiles;
            _pathCalculator = new PathCalculator(this);
        }

        public void FillEmptyTiles()
        {
            if (Tiles == null) return;
            FillEmptyTiles(Tiles, TileSize);
        }

        private void FillEmptyTiles(ITileContent[,] tiles, float tileSize)
        {
            for (var y = 0; y < Size.y; y++)
            {
                for (var x = 0; x < Size.x; x++)
                {
                    if (tiles[y, x] != null) continue;
                    tiles[y, x] = new EmptyTileContent(new Vector2Int(x, y), tileSize);
                }
            }
        }
    }
}