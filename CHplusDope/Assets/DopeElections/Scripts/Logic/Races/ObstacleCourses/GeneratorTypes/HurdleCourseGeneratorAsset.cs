using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "HurdleCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Hurdle Course Generator")]
    public class HurdleCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
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