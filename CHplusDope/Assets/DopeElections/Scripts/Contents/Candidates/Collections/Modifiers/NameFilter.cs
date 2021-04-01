using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;

namespace DopeElections.Candidates
{
    public class NameFilter : CollectionModifier<NameFilter.FilterState>, IFilter<Candidate>
    {
        public LocalizationKey Label => LKey.Components.CandidateCollection.Filter.Name;

        protected override FilterState DefaultState => new FilterState();

        public bool Active => !string.IsNullOrWhiteSpace(State.Key);

        public IEnumerable<Candidate> Apply(IEnumerable<Candidate> entries)
        {
            var key = (State.Key ?? "").ToLower();
            var map = entries.ToDictionary(e => e, e => e.firstName + " " + e.lastName);
            var pattern = new Regex("^.*" + key + ".*$");
            return map.Where(e => pattern.IsMatch(e.Value.ToLower())).Select(e => e.Key);
        }

        public class FilterState : CollectionModifierState
        {
            public string Key { get; }

            public FilterState(string key = null)
            {
                Key = key;
            }
        }
    }
}