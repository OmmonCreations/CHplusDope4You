using System;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Elections;
using DopeElections.Localizations;
using DopeElections.Users;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class LeaderboardCouncilControl : LeaderboardControl
    {
        [SerializeField] private Button _goLeftButton = null;
        [SerializeField] private Button _goRightButton = null;
        [SerializeField] private LocalizedText _text = null;

        private ActiveUser User { get; set; }
        private Election _selected;

        private Election[] Elections { get; set; }

        private Election Selected
        {
            get => _selected;
            set => ApplyElection(value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _goLeftButton.onClick.AddListener(GoLeft);
            _goRightButton.onClick.AddListener(GoRight);
            User = DopeElectionsApp.Instance.User;
            Leaderboard.CandidatesChanged += OnCandidatesChanged;

            UpdateControl();
        }

        public override void UpdateControl()
        {
            Elections = User.GetAvailableElections();
            _selected = User.GetElection();
            if (_selected != null && !_selected.IsAvailableTo(User)) _selected = null;
            UpdateCandidates();
            UpdateLabel();
        }

        private void GoLeft()
        {
            var currentIndex = Selected != null && Elections.Any(e => e == Selected) ? Elections.IndexOf(Selected) : -1;
            if (currentIndex < 0) currentIndex = Elections.Length - 1;
            else currentIndex -= 1;

            Selected = currentIndex >= 0 ? Elections[currentIndex] : null;
        }

        private void GoRight()
        {
            var currentIndex = Selected != null && Elections.Any(e => e == Selected) ? Elections.IndexOf(Selected) : -1;
            if (currentIndex >= Elections.Length - 1) currentIndex = -1;
            else currentIndex += 1;

            Selected = currentIndex >= 0 ? Elections[currentIndex] : null;
        }

        private void OnCandidatesChanged()
        {
            UpdateLabel();
        }

        private void ApplyElection(Election election)
        {
            _selected = election;

            User.ElectionId = election != null ? election.id : 0;
            User.Save();

            UpdateCandidates();
        }

        private void UpdateCandidates()
        {
            var leaderboard = Leaderboard;
            leaderboard.UpdateCollection();
        }

        private void UpdateLabel()
        {
            var election = Selected;
            _text.key = election != null
                ? new LocalizationKey {fallback = election.label}
                : LKey.Views.Leaderboard.AllCouncils;
        }
    }
}