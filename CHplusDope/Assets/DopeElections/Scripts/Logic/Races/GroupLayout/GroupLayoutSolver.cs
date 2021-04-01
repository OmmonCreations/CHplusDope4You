using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DopeElections.Races.GroupLayout
{
    public static class GroupLayoutSolver
    {
        private static readonly Vector2Int[] SlotSearchDirectionsA =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        private static readonly Vector2Int[] SlotSearchDirectionsB =
        {
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
        };

        public static CandidateGroupLayout CalculateLayoutFixedWidth(CandidateGroup group, float width,
            IEnumerable<CandidateSubgroup> groups)
        {
            var rows = new List<CandidateSlotRow>();
            var layoutConfiguration = group.LayoutConfiguration;
            var groupSpacing = layoutConfiguration.GroupSpacing;
            var currentPosition = 0f;
            foreach (var g in groups)
            {
                rows.AddRange(CalculateRowsFixedWidth(group, width, ref currentPosition, g));
                currentPosition -= groupSpacing;
            }

            var endPosition = currentPosition + groupSpacing;
            var length = Mathf.Abs(endPosition);
            return new CandidateGroupLayout(layoutConfiguration, rows.ToArray(), length);
        }

        private static IEnumerable<CandidateSlotRow> CalculateRowsFixedWidth(CandidateGroup group, float width,
            ref float position,
            CandidateSubgroup subgroup)
        {
            var layoutConfiguration = group.LayoutConfiguration;
            var slotSize = layoutConfiguration.SlotSize;
            var candidates = subgroup.Candidates;

            var emptySlotPercentage = layoutConfiguration.EmptySlotPercentage;

            var columnCount = Mathf.FloorToInt(width / slotSize);
            var rowCount = Mathf.CeilToInt(candidates.Length * (1 + emptySlotPercentage) / columnCount);
            var subgroupSize = new Vector2Int(columnCount, rowCount);

            var previousCandidatesPerRow = group.Layout != null ? group.Layout.LargestRow : -1;

            var slotMap = new Dictionary<Vector2Int, RaceCandidate>();
            foreach (var candidate in candidates)
            {
                var currentSlot = candidate.Slot;
                var currentColumn = currentSlot.x;
                var currentRelativeColumn = previousCandidatesPerRow >= 0
                    ? currentColumn / (float) previousCandidatesPerRow
                    : Random.value;
                var preferredColumn = Mathf.Clamp(
                    Mathf.FloorToInt(currentRelativeColumn * columnCount), 0, subgroupSize.x - 1
                );
                var slot = GetClosestEmptySlot(slotMap, preferredColumn, subgroupSize);
                slotMap.Add(slot, candidate);
            }

            var result = new CandidateSlotRow[rowCount];
            for (var y = 0; y < rowCount; y++)
            {
                var slots = new RaceCandidate[columnCount];
                for (var x = 0; x < columnCount; x++)
                {
                    var slot = new Vector2Int(x, y);
                    if (!slotMap.TryGetValue(slot, out var c)) continue;
                    slots[x] = c;
                }

                var row = new CandidateSlotRow(position, subgroup, slots);
                result[y] = row;
                position -= slotSize;
            }

            var leftoverCandidates = subgroup.Candidates
                .Except(result.SelectMany(r => r.Candidates.Where(c => c != null))).ToList();
            if (leftoverCandidates.Count > 0)
            {
                var leftoverEntries = leftoverCandidates.Select(c => c.fullName + slotMap
                    .Where(e => e.Value == c)
                    .Select(e => e.Key)
                    .DefaultIfEmpty(new Vector2Int(-1, -1))
                    .First()
                );
                Debug.LogError("Not all candidates received a slot in " + subgroupSize + "!\n" +
                               string.Join("\n", leftoverEntries));
            }

            return result;
        }

        /// <summary>
        /// Attempts to find the closest slot in the desired column. Spreads out in a circular pattern if
        /// the preferred slot is occupied
        /// </summary>
        private static Vector2Int GetClosestEmptySlot(IReadOnlyDictionary<Vector2Int, RaceCandidate> slotMap,
            int preferredColumn, Vector2Int subgroupSize)
        {
            var preferredRow = Mathf.Clamp(Mathf.FloorToInt(
                (Random.value > 0.5f
                    ? 1 - Mathf.Pow(Random.value, 1.2f) / 2
                    : Mathf.Pow(Random.value, 1.2f) / 2
                ) * subgroupSize.y), 0, subgroupSize.y - 1);
            var preferredSlot = new Vector2Int(preferredColumn, preferredRow);
            if (!slotMap.ContainsKey(preferredSlot)) return preferredSlot;

            var queue = new Queue<Vector2Int>();
            var done = new HashSet<Vector2Int>();
            queue.Enqueue(preferredSlot);
            done.Add(preferredSlot);
            var iteration = 0;
            var directions = Random.value > 0.5f ? SlotSearchDirectionsA : SlotSearchDirectionsB;
            var maxIterations = subgroupSize.x * subgroupSize.y;
            while (iteration < maxIterations && queue.Count > 0)
            {
                iteration++;
                var current = queue.Dequeue();
                foreach (var d in directions)
                {
                    var relative = current + d;
                    if (relative.x < 0 || relative.y < 0 || relative.x >= subgroupSize.x ||
                        relative.y >= subgroupSize.y)
                    {
                        continue;
                    }

                    if (!slotMap.ContainsKey(relative)) return relative;
                    if (done.Contains(relative)) continue;
                    queue.Enqueue(relative);
                    done.Add(relative);
                }
            }

            Debug.LogError("No empty slot found!");
            return new Vector2Int(-1, -1);
        }

        #region Obsolete

        /*

        public static bool TestLayout(CandidateGroup group, float position, CandidateGroupLayout layout)
        {
            var raceTrack = group.RaceTrack;
            var layoutConfiguration = group.LayoutConfiguration;
            var currentPosition = position;
            var slotSize = layoutConfiguration.SlotSize;
            foreach (var r in layout.Rows)
            {
                var candidateCount = r.Candidates.Length;
                var segment = raceTrack.GetMainSegment(currentPosition);
                var trackWidth = segment != null ? segment.GetMinWidth() : 0;
                var requiredWidth = candidateCount * slotSize;
                var tooSmall = requiredWidth > trackWidth + 0.0001f;
                // prevent track width from being exact integer, floating point imprecision can cause flickering
                if (!tooSmall) continue;
                return false;
            }

            return true;
        }
        public static CandidateGroupLayout CalculateLayout(CandidateGroup group, float position,
            CandidateSubgroup[] groups)
        {
            var raceTrack = group.RaceTrack;
            var layoutConfiguration = group.LayoutConfiguration;

            var rows = new List<CandidateSlotRow>();
            var groupSpacing = layoutConfiguration.GroupSpacing;

            var currentPosition = position;
            if (raceTrack.MaxPosition < position) raceTrack.MainGenerator.CreateParts(position);

            foreach (var g in groups.OrderByDescending(g => g.RequiredAgreementScore))
            {
                rows.AddRange(CalculateRows(group, position, ref currentPosition, g));
                currentPosition -= groupSpacing;
            }

            var endPosition = currentPosition + groupSpacing;
            var length = Mathf.Abs(position - endPosition);
            return new CandidateGroupLayout(layoutConfiguration, rows.ToArray(), length);
        }
        
        private static IEnumerable<CandidateSlotRow> CalculateRows(CandidateGroup group, float startPosition,
            ref float position,
            CandidateSubgroup g)
        {
            var raceTrack = group.RaceTrack;
            var layoutConfiguration = group.LayoutConfiguration;
            var segment = raceTrack.GetMainSegment(position);
            var trackWidth = segment != null ? segment.GetMinWidth() : 0;
            var slotSize = layoutConfiguration.SlotSize;
            var candidates = g.Candidates;
            var maxCandidates = Mathf.FloorToInt(trackWidth / slotSize);
            var totalCandidates = candidates.Length;
            var remainingSlots = totalCandidates;

            var result = new List<CandidateSlotRow>();
            var slots = new RaceCandidate[Mathf.Min(remainingSlots, maxCandidates)];
            var column = 0;
            var step = 1;
            var reverse = false;
            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (column < 0 || column >= slots.Length)
                {
                    Debug.LogWarning("Encountered track width that is not large enough for candidates! (track width: " +
                                     trackWidth + ")");
                    return result;
                }

                slots[column] = candidate;
                column += step;
                if (column < maxCandidates && column >= 0) continue;

                result.Add(new CandidateSlotRow(position - startPosition, g.RequiredAgreementScore, slots));
                position -= slotSize;
                segment = raceTrack.GetMainSegment(position);
                trackWidth = segment != null ? segment.GetMinWidth() : 0;
                remainingSlots = totalCandidates - (i + 1);
                maxCandidates = Mathf.Min(remainingSlots, Mathf.FloorToInt(trackWidth / slotSize));
                reverse = !reverse;
                column = reverse ? maxCandidates - 1 : 0;
                step = reverse ? -1 : 1;
                slots = new RaceCandidate[maxCandidates];
            }

            if (column != 0 && column != maxCandidates - 1)
            {
                result.Add(new CandidateSlotRow(position - startPosition, g.RequiredAgreementScore, slots));
                position -= slotSize;
            }

            return result;
        }

        private static IEnumerable<CandidateSlotRow> _CalculateRowsFixedWidth(CandidateGroup group, float width,
            ref float position,
            CandidateSubgroup g)
        {
            var layoutConfiguration = group.LayoutConfiguration;
            var slotSize = layoutConfiguration.SlotSize;
            var candidates = g.Candidates;
            var maxCandidates = Mathf.FloorToInt(width / slotSize);
            var totalCandidates = candidates.Length;
            var remainingSlots = totalCandidates;

            var result = new List<CandidateSlotRow>();
            var slots = new RaceCandidate[Mathf.Min(remainingSlots, maxCandidates)];
            var column = 0;
            var step = 1;
            var reverse = false;
            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (column < 0 || column >= slots.Length)
                {
                    Debug.LogWarning("Encountered track width that is not large enough for candidates! (track width: " +
                                     width + ")");
                    return result;
                }

                slots[column] = candidate;
                column += step;
                if (column < maxCandidates && column >= 0) continue;

                result.Add(new CandidateSlotRow(position, g.RequiredAgreementScore, slots));
                position -= slotSize;
                remainingSlots = totalCandidates - (i + 1);
                maxCandidates = Mathf.Min(remainingSlots, Mathf.FloorToInt(width / slotSize));
                reverse = !reverse;
                column = reverse ? maxCandidates - 1 : 0;
                step = reverse ? -1 : 1;
                slots = new RaceCandidate[maxCandidates];
            }

            if (column != 0 && column != maxCandidates - 1)
            {
                result.Add(new CandidateSlotRow(position, g.RequiredAgreementScore, slots));
                position -= slotSize;
            }

            return result;
        }*/

        #endregion
    }
}