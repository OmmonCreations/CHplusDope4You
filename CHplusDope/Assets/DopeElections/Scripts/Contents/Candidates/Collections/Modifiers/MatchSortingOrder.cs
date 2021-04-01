using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class MatchSortingOrder : CollectionModifier<MatchSortingOrder.FilterState>, ISortingOrder<Candidate>
    {
        public string Id => SortingOrderId.Match;
        public LocalizationKey Label => LKey.Components.CandidateCollection.SortingOrder.Match;

        protected override FilterState DefaultState => new FilterState(false);

        public bool Reverse
        {
            get => State.Reverse;
            set => State = new FilterState(value);
        }

        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            return State.Reverse
                ? entries.OrderBy(c => c.match)
                : entries.OrderByDescending(c => c.match);
        }

        public class FilterState : CollectionModifierState
        {
            public bool Reverse { get; }

            public FilterState(bool reverse)
            {
                Reverse = reverse;
            }
        }
    }
}