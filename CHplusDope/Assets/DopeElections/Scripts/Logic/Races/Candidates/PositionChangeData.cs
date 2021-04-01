using DopeElections.Races.RaceTracks;

namespace DopeElections.Races
{
    public class PositionChangeData
    {
        public CandidateSlot Slot { get; }
        public RaceTrackVector GroupAnchor { get; set; }

        public PositionChangeData(CandidateSlot slot, RaceTrackVector groupAnchor)
        {
            Slot = slot;
            GroupAnchor = groupAnchor;
        }
    }
}