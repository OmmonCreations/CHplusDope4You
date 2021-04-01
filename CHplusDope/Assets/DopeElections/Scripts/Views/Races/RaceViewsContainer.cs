using DopeElections.Races.QuestionInfos;
using UnityEngine;
using Views;

namespace DopeElections.Races
{
    public class RaceViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private QuestionView _questionView = null;
        [SerializeField] private QuestionInfoView _questionInfoView = null;
        [SerializeField] private SubgroupLeaderboardView _subgroupLeaderboardView = null;

        public QuestionView QuestionView => _questionView;
        public QuestionInfoView QuestionInfoView => _questionInfoView;
        public SubgroupLeaderboardView SubgroupLeaderboardView => _subgroupLeaderboardView;

        public RaceController RaceController { get; private set; }

        public void Initialize(RaceController raceController)
        {
            RaceController = raceController;
            base.Initialize();
        }

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(QuestionView,
                    new NavigationBranch(QuestionInfoView),
                    new NavigationBranch(SubgroupLeaderboardView)
                )
            );
        }
    }
}