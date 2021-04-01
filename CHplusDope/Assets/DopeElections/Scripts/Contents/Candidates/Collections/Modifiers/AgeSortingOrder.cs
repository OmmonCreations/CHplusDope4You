using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class AgeSortingOrder : CollectionModifier<AgeSortingOrder.FilterState>, ISortingOrder<Candidate>
    {
        public string Id => SortingOrderId.Age;
        public LocalizationKey Label => LKey.Components.CandidateCollection.SortingOrder.Age;
        
        protected override FilterState DefaultState => new FilterState(false);
        
        public bool Reverse
        {
            get => State.Reverse;
            set => State = new FilterState(value);
        }

        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            return State.Reverse 
                    ? entries.OrderBy(e => e.birthYear) 
                    : entries.OrderByDescending(e => e.birthYear);
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