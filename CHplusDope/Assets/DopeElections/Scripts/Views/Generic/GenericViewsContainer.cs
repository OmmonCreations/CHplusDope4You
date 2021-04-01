using DopeElections.Candidates;
using DopeElections.RaceInfos;
using DopeElections.SmartSpiderInfos;
using UnityEngine;
using Views;

namespace DopeElections
{
    public class GenericViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private CandidateView _candidateView = null;
        [SerializeField] private SmartSpiderInfoView _smartSpiderInfoView = null;
        [SerializeField] private RaceInfoView _raceInfoView = null;

        public CandidateView CandidateView => _candidateView;
        public SmartSpiderInfoView SmartSpiderInfoView => _smartSpiderInfoView;
        public RaceInfoView RaceInfoView => _raceInfoView;

        protected override void OnBeforeInitialize()
        {
            gameObject.SetActive(true);
            base.OnBeforeInitialize();
        }

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(CandidateView,
                    new NavigationBranch(SmartSpiderInfoView)),
                new NavigationBranch(SmartSpiderInfoView),
                new NavigationBranch(RaceInfoView)
            );
        }
    }
}