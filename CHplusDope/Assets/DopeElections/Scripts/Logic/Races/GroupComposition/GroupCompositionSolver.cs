using System.Collections.Generic;
using System.Linq;
using Essentials;
using UnityEngine;

namespace DopeElections.Races
{
    public static class GroupCompositionSolver
    {
        public static CandidateSubgroup[] CalculateGroups(RaceContext raceContext,
            IEnumerable<RaceCandidate> candidates, CandidateGroupComposition composition, float relativeRaceIndex,
            float currentRaceProgress)
        {
            return CalculateGroups(raceContext, candidates.ToDictionary(c => c, c => c.AgreementState), composition,
                relativeRaceIndex, currentRaceProgress);
        }

        public static CandidateSubgroup[] CalculateGroups(RaceContext raceContext,
            Dictionary<RaceCandidate, AgreementState> agreementMap, CandidateGroupComposition composition,
            float relativeRaceIndex, float currentRaceProgress)
        {
            var remainingCandidates =
                GetRemainingCandidates(agreementMap, composition, relativeRaceIndex, currentRaceProgress);
            var context = Context.Calculate(raceContext, composition, remainingCandidates);

            var subgroupTypes = context.SubgroupTypes;

            var result = new List<CandidateSubgroup>(subgroupTypes.Count);

            var isFirst = true;

            foreach (var type in subgroupTypes)
            {
                if (!TryCreateSubgroup(context, type, isFirst, out var subgroup)) continue;

                result.Add(subgroup);
                isFirst = false;
            }

#if UNITY_EDITOR
            if (result.Count == 0)
            {
                Debug.LogWarning(context.ToString());
            }
#endif

            return result.ToArray();
        }

        private static bool TryCreateSubgroup(Context context, CandidateSubgroupType type, bool isFirst,
            out CandidateSubgroup subgroup)
        {
            var scoreRange = context.ScoreRange;
            var minGroupAgreement =
                Mathf.Max(type.MinScore, context.MinAgreement + type.AgreementThreshold * scoreRange);
            var candidatesMap = context.Candidates
                .Where(e => e.Value.AgreementScore >= minGroupAgreement && !context.Done.Contains(e.Key))
                .OrderByDescending(e => e.Value.AgreementScore + e.Key.match)
                .ToDictionary(e => e.Key, e => e.Value);
            if (candidatesMap.Count == 0)
            {
                subgroup = null;
                return false;
            }

            var highestAgreement = candidatesMap.Max(c => c.Value.AgreementScore);
            var showLabel = highestAgreement != context.LastHighestAgreement && highestAgreement > 0;

            var lowestMatch = candidatesMap.Min(e => e.Key.match);
            var highestMatch = candidatesMap.Max(e => e.Key.match);
            var matchRange = highestMatch - lowestMatch;
            var minMatch = lowestMatch + matchRange * type.MatchThreshold;

            var categoryMatches = candidatesMap.Select(e => e.Value.CategoryMatch).ToList();
            var groupMinCategoryMatch = categoryMatches.Min();
            var groupMaxCategoryMatch = categoryMatches.Max();

            subgroup = new CandidateSubgroup(
                type,
                candidatesMap
                    .Where(e => matchRange <= 0 || e.Key.match >= minMatch)
                    .Select(e => e.Key)
                    .ToArray(),
                new CandidateSubgroup.GroupMeta(new MinMaxRange(
                    groupMinCategoryMatch,
                    groupMaxCategoryMatch
                ), isFirst ? context.Icon : null, showLabel)
            );

            foreach (var c in subgroup.Candidates) context.Done.Add(c);

            context.LastHighestAgreement = highestAgreement;

            return subgroup.Candidates.Length > 0;
        }

        private static Dictionary<RaceCandidate, AgreementState> GetRemainingCandidates(
            Dictionary<RaceCandidate, AgreementState> agreementMap, CandidateGroupComposition composition,
            float relativeRaceIndex, float currentRaceProgress)
        {
            var totalCandidatesCount = agreementMap.Count;

            var candidatesPercentage = composition.MaxCandidatesCurve.Evaluate(relativeRaceIndex);
            var stayingPercentage = 1 - composition.DropoutCurve.Evaluate(currentRaceProgress);
            var remainingCandidatesPercentage = Mathf.Clamp01(candidatesPercentage * stayingPercentage);
            var preferredRemainingCandidatesCount =
                Mathf.RoundToInt(totalCandidatesCount * remainingCandidatesPercentage);

            var remainingCandidatesCount = Mathf.Clamp(
                preferredRemainingCandidatesCount,
                composition.MinCandidates,
                composition.MaxCandidates
            );

            /*
            Debug.Log("Current Progress: " + currentRaceProgress);
            Debug.Log("Remaining: " + remainingCandidatesCount + "/" + agreementMap.Count + "(Intended: " +
                      Mathf.RoundToInt(remainingCandidatesPercentage * 100) + "%)");
                      */

            return agreementMap
                .OrderByDescending(e =>
                    e.Value.AgreementScore + e.Key.match + Random.value * 0.0001f +
                    (e.Key.IsAlive ? 1 : 0) * 0.0002f)
                .Take(remainingCandidatesCount)
                .ToDictionary(e => e.Key, e => e.Value);
        }

        private class Context
        {
            public IReadOnlyDictionary<RaceCandidate, AgreementState> Candidates { get; }
            public HashSet<RaceCandidate> Done { get; } = new HashSet<RaceCandidate>();

            public IReadOnlyList<CandidateSubgroupType> SubgroupTypes { get; private set; }

            public Sprite Icon { get; private set; }

            /// <summary>
            /// The lowest agreement score found in the agreement map
            /// </summary>
            public int MinAgreement { get; private set; }

            /// <summary>
            /// The highest agreement score found in the agreement map
            /// </summary>
            public int MaxAgreement { get; private set; }

            public int LastHighestAgreement { get; set; } = int.MaxValue;

            public int ScoreRange => MaxAgreement - MinAgreement;

            private Context(IReadOnlyDictionary<RaceCandidate, AgreementState> candidates)
            {
                Candidates = candidates;
            }

            public override string ToString()
            {
                return SubgroupTypes.Count + " candidate group types available. \n" +
                       "Groups: " + string.Join(", ", SubgroupTypes.Select(t => t.name)) + "\n" +
                       "Min Agreement: " + MinAgreement + "\n" +
                       "Max Agreement: " + MaxAgreement + "\n" +
                       "Agreement Map:\n" +
                       string.Join("\n", Candidates.Select(e => "- " + e.Key.id + ": " + e.Value));
            }

            public static Context Calculate(RaceContext raceContext, CandidateGroupComposition composition,
                Dictionary<RaceCandidate, AgreementState> candidates)
            {
                var minAgreement = candidates.Select(e => e.Value.AgreementScore).DefaultIfEmpty(0).Min();
                var matchValues = candidates.Select(e => e.Key.match).DefaultIfEmpty(0).ToList();
                var maxAgreement = candidates.Values.Select(v => v.AgreementScore).DefaultIfEmpty(0).Max();
                var highestOverallMatch = matchValues.Max();

                var subgroupTypes = composition.Subgroups
                    .Where(t => t.MinScore <= maxAgreement && t.MinMatch <= highestOverallMatch)
                    .ToList();
                var raceIcon = raceContext.Race.IconOutline;

                return new Context(candidates)
                {
                    Icon = raceIcon,
                    SubgroupTypes = subgroupTypes,
                    MinAgreement = minAgreement,
                    MaxAgreement = maxAgreement
                };
            }
        }
    }
}