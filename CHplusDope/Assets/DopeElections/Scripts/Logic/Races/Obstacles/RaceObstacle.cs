using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public abstract class RaceObstacle : MultiTileContent
    {
        public RaceObstacleCourse Course { get; }
        public RaceObstacleType Type { get; }

        public RaceObstacle(RaceObstacleCourse course, RaceObstacleType type, Vector2Int position, Vector2Int size)
            : base(position, size, course.Configuration.TileSize)
        {
            Course = course;
            Type = type;
        }
    }

    public abstract class RaceObstacle<T> : RaceObstacle where T : RaceObstacleType
    {
        public new T Type { get; }

        protected RaceObstacle(RaceObstacleCourse course, T type, Vector2Int position, Vector2Int size)
            : base(course, type, position, size)
        {
            Type = type;
        }
    }
}