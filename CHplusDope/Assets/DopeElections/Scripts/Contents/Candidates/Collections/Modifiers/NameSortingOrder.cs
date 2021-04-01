using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class NameSortingOrder : CollectionModifier<NameSortingOrder.FilterState>, ISortingOrder<Candidate>
    {
        public string Id => SortingOrderId.Name;
        public LocalizationKey Label => LKey.Components.CandidateCollection.SortingOrder.Name;
        
        protected override FilterState DefaultState => new FilterState(false);

        public bool Reverse
        {
            get => State.Reverse;
            set => State = new FilterState(value);
        }
        
        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            return State.Reverse 
                    ? entries.OrderByDescending(e => e.lastName) 
                    : entries.OrderBy(e => e.lastName);
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