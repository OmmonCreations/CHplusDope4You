using System.Collections.Generic;
using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    public class SubgroupLeaderboardViewData
    {
        public LocalizationKey Match { get; }
        public Sprite Icon { get; }
        public LocalizationKey MatchType { get; }
        public Dictionary<RaceCandidate, float> Candidates { get; }

        public SubgroupLeaderboardViewData(LocalizationKey match, Sprite icon, LocalizationKey matchType, Dictionary<RaceCandidate, float> candidates)
        {
            Match = match;
            Icon = icon;
            MatchType = matchType;
            Candidates = candidates;
        }
    }
}