using System.Linq;
using AnimatedObjects;
using DopeElections.Candidates;
using DopeElections.Localizations;
using Essentials;
using Localizator;
using SortableCollections;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Progress.LeaderboardFilter
{
    public class LeaderboardFilterView : ProgressView, IView<LeaderboardFilterControl>
    {
        public override NamespacedKey Id => ProgressViewId.LeaderboardFilter;

        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private ToggleableObjectController _bodyAnimationController = null;
        [SerializeField] private PartyFilterController _partyFilter = null;
        [SerializeField] private SortingButtonController _matchSortingButton = null;
        [SerializeField] private SortingButtonController _nameSortingButton = null;
        [SerializeField] private SortingButtonController _partySortingButton = null;
        [SerializeField] private SortingButtonController _ageSortingButton = null;
        [SerializeField] private LocalizedText _orderText = null;
        [SerializeField] private LocalizedText _matchText = null;
        [SerializeField] private LocalizedText _nameText = null;
        [SerializeField] private LocalizedText _partyText = null;
        [SerializeField] private LocalizedText _ageText = null;
        [SerializeField] private LocalizedText _partyFilterText = null;
        [SerializeField] private LocalizedText _confirmText = null;

        private LeaderboardFilterControl Control { get; set; }

        public bool HasFiltersActive
        {
            get
            {
                var collection = Control.Leaderboard.Collection;
                return collection.SortingOrder != _matchSortingButton.SortingOrder.Id ||
                       _matchSortingButton.SortingOrder.Reverse ||
                       collection.Modifiers.OfType<IFilter>().Any(f => f.Active);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _closeBackground.onClick.AddListener(Confirm);
            _confirmButton.onClick.AddListener(Confirm);
            _bodyAnimationController.HideImmediate();

            _orderText.key = LKey.Views.LeaderboardFilter.Order;
            _matchText.key = LKey.Views.LeaderboardFilter.Match;
            _nameText.key = LKey.Views.LeaderboardFilter.Name;
            _partyText.key = LKey.Views.LeaderboardFilter.Party;
            _ageText.key = LKey.Views.LeaderboardFilter.Age;
            _partyFilterText.key = LKey.Views.LeaderboardFilter.PartyFilter;
            _confirmText.key = LKey.Views.LeaderboardFilter.Confirm;
        }

        public void Open(LeaderboardFilterControl control)
        {
            Control = control;
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            var collection = Control.Leaderboard.Collection;
            _partyFilter.Initialize(collection, collection.Modifiers.OfType<PartyFilter>().FirstOrDefault());
            _matchSortingButton.Initialize(collection,
                collection.Modifiers.OfType<MatchSortingOrder>().FirstOrDefault());
            _nameSortingButton.Initialize(collection, collection.Modifiers.OfType<NameSortingOrder>().FirstOrDefault());
            _partySortingButton.Initialize(collection,
                collection.Modifiers.OfType<PartySortingOrder>().FirstOrDefault());
            _ageSortingButton.Initialize(collection, collection.Modifiers.OfType<AgeSortingOrder>().FirstOrDefault());
            _bodyAnimationController.Show();
        }

        protected override StateChangePromise PrepareClose()
        {
            if (Control != null)
            {
                var collection = Control.Leaderboard.Collection;
                if (collection != null && collection.SortingOrder == null)
                {
                    _matchSortingButton.SortingOrder.Reverse = false;
                    collection.SortingOrder = _matchSortingButton.SortingOrder.Id;
                }
            }

            var promise = new StateChangePromise();
            _bodyAnimationController.Hide().Then(promise.Fulfill);
            return promise;
        }

        private void Confirm()
        {
            Control.Leaderboard.Collection.Update();
            Close();
        }
    }
}