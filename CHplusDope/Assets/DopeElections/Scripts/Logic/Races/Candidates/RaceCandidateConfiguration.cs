namespace DopeElections.Races
{
    public class RaceCandidateConfiguration
    {
        public float StrollSpeed { get; set; } = 5;
        public float RunSpeed { get; set; } = 20;

        public static RaceCandidateConfiguration Default { get; } = new RaceCandidateConfiguration();
    }
}