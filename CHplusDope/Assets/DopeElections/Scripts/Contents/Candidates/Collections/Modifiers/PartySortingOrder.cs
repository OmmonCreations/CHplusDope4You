using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class PartySortingOrder : CollectionModifier<PartySortingOrder.FilterState>, ISortingOrder<Candidate>
    {
        public string Id => SortingOrderId.Party;
        public LocalizationKey Label => LKey.Components.CandidateCollection.SortingOrder.Party;
        
        protected override FilterState DefaultState => new FilterState(false);

        public bool Reverse
        {
            get => State.Reverse;
            set => State = new FilterState(value);
        }

        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            var mapped = new Dictionary<int,List<Candidate>>();
            foreach (var c in entries.OrderBy(c=>c.lastName))
            {
                if(!mapped.ContainsKey(c.partyId)) mapped[c.partyId] = new List<Candidate>();
                mapped[c.partyId].Add(c);
            }

            return (Reverse 
                    ? mapped.OrderBy(e => e.Value.Count) 
                    : mapped.OrderByDescending(e => e.Value.Count))
                .SelectMany(e => e.Value);
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