using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    [CreateAssetMenu(fileName = "PillarParcourCourseGenerator",
        menuName = "Dope Elections/Obstacle Courses/Pillar Parcour Course Generator")]
    public class PillarParcourCourseGeneratorAsset : RaceObstacleCourseGeneratorAsset
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