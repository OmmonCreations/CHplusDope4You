using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using Localizator;
using SortableCollections;
using UnityEngine;

namespace DopeElections.Candidates
{
    public class CandidateCollection : SortableCollection<Candidate>
    {
        public CandidateCollection(Candidate[] candidates, params CollectionModifier[] modifiers) : base(candidates,
            modifiers.Concat(new CollectionModifier[]
            {
                new NameSortingOrder(),
                new MatchSortingOrder(),
                new AgeSortingOrder(),
                new PartySortingOrder(),
                new PartyFilter(),
                new NameFilter()
            }).ToArray()
        )
        {
        }

        public CandidateSection[] GetCandidatesAsSections()
        {
            switch (SortingOrder)
            {
                case SortingOrderId.Age: return GetAgeSections(FilteredEntries);
                case SortingOrderId.Match: return GetMatchSections(FilteredEntries);
                case SortingOrderId.Party: return GetPartySections(FilteredEntries);
                default: return GetAlphabeticalSections(FilteredEntries);
            }
        }

        private CandidateSection[] GetAlphabeticalSections(Candidate[] candidates)
        {
            var mapped = new Dictionary<string, List<Candidate>>();
            foreach (var c in candidates)
            {
                var letter = c.lastName.Length > 0 ? c.lastName[0].ToString().ToUpper() : "";
                if (!mapped.ContainsKey(letter)) mapped[letter] = new List<Candidate>();
                mapped[letter].Add(c);
            }

            return mapped
                .Select(e => new CandidateSection(new LocalizationKey {fallback = e.Key}, e.Value.ToArray()))
                .ToArray();
        }

        private CandidateSection[] GetAgeSections(Candidate[] candidates)
        {
            var mapped = new Dictionary<int, List<Candidate>>();
            var year = System.DateTime.Now.Year;
            foreach (var c in candidates)
            {
                var age = Mathf.FloorToInt((year - c.birthYear) / 10f);
                if (!mapped.ContainsKey(age)) mapped[age] = new List<Candidate>();
                mapped[age].Add(c);
            }

            return mapped
                .Where(e => e.Key > 0)
                .Select(e =>
                    new CandidateSection(
                        new LocalizationKey {fallback = GetAgeRangeString(e.Key * 10 + 1, (e.Key + 1) * 10)},
                        e.Value.ToArray()))
                .ToArray();
        }

        private string GetAgeRangeString(int min, int max)
        {
            if (max <= 20) return $"< {max}";
            return $"{min} - {max}";
        }

        private CandidateSection[] GetMatchSections(Candidate[] candidates)
        {
            var mapped = new Dictionary<int, List<Candidate>>();
            foreach (var c in candidates)
            {
                var match = Mathf.FloorToInt(GetMatch(c) / 10f);
                if (!mapped.ContainsKey(match)) mapped[match] = new List<Candidate>();
                mapped[match].Add(c);
            }

            return mapped
                .Select(e => new CandidateSection(
                    new LocalizationKey {fallback = GetMatchRangeString(e.Key * 10 + 1, (e.Key + 1) * 10)},
                    e.Value.ToArray()))
                .ToArray();
        }

        private float GetMatch(Candidate c)
        {
            return 0;
        }

        private string GetMatchRangeString(int min, int max)
        {
            return $"{min}% - {max}%";
        }

        private CandidateSection[] GetPartySections(Candidate[] candidates)
        {
            var groups = candidates.GroupBy(c => c.partyId);

            return (
                    Modifiers.OfType<PartySortingOrder>().First().State.Reverse
                        ? groups.OrderByDescending(g => g.Key)
                        : groups.OrderBy(g => g.Key)
                )
                .Select(g => new CandidateSection(new LocalizationKey {fallback = g.Key.ToString()}, g.ToArray()))
                .ToArray();
        }
    }
}