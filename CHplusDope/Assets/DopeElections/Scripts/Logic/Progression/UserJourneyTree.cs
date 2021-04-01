using Progression;

namespace DopeElections.Progression
{
    public class UserJourneyTree : ProgressionTree
    {
        private UserJourneyTree(IProgressEntry[] entries) : base(entries)
        {
        }

        public static UserJourneyTree Get()
        {
            return new UserJourneyTree(CreateEntries());
        }

        private static IProgressEntry[] CreateEntries()
        {
            return new IProgressEntry[]
            {
                new GenericProgressEntry(UserJourneyStepId.CharacterCreation),
                new CutsceneProgressEntry(UserJourneyStepId.GrabSequence, CutsceneId.GrabSequence),
                new CutsceneProgressEntry(UserJourneyStepId.OpeningComic, CutsceneId.OpeningComic),
                new CutsceneProgressEntry(UserJourneyStepId.MainMenuLanding, CutsceneId.MainMenuLanding),
                new GenericProgressEntry(UserJourneyStepId.FirstRace),
                new GenericProgressEntry(UserJourneyStepId.RaceInfo)
            };
        }
    }
}