using Essentials;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Data object that groups race candidates by their shared category agreement score
    /// </summary>
    public class CandidateSubgroup
    {
        public CandidateSubgroupType Type { get; }
        public RaceCandidate[] Candidates { get; }
        public GroupMeta Meta { get; }

        public MinMaxRange Match => Meta.Match;
        public Sprite Icon => Meta.Icon;
        public bool ShowLabel => Meta.ShowLabel;

        public CandidateSubgroup(CandidateSubgroupType type, RaceCandidate[] candidates, GroupMeta meta)
        {
            Type = type;
            Candidates = candidates;
            Meta = meta;
        }

        public class GroupMeta
        {
            public MinMaxRange Match { get; }
            public Sprite Icon { get; }
            public bool ShowLabel { get; }
            
            public GroupMeta(MinMaxRange match, Sprite icon, bool showLabel)
            {
                Match = match;
                Icon = icon;
                ShowLabel = showLabel;
            }
        }
    }
}