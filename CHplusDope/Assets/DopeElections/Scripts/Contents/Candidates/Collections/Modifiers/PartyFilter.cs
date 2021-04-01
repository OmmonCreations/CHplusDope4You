using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class PartyFilter : CollectionModifier<PartyFilter.FilterState>, IFilter<Candidate>
    {
        public LocalizationKey Label => LKey.Components.CandidateCollection.Filter.Party;

        protected override FilterState DefaultState => new FilterState();

        public bool Active => State.Parties.Length > 0;

        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            return entries.Where(e => State.Parties.Any(p => p.id == e.partyId));
        }

        public class FilterState : CollectionModifierState
        {
            public Party[] Parties { get; }

            public FilterState(params Party[] parties)
            {
                Parties = parties;
            }
        }
    }
}