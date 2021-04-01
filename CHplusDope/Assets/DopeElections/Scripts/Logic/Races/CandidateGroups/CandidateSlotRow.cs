namespace DopeElections.Races
{
    /// <summary>
    /// Data object that forms one row of candidates within the candidate group layout
    /// </summary>
    public class CandidateSlotRow
    {
        /// <summary>
        /// Forward position within the current layout
        /// </summary>
        public float RelativeForwardPosition { get; }
        public CandidateSubgroup Subgroup { get; }
        public RaceCandidate[] Candidates { get; }

        public CandidateSlotRow(float relativeForwardPosition, CandidateSubgroup subgroup, RaceCandidate[] candidates)
        {
            RelativeForwardPosition = relativeForwardPosition;
            Subgroup = subgroup;
            Candidates = candidates;
        }
    }
}