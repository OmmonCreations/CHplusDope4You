using System.Linq;
using DopeElections.Progression;
using DopeElections.RaceResults.Celebration;
using DopeElections.Races;
using Progression;
using UnityEngine;
using Views;

namespace DopeElections.RaceResults
{
    public class RaceResultViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private CelebrationView _celebration = null;
        [SerializeField] private MarathonReviewView _reviewView = null;

        public CelebrationView CelebrationView => _celebration;
        public MarathonReviewView ReviewView => _reviewView;

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(CelebrationView),
                new NavigationBranch(ReviewView)
            );
        }

        public void Continue(RaceContext lastContext)
        {
            var entry = lastContext.ProgressEntry;
            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            var tree = questionnaire.Progression;
            var raceEntries = tree.Entries.OfType<IRaceProgressEntry>().ToList();
            var currentIndex = raceEntries.IndexOf(entry);
            if (currentIndex + 1 < raceEntries.Count)
            {
                var next = raceEntries[currentIndex + 1];
                if (!next.IsAvailable && next.Unlockable)
                {
                    entry = next;
                }
            }

            BlackMask.BlockInteractions(true);
            BlackMask.FadeToBlack(() => { DopeElectionsRouter.GoToProgress(entry); });
        }

        public void Repeat(RaceContext context)
        {
            context.Race.ApplyInitialState();
            BlackMask.BlockInteractions(true);
            BlackMask.FadeToBlack(() => { DopeElectionsRouter.GoToRace(context); });
        }
    }
}