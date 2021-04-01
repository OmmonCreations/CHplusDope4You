using DopeElections.Answer;
using UnityEngine;
using Views;

namespace DopeElections.Candidates
{
    public class CandidateSubviews : DopeElectionsViewsContainer
    {
        [SerializeField] private CandidateProfileView _profileView = null;
        [SerializeField] private CandidateAnswersView _answersView = null;

        private Candidate _candidate;

        public CandidateProfileView ProfileView => _profileView;
        public CandidateAnswersView AnswersView => _answersView;
        
        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(ProfileView),
                new NavigationBranch(AnswersView)
            );
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;
            
            UpdateLocalization();
            ProfileView.Candidate = candidate;
            AnswersView.Candidate = candidate;
        }
    }
}