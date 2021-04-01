using DopeElections.Progression;

namespace DopeElections.Races
{
    public class RaceContext
    {
        public RaceProgressionTree Tree { get; }
        public IRaceProgressEntry ProgressEntry { get; }
        public IRace Race { get; set; }
        public float RelativeRaceIndex { get; }

        public RaceContext(RaceProgressionTree tree, IRaceProgressEntry progressEntry,
            IRace race = null)
        {
            Tree = tree;
            ProgressEntry = progressEntry;
            Race = race;
            RelativeRaceIndex = tree.GetRelativeRaceIndex(progressEntry);
        }

        public bool CreateRace()
        {
            Race = ProgressEntry.CreateRace(this);
            return Race != null;
        }
    }
}