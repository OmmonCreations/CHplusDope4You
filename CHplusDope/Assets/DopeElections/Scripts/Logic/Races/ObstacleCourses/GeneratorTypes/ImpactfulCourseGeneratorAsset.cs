using DopeElections.Races;
using DopeElections.Races.Hurdle;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "PoolCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Impactful Course Generator")]
    public class ImpactfulCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
    {
        [SerializeField] private Layout _layout = Layout.LargeSmallLarge;

        [Header("Large Obstacles")] [SerializeField]
        private ImpactObstacleType _largeObstacleType = null;

        [SerializeField] private int _largeObstacleSpacing = 4;
        [SerializeField] private int _largeCount = 3;
        [SerializeField] private bool _offsetBottomSide = true;

        [Header("Small Obstacles")] [SerializeField]
        private HurdleObstacleType _smallObstacleType = null;

        [SerializeField] private int _smallMinSpacing = 3;
        [SerializeField] private int _smallRandomSpacing = 2;

        protected override void Generate(RaceObstacleCourse course)
        {
            var largeObstacleType = _largeObstacleType;
            var smallObstacleType = _smallObstacleType;

            if (largeObstacleType && largeObstacleType != null)
            {
                GenerateLargeObstacles(course, largeObstacleType);
            }

            if (smallObstacleType && smallObstacleType != null)
            {
                GenerateSmallObstacles(course, smallObstacleType);
            }
        }

        public override int GetPreferredObstacleSpaceLength(int width)
        {
            return _largeCount * _largeObstacleType.Size.y + (_largeCount - 1) * _largeObstacleSpacing;
        }

        private void GenerateLargeObstacles(RaceObstacleCourse course, ImpactObstacleType type)
        {
            var courseLength = course.Configuration.ObstacleSpaceLength;
            var courseWidth = course.Configuration.Size.x;
            var size = type.Size;

            var count = Mathf.FloorToInt((courseLength + _largeObstacleSpacing) /
                                         (size.x + (float) _largeObstacleSpacing));
            if (_layout == Layout.LargeSmallLarge || _layout == Layout.LargeSmall)
            {
                GenerateLargeObstacleRow(course, type, 0, count, 0, ImpactObstacle.TrackAnchor.Left);
            }

            if (_layout == Layout.LargeSmallLarge || _layout == Layout.SmallLarge)
            {
                var offset = _offsetBottomSide ? _largeObstacleSpacing : 0;
                var bottomCount = _offsetBottomSide ? count - 1 : count;
                GenerateLargeObstacleRow(course, type, courseWidth - 1 - type.Size.x, bottomCount, offset,
                    ImpactObstacle.TrackAnchor.Right);
            }
        }

        /// <summary>
        /// Generates a row of large obstacles along the movement direction of the racetrack
        /// </summary>
        private void GenerateLargeObstacleRow(RaceObstacleCourse course, ImpactObstacleType type, int x, int count,
            int yOffset, ImpactObstacle.TrackAnchor anchor)
        {
            var size = type.Size;
            var courseStartY = course.Configuration.StartAreaLength;

            for (var i = 0; i < count; i++)
            {
                var y = courseStartY + i * (size.x + _largeObstacleSpacing) + yOffset;
                var position = new Vector2Int(x, y);
                var delay = type.Delay + Random.Range(-type.RandomizeDelay / 2, type.RandomizeDelay / 2);
                var data = new ImpactObstacle.Data(anchor, delay);
                var obstacle = new ImpactObstacle(course, type, position, size, data);
                course.AddObstacle(obstacle);
            }
        }

        private void GenerateSmallObstacles(RaceObstacleCourse course, HurdleObstacleType type)
        {
            var largeObstacleType = _largeObstacleType;
            var largeObstacleSize = largeObstacleType.Size;

            var courseLength = course.Configuration.ObstacleSpaceLength;
            var courseWidth = course.Configuration.Size.x;
            var courseStartY = course.Configuration.StartAreaLength;

            var size = type.Size;
            // ensure spacing + randomSpacing always equal or greater than 1 to prevent infinite while loop
            var minSpacing = Mathf.Max(1, size.y + Mathf.Max(0, _smallMinSpacing));
            var randomSpacing = Mathf.Max(0, _smallRandomSpacing);

            var fromX = _layout == Layout.LargeSmall || _layout == Layout.LargeSmallLarge ? largeObstacleSize.x : 0;
            var toX = _layout == Layout.SmallLarge || _layout == Layout.LargeSmallLarge
                ? courseWidth - largeObstacleSize.x - 1
                : courseWidth - 1;
            var countPerRow = Mathf.FloorToInt((toX - fromX) / (float) size.x);
            var y = courseStartY + _smallMinSpacing / 2;
            while (y < courseStartY + courseLength - 1)
            {
                var from = new Vector2Int(fromX, y);
                GenerateSmallObstacleRow(course, type, from, countPerRow);
                y += minSpacing + RandomUtils.Random.Range(0, randomSpacing + 1);
            }
        }

        private void GenerateSmallObstacleRow(RaceObstacleCourse course, HurdleObstacleType type, Vector2Int from,
            int count)
        {
            var size = type.Size;
            for (var x = 0; x < count; x++)
            {
                var position = new Vector2Int(from.x + x * size.x, from.y);
                var hurdle = new HurdleObstacle(course, type, position, size);
                course.AddObstacle(hurdle);
            }
        }

        private enum Layout
        {
            LargeSmallLarge,
            SmallLargeSmall,
            LargeSmall,
            SmallLarge
        }
    }
}