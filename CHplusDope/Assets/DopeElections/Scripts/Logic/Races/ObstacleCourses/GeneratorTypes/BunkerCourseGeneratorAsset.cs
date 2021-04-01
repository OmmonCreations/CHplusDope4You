using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "BunkerCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Bunker Course Generator")]
    public class BunkerCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
    {
        protected override void Generate(RaceObstacleCourse course)
        {
            
        }

        public override int GetPreferredObstacleSpaceLength(int width)
        {
            return 0;
        }
    }
}