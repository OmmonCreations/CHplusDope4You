using System;
using UnityEngine;

namespace DopeElections.Races
{
    [Serializable]
    public class CandidateGroupComposition
    {
        [SerializeField] private CandidateGroupLayoutConfiguration _layoutConfiguration = null;
        [SerializeField] private AnimationCurve _dropoutCurve = AnimationCurve.Linear(0, 0, 1, 0.5f);
        [SerializeField] private AnimationCurve _maxCandidatesCurve = AnimationCurve.Linear(0, 1, 1, 0.2f);
        [SerializeField] private int _minCandidates = 20;
        [SerializeField] private int _maxCandidates = 200;
        [SerializeField] private CandidateSubgroupType[] _subgroups = null;

        public CandidateGroupLayoutConfiguration LayoutConfiguration => _layoutConfiguration;
        public AnimationCurve DropoutCurve => _dropoutCurve;
        public AnimationCurve MaxCandidatesCurve => _maxCandidatesCurve;
        public int MinCandidates => _minCandidates;
        public int MaxCandidates => _maxCandidates;
        public CandidateSubgroupType[] Subgroups => _subgroups;

        public int CalculateMaxCandidates(int candidatesCount, float relativeRaceIndex)
        {
            var preferredPercentage = MaxCandidatesCurve.Evaluate(relativeRaceIndex);
            var preferredMaxCandidates = Mathf.RoundToInt(preferredPercentage * candidatesCount);
            return Mathf.Clamp(preferredMaxCandidates, MinCandidates, MaxCandidates);
        }
    }
}