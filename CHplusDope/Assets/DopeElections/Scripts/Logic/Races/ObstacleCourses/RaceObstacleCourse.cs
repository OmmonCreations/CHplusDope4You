using System.Collections.Generic;
using DopeElections.Races;
using DopeElections.Races.RaceTracks;
using Navigation;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    public class RaceObstacleCourse
    {
        public CourseConfiguration Configuration { get; }
        public RaceCandidateConfiguration CandidateConfiguration { get; }
        public TileGridNavMesh NavigationMesh { get; }
        public ITileContent[,] Tiles { get; }
        public Vector2Int Size { get; }

        private readonly List<RaceObstacle> _obstacles = new List<RaceObstacle>();

        public IEnumerable<RaceObstacle> Obstacles => _obstacles;
        public float AverageClearTime { get; private set; }

        public RaceObstacleCourse(CourseConfiguration configuration, RaceCandidateConfiguration candidateConfiguration)
        {
            Configuration = configuration;
            CandidateConfiguration = candidateConfiguration;
            var navigationMesh = new TileGridNavMesh(configuration.TileSize);
            navigationMesh.MovementDirections = new[]
            {
                new Vector2Int(-1, 0), // left
                new Vector2Int(-1, 1), // left forward
                new Vector2Int(0, 1), // forward
                new Vector2Int(1, 1), // right forward
                new Vector2Int(1, 0) // right
            };
            NavigationMesh = navigationMesh;

            var size = configuration.Size;
            Tiles = new ITileContent[size.y, size.x];
            Size = size;
        }

        public RaceTrackVector GetRaceTrackVector(Vector2 gridPoint, float referenceStartPoint)
        {
            var configuration = Configuration;
            var tileSize = configuration.TileSize;
            var localVector = new Vector2((gridPoint.x - configuration.Size.x / 2f) * tileSize, gridPoint.y * tileSize);
            var x = localVector.x;
            var y = localVector.y + referenceStartPoint;
            return new RaceTrackVector(x, y, RaceTrackVector.AxisType.Distance);
        }

        /// <summary>
        /// Returns a tile position within this race obstacle course
        /// </summary>
        /// <param name="v">A racetrack position, relative positions will be interpreted in the course's space</param>
        /// <param name="referenceStartPoint">The distance from start at which the course starts (tile y=0)</param>
        /// <returns></returns>
        public Vector2Int GetTile(RaceTrackVector v, float referenceStartPoint)
        {
            var configuration = Configuration;
            var tileSize = configuration.TileSize;
            var size = configuration.Size;
            var relativeX = v.GetPercentageX(size.x * tileSize);
            var relativeY = v.GetPercentageY(referenceStartPoint, size.y * tileSize);

            return new Vector2Int(
                Mathf.Clamp(Mathf.FloorToInt(relativeX * size.x), 0, size.x - 1),
                Mathf.Clamp(Mathf.FloorToInt(relativeY * size.y), 0, size.y - 1)
            );
        }

        public Vector2Int GetClosestEmptyTile(INavigationAgent agent, RaceTrackVector v, float referenceStartPoint)
        {
            var intendedPosition = GetTile(v, referenceStartPoint);
            var intendedTile = Tiles[intendedPosition.y, intendedPosition.x];
            if (intendedTile.CanPass(agent, intendedPosition)) return intendedPosition;

            var size = Size;

            var queue = new Queue<Vector2Int>();
            var done = new HashSet<Vector2Int> {intendedPosition};
            queue.Enqueue(intendedPosition);

            var tiles = Tiles;
            var iteration = 0;
            var maxIterations = size.x * size.y;
            var directions = Directions.All;

            while (queue.Count > 0 && iteration < maxIterations)
            {
                iteration++;
                var current = queue.Dequeue();
                foreach (var d in directions)
                {
                    var relative = current + d;
                    if (done.Contains(relative) || relative.x < 0 || relative.y < 0 || relative.x >= size.x ||
                        relative.y >= size.y) continue;
                    var tile = tiles[relative.y, relative.x];
                    if (tile != null && tile.CanPass(agent, relative)) return relative;
                    queue.Enqueue(relative);
                    done.Add(relative);
                }
            }

            return intendedPosition;
        }

        public RawPath CalculatePath(INavigationAgent agent, Vector2Int start, Vector2Int target, float timestamp,
            bool precise)
        {
            return NavigationMesh.CalculatePath(agent, new NavigationQuery(start, target, timestamp, precise));
        }

        public RawPath CalculatePath(INavigationAgent agent, Vector2Int start, Vector2Int target, float timestamp,
            bool precise, ReactionData data)
        {
            return NavigationMesh.CalculatePath(agent,
                new RaceCandidateNavigationQuery(start, target, timestamp, precise, data));
        }

        public void AddObstacle(RaceObstacle obstacle)
        {
            _obstacles.Add(obstacle);
            var position = obstacle.Position;
            var size = obstacle.Size;
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var v = position + new Vector2Int(x, y);
                    if (v.x < 0 || v.y < 0 || v.x >= Tiles.GetLength(1) || v.y >= Tiles.GetLength(0)) continue;
                    Tiles[v.y, v.x] = obstacle;
                }
            }
        }

        public void RemoveObstacle(RaceObstacle obstacle)
        {
            _obstacles.Remove(obstacle);
            var position = obstacle.Position;
            var size = obstacle.Size;
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var v = position + new Vector2Int(x, y);
                    if (v.x < 0 || v.y < 0 || v.x >= Tiles.GetLength(1) || v.y >= Tiles.GetLength(0)) continue;
                    if (Tiles[y, x] != obstacle) continue;
                    Tiles[v.y, v.x] = null;
                }
            }
        }

        internal void RecalculateNavMesh()
        {
            NavigationMesh.Tiles = Tiles;
            NavigationMesh.FillEmptyTiles();
        }

        internal void RecalculateAverageClearTime()
        {
            AverageClearTime = ClearTimeTester.Test(this);
        }

        public class CourseConfiguration
        {
            public float TileSize { get; set; }
            public Vector2Int Size { get; set; }
            public int StartAreaLength { get; set; }
            public int ObstacleSpaceLength { get; set; }
        }
    }
}