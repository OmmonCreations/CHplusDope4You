using System.Linq;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class LeaderboardIndexControl : LeaderboardControl
    {
        private static readonly int[] ValidIntervals = new[] {5, 10, 20, 50, 100, 200, 500, 1000};

        [SerializeField] private Button _goLeftButton = null;
        [SerializeField] private Button _goRightButton = null;
        [SerializeField] private LocalizedText _text = null;

        private int _step;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _goLeftButton.onClick.AddListener(GoLeft);
            _goRightButton.onClick.AddListener(GoRight);
            _text.key = LKey.Views.Leaderboard.Index;
            Leaderboard.CandidatesChanged += OnCandidatesChanged;
            Leaderboard.Scrolled += OnScrolled;
        }

        public override void UpdateControl()
        {
            var length = Leaderboard.Length;
            var tenth = Mathf.RoundToInt(length / 10f);
            _step = ValidIntervals.OrderBy(i => Mathf.Abs(tenth - i)).First();
            UpdateLabel();
        }

        private void GoLeft()
        {
            var leaderboard = Leaderboard;
            var scrollIndex = leaderboard.ScrollIndex;
            var from = Mathf.CeilToInt(scrollIndex / (float) _step) * _step;
            var previous = Mathf.Max(from - _step, 0);
            leaderboard.ScrollIndex = Mathf.Max(previous, 0);
        }

        private void GoRight()
        {
            var leaderboard = Leaderboard;
            var scrollIndex = leaderboard.ScrollIndex;
            var from = Mathf.FloorToInt(scrollIndex / (float) _step) * _step;
            var next = Mathf.Min(from + _step, leaderboard.Length - _step);
            leaderboard.ScrollIndex = next;
        }

        private void OnCandidatesChanged()
        {
            UpdateControl();
        }

        private void OnScrolled()
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var leaderboard = Leaderboard;
            var from = Mathf.FloorToInt(leaderboard.ScrollIndex / (float) _step) * _step + 1;
            var to = Mathf.Min(leaderboard.Length, from + _step - 1);
            _text.SetVariable("min", from.ToString());
            _text.SetVariable("max", to.ToString());
        }
    }
}