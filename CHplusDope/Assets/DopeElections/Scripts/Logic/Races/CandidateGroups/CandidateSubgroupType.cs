using System.Collections.Generic;
using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    [CreateAssetMenu(fileName = "CandidateSubgroup", menuName = "Dope Elections/Candidate Subgroup")]
    public class CandidateSubgroupType : ScriptableObject
    {
        [Tooltip("Fixed minimum score threshold for the group to be visible")] [SerializeField]
        private int _minScore = 0;

        [Tooltip("Fixed minimum overall match percentage")] [SerializeField]
        private float _minMatch = 0;

        [Tooltip("Minimum category agreement percentage based on the current agreement range in the race")]
        [SerializeField]
        private float _agreementThreshold = 0;

        [Tooltip("Minimum relative match percentage within candidates of equal agreement, " +
                 "0 means lowest match percentage, 1 means highest match percentage.")]
        [SerializeField]
        private float _matchThreshold = 0;

        [SerializeField] private string _name = "";
        [SerializeField] private Color _color = Color.white;

        public int MinScore => _minScore;
        public float MinMatch => _minMatch;
        public float AgreementThreshold => _agreementThreshold;
        public float MatchThreshold => _matchThreshold;
        public LocalizationKey Name => new LocalizationKey(_name) {fallback = _name};
        public LocalizationKey MatchString => new LocalizationKey {fallback = "{value}%"};
        public LocalizationKey MatchRangeString => new LocalizationKey {fallback = "{min}% - {max}%"};
        public Color Color => _color;
        
        public string GetMatchString(ILocalization localization, float minMatch, float maxMatch)
        {
            var minMatchDisplayValue = Mathf.RoundToInt(minMatch * 100);
            var maxMatchDisplayValue = Mathf.RoundToInt(maxMatch * 100);
            
            if (minMatchDisplayValue == maxMatchDisplayValue)
            {
                return LocalizationUtility.ApplyReplacements(localization.GetString(MatchString),
                    new[]
                    {
                        new KeyValuePair<string, string>("value", minMatchDisplayValue.ToString())
                    });
            }
            
            return LocalizationUtility.ApplyReplacements(localization.GetString(MatchRangeString),
                new[]
                {
                    new KeyValuePair<string, string>("min", minMatchDisplayValue.ToString()),
                    new KeyValuePair<string, string>("max", maxMatchDisplayValue.ToString())
                });
        }
    }
}