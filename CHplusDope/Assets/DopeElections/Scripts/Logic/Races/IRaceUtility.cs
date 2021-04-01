using System.Collections.Generic;
using System.Linq;

namespace DopeElections.Races
{
    public static class RaceUtility
    {
        public static IEnumerable<RaceCandidate> GetBestCandidates(this IRace race, int count)
        {
            return race.Candidates
                .OrderByDescending(c => c.AgreementScore + c.match)
                .Take(count);
        }   
    }
}