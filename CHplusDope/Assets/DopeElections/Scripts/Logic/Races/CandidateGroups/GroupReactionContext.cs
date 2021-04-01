using DopeElections.ObstacleCourses;

namespace DopeElections.Races
{
    /// <summary>
    /// Data object that shares information on the group's reaction to an answer with other classes
    /// </summary>
    public class GroupReactionContext
    {
        /// <summary>
        /// The layout of the group after the reaction
        /// </summary>
        public CandidateGroupLayout Layout { get; }

        /// <summary>
        /// The obstacle course for the candidates to move to their designated target location
        /// </summary>
        public RaceObstacleCourse ObstacleCourse { get; }

        /// <summary>
        /// The obstacle course controller for the candidates to move to their designated target location
        /// </summary>
        public RaceObstacleCourseController ObstacleCourseController { get; }

        /// <summary>
        /// The maximun agreement score that candidates can have right now
        /// </summary>
        public int MaxAgreementScore { get; }

        public float TimePassed { get; }

        public GroupReactionContext(CandidateGroupLayout layout, RaceObstacleCourseController obstacleCourseController,
            int maxAgreementScore, float timePassed)
        {
            Layout = layout;
            ObstacleCourse = obstacleCourseController.Course;
            ObstacleCourseController = obstacleCourseController;
            MaxAgreementScore = maxAgreementScore;
            TimePassed = timePassed;
        }
    }
}