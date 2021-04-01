using DopeElections.Races;
using Navigation;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    public class ClearTimeTester : INavigationAgent
    {
        private RaceObstacleCourse.CourseConfiguration CourseConfiguration { get; }
        private RaceCandidateConfiguration CandidateConfiguration { get; }
        public float Speed => CandidateConfiguration.RunSpeed * 0.8f;

        private ClearTimeTester(RaceObstacleCourse.CourseConfiguration courseConfiguration,
            RaceCandidateConfiguration candidateConfiguration)
        {
            CourseConfiguration = courseConfiguration;
            CandidateConfiguration = candidateConfiguration;
        }

        public static float Test(RaceObstacleCourse course)
        {
            var courseConfiguration = course.Configuration;
            var candidateConfiguration = course.CandidateConfiguration;

            var tiles = course.Tiles;
            var xCenter = tiles.GetLength(1) / 2;

            var length = tiles.GetLength(0);
            var startAreaLength = courseConfiguration.StartAreaLength;

            var start = new Vector2Int(xCenter, startAreaLength / 3);
            var end = new Vector2Int(xCenter, length - Mathf.Max(1, Mathf.CeilToInt(startAreaLength / 3f)));

            // Debug.Log("Course size: " + courseConfiguration.Size + ", Test Start: " + start + ", Test End: " + end);

            var agent = new ClearTimeTester(courseConfiguration, candidateConfiguration);
            var path = course.CalculatePath(agent, start, end, 0, true);
            if (path == null)
            {
                Debug.LogError("Obstacle Course is not solvable!");
                return 1;
            }

            return path.ClearTime;
        }
    }
}