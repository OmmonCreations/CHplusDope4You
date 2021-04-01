using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "HotWheelsCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Hot Wheels Course Generator")]
    public class HotWheelsCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
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