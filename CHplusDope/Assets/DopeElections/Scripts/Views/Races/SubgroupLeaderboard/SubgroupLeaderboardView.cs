using System.Linq;
using AnimatedObjects;
using Essentials;
using Localizator;
using Pagination;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Races
{
    public class SubgroupLeaderboardView : RaceView, IView<SubgroupLeaderboardViewData>
    {
        public override NamespacedKey Id => RaceViewId.SubgroupLeaderboard;

        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private LocalizedText _matchText = null;
        [SerializeField] private Image _raceIcon = null;
        [SerializeField] private LocalizedText _matchTypeText = null;
        [SerializeField] private PaginatedViewController _candidatesView = null;
        [SerializeField] private PoppablePanelController _mainPanelController = null;

        public PoppablePanelController MainPanelController => _mainPanelController;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            _closeButton.onClick.AddListener(Back);
            _closeBackground.onClick.AddListener(Back);
            _mainPanelController.HideImmediate();
        }

        public void Open(SubgroupLeaderboardViewData data)
        {
            ApplyData(data);
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            MainPanelController.Show();
        }

        private void ApplyData(SubgroupLeaderboardViewData data)
        {
            _matchText.key = data.Match;
            _raceIcon.sprite = data.Icon;
            _matchTypeText.key = data.MatchType;
            _candidatesView.Entries = data.Candidates
                .Select(e => new CandidateEntry(e.Key, e.Value, () => OpenCandidateProfile(e.Key)))
                .Cast<PaginatedViewEntry>()
                .ToArray();
        }

        private void OpenCandidateProfile(RaceCandidate candidate)
        {
            DopeElectionsApp.Instance.Views.CandidateView.Open(candidate.Candidate);
        }

        protected override StateChangePromise PrepareClose()
        {
            var promise = new StateChangePromise();
            MainPanelController.Hide().Then(promise.Fulfill);
            return promise;
        }

        private void Back()
        {
            Close();
        }
    }
}