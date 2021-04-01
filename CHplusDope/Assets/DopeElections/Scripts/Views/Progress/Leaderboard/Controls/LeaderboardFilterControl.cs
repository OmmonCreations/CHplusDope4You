using AnimatedObjects;
using DopeElections.Localizations;
using DopeElections.Progress.LeaderboardFilter;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class LeaderboardFilterControl : LeaderboardControl
    {
        [SerializeField] private LocalizedText _text = null;
        [SerializeField] private Button _button = null;
        [SerializeField] private ToggleableObjectController _highlightController = null;
        [SerializeField] private LeaderboardFilterView _filterView = null;

        private bool _highlighted = false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _text.key = LKey.Views.Leaderboard.Filter;
            _button.onClick.AddListener(OpenFilters);
            _highlightController.HideImmediate();
            _filterView.Closed += OnFiltersClosed;
        }

        public override void UpdateControl()
        {
            
        }

        private void OpenFilters()
        {
            _filterView.Open(this);
        }

        private void OnFiltersClosed()
        {
            var highlighted = _filterView.HasFiltersActive;
            if (highlighted == _highlighted) return;
            _highlighted = highlighted;
            _highlightController.Show(highlighted);
        }
    }
}