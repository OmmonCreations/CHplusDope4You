using System.Linq;
using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Data object to arrange candidates in a grid like fashion
    /// </summary>
    public class CandidateGroupLayout
    {
        public CandidateGroupLayoutConfiguration Configuration { get; }
        public CandidateSlotRow[] Rows { get; }
        public float Length { get; }
        private readonly RaceTrackVector[][] _groupAnchors;
        public int LargestRow { get; }

        public CandidateGroupLayout(CandidateGroupLayoutConfiguration configuration, CandidateSlotRow[] rows, float length)
        {
            Configuration = configuration;
            Rows = rows;
            Length = length;
            LargestRow = rows.Select(r=>r.Candidates.Length).DefaultIfEmpty(0).Max();
            var groupAnchors = new RaceTrackVector[rows.Length][];
            for (var y = 0; y < rows.Length; y++)
            {
                var rowSize = rows[y].Candidates.Length;
                groupAnchors[y] = new RaceTrackVector[rowSize];
            }

            _groupAnchors = groupAnchors;
        }

        public RaceTrackVector[][] CalculateAnchors(float position)
        {
            var slotSize = Configuration.SlotSize;
            var rows = Rows;
            var output = _groupAnchors;
            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                var candidates = row.Candidates;
                var rowPosition = position + row.RelativeForwardPosition;
                var slotCount = candidates.Length;
                var xCenter = slotCount / 2f;
                for (var x = 0; x < slotCount; x++)
                {
                    var candidate = candidates[x];
                    if (candidate == null) continue;

                    var columnPosition = (x + 0.5f - xCenter) * slotSize;
                    output[y][x] = new RaceTrackVector(columnPosition, rowPosition, RaceTrackVector.AxisType.Distance);
                }
            }

            return output;
        }

        public RaceTrackVector CreateOffset(CandidateSlot slot)
        {
            var layoutConfiguration = Configuration;
            var slotSize = layoutConfiguration.SlotSize;
            var row = slot.y >= 0 && slot.y < Rows.Length ? Rows[slot.y] : null;
            if (row == null)
            {
                Debug.LogError("Invalid slot: "+slot);
                return default;
            }
            var rowSize = row.Candidates.Length;
            var offsetRadius = slotSize * layoutConfiguration.RandomOffsetFraction;
            var minX = slot.x <= 0 ? 0 : float.MinValue;
            var maxX = slot.x >= rowSize - 1 ? 0 : float.MaxValue;
            return new RaceTrackVector(
                Mathf.Clamp((Random.value * 2 - 1) * offsetRadius, minX, maxX),
                (Random.value * 2 - 1) * offsetRadius, RaceTrackVector.AxisType.Distance
            );
        }

        public void ApplySlots()
        {
            for (var y = 0; y < Rows.Length; y++)
            {
                var row = Rows[y];
                var candidates = row.Candidates;
                for (var x = 0; x < candidates.Length; x++)
                {
                    var candidate = candidates[x];
                    if (candidate == null) continue;
                    candidate.Slot = new CandidateSlot(x, y);
                }
            }
        }
    }
}