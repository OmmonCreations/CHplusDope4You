using System.Collections.Generic;
using System.Linq;
using DopeElections.Races;
using Navigation;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    public class RaceObstacleCourseFactory
    {
        public RaceCandidateConfiguration CandidateConfiguration { get; set; }
        public float ReferenceStartPoint { get; set; }
        public float StartAreaLength { get; set; }
        public float ObstacleSpaceLength { get; set; }
        public float Width { get; set; } = 10;
        public float Length { get; set; } = 10;
        public float TileSize { get; set; } = 1;
        public IEnumerable<RaceCandidateController> JokerUsers { get; set; }
        public IObstacleCourseGenerator Generator { get; set; }

        public RaceObstacleCourseFactory SetCandidateConfiguration(RaceCandidateConfiguration candidateConfiguration)
        {
            CandidateConfiguration = candidateConfiguration;
            return this;
        }

        public RaceObstacleCourseFactory SetStartAreaLength(float length)
        {
            StartAreaLength = length;
            return this;
        }

        public RaceObstacleCourseFactory SetObstacleSpaceLength(float length)
        {
            ObstacleSpaceLength = length;
            return this;
        }

        public RaceObstacleCourseFactory SetWidth(float width)
        {
            Width = width;
            return this;
        }

        public RaceObstacleCourseFactory SetLength(float length)
        {
            Length = length;
            return this;
        }

        public RaceObstacleCourseFactory SetTileSize(float size)
        {
            TileSize = size;
            return this;
        }

        public RaceObstacleCourseFactory SetJokerUsers(IEnumerable<RaceCandidateController> jokerUsers,
            float referenceStartPoint)
        {
            JokerUsers = jokerUsers;
            ReferenceStartPoint = referenceStartPoint;
            return this;
        }

        public RaceObstacleCourseFactory SetGenerator(IObstacleCourseGenerator generator)
        {
            Generator = generator;
            return this;
        }

        private Vector2Int CalculateGridSize()
        {
            return new Vector2Int(Mathf.FloorToInt(Width / TileSize), Mathf.FloorToInt(Length / TileSize));
        }

        public RaceObstacleCourse Build()
        {
            var gridSize = CalculateGridSize();
            // Debug.Log(gridSize);
            var startAreaLength = Mathf.CeilToInt(StartAreaLength / TileSize);
            var obstacleSpaceLength = Mathf.CeilToInt(ObstacleSpaceLength / TileSize);
            var courseConfiguration = new RaceObstacleCourse.CourseConfiguration()
            {
                Size = gridSize,
                TileSize = TileSize,
                StartAreaLength = startAreaLength,
                ObstacleSpaceLength = obstacleSpaceLength
            };


            var result = new RaceObstacleCourse(courseConfiguration, CandidateConfiguration);
            var jokerUsers = JokerUsers != null
                ? JokerUsers.Select(c => new KeyValuePair<INavigationAgent, Vector2Int>(
                    c,
                    result.GetTile(c.Position, ReferenceStartPoint) + new Vector2Int(0, 1)
                ))
                : null;

            Generator.Generate(result, jokerUsers);
            result.RecalculateNavMesh();
            result.RecalculateAverageClearTime();
            return result;
        }
    }
}