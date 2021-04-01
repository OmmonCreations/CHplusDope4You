using CameraSystems;
using DopeElections.Progress.Congratulations;
using DopeElections.Progress.ExtraInfos;
using DopeElections.Progress.LeaderboardFilter;
using Localizator;
using UnityEngine;
using Views;

namespace DopeElections.Progress
{
    public class ProgressViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private CameraSystem _cameraSystem = null;
        [Header("Views")] [SerializeField] private ProgressionView _progressView = null;
        [SerializeField] private LeaderboardView _leaderboardView = null;
        [SerializeField] private LeaderboardFilterView _leaderboardFilterView = null;
        [SerializeField] private ExtraInfoView _extraInfoView = null;
        [SerializeField] private CongratulationsView _congratulationsView = null;

        public ProgressionView ProgressionView => _progressView;
        public LeaderboardView LeaderboardView => _leaderboardView;
        public LeaderboardFilterView LeaderboardFilterView => _leaderboardFilterView;
        public ExtraInfoView ExtraInfoView => _extraInfoView;
        public CongratulationsView CongratulationsView => _congratulationsView;

        public CameraSystem CameraSystem => _cameraSystem;

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(ProgressionView),
                new NavigationBranch(LeaderboardView,
                    new NavigationBranch(LeaderboardFilterView)
                ),
                new NavigationBranch(ExtraInfoView),
                new NavigationBranch(CongratulationsView)
            );
        }
    }
}