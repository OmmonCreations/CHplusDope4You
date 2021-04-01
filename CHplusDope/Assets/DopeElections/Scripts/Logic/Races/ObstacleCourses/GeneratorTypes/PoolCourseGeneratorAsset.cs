using DopeElections.Races;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "PoolCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Pool Course Generator")]
    public class PoolCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
    {
        [SerializeField] private int _poolLength = 1;
        [SerializeField] private int _spacing = 6;
        [SerializeField] private int _count = 1;
        [SerializeField] private PoolObstacleType _poolType = null;

        protected override void Generate(RaceObstacleCourse course)
        {
            var configuration = course.Configuration;
            var startY = configuration.StartAreaLength + 1;
            var obstacleSpaceLength = configuration.ObstacleSpaceLength;

            GeneratePoolRows(course, startY, obstacleSpaceLength);
        }

        public override int GetPreferredObstacleSpaceLength(int width)
        {
            var result = _poolLength * _count + _spacing * Mathf.Max(0, _count - 1) + 2;
            return result;
        }

        private void GeneratePoolRows(RaceObstacleCourse course, int y, int length)
        {
            var maxY = y + length - 2;

            var poolSpacing = _spacing;

            // Debug.Log("Generate pools at " + y + " (length: " + length + ")");

            if (_poolType)
            {
                var poolLength = _poolLength;
                var currentY = y + Mathf.FloorToInt(_spacing / 2f);
                const int maxIterations = 1000;
                var iteration = 0;
                while (currentY < maxY && iteration < maxIterations)
                {
                    iteration++;
                    var poolType = _poolType;
                    GeneratePool(course, poolType, currentY);
                    currentY += poolLength;

                    currentY += poolSpacing;
                }
            }
            else
            {
                Debug.LogWarning("Pool Generator " + name + " has no PoolType assigned.");
            }
        }

        private void GeneratePool(RaceObstacleCourse course, PoolObstacleType type, int y)
        {
            var poolLength = _poolLength;
            var poolWidth = course.Tiles.GetLength(1);
            var size = new Vector2Int(poolWidth, poolLength);
            var position = new Vector2Int(0, y);
            var pool = new PoolObstacle(course, type, position, size);
            course.AddObstacle(pool);
        }
    }
}