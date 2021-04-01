using System;
using Pagination;

namespace DopeElections.Races
{
    public class CandidateEntry : PaginatedViewEntry
    {
        public RaceCandidate Candidate { get; }
        public float CategoryMatch { get; }
        public Action SelectAction { get; }

        public CandidateEntry(RaceCandidate candidate, float categoryMatch, Action selectAction)
        {
            Candidate = candidate;
            CategoryMatch = categoryMatch;
            SelectAction = selectAction;
        }
    }
}