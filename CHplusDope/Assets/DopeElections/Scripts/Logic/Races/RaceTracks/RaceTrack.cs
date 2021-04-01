using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public class RaceTrack
    {
        public delegate void RaceTrackPartEvent(RaceTrackPartInstance instance);

        public event RaceTrackPartEvent PartAdded = delegate { };
        public event RaceTrackPartEvent PartRemoved = delegate { };

        public RaceTrackPartSet PartsSet { get; }

        private float _minPosition;
        private float _maxPosition;

        private List<RaceTrackPartInstance> _parts = new List<RaceTrackPartInstance>();

        public IReadOnlyList<RaceTrackPartInstance> Parts => _parts;
        public float MinPosition => _minPosition;
        public float MaxPosition => _maxPosition;
        public float Width => PartsSet.Width;
        public float Length => _maxPosition;

        public IRaceTrackGenerator MainGenerator { get; set; }

        public RaceTrack(RaceTrackPartSet parts)
        {
            PartsSet = parts;
        }

        public void AddPart(RaceTrackPartInstance part)
        {
            _parts.Add(part);
            Recalculate();
            PartAdded(part);
        }

        public void InsertPart(int index, RaceTrackPartInstance part)
        {
            _parts.Insert(index, part);
            Recalculate();
            PartAdded(part);
        }

        public void RemovePart(RaceTrackPartInstance part)
        {
            _parts.Remove(part);
            Recalculate();
            PartRemoved(part);
        }

        public void RemovePartAt(int index)
        {
            if (index < 0 || index >= _parts.Count) return;
            var p = _parts[index];
            _parts.RemoveAt(index);
            Recalculate();
            PartRemoved(p);
        }

        public void ClearParts()
        {
            foreach (var p in _parts) PartRemoved(p);
            _parts.Clear();
            Recalculate();
        }

        private void Recalculate()
        {
            _minPosition = _parts.Select(p => p.Position).DefaultIfEmpty(0).Min();
            _maxPosition = _parts.Select(p => p.EndPosition).DefaultIfEmpty(0).Max();
        }

        public RaceTrackPartInstance GetPart(float position)
        {
            var queryPosition = Mathf.Clamp(position, _minPosition, _maxPosition - float.Epsilon);
            return _parts.FirstOrDefault(p => p.Position <= queryPosition && p.EndPosition > queryPosition);
        }

        public RaceTrackPartInstance GetPrevious(RaceTrackPartInstance part)
        {
            var index = _parts.IndexOf(part) - 1;
            return index >= 0 ? _parts[index] : null;
        }

        public RaceTrackVector ToPercentageVector(RaceTrackVector position)
        {
            var y = position.GetPercentageY(0, Length);
            if (position.xType == RaceTrackVector.AxisType.Percentage)
            {
                return new RaceTrackVector(position.x, y, RaceTrackVector.AxisType.Percentage);
            }

            var x = position.GetPercentageX(Width);
            return new RaceTrackVector(x, y, RaceTrackVector.AxisType.Percentage);
        }

        /// <summary>
        /// Returns a position in local space of the race track
        /// </summary>
        public Vector3 GetWorldPosition(RaceTrackVector position)
        {
            var y = position.GetDistanceY(0, Length);
            var width = Width;
            var x = position.GetDistanceX(width);
            return new Vector3(x, 0, y);
        }

        private void EnforceYAxisTypeDistance(RaceTrackVector v)
        {
            if (v.yType == RaceTrackVector.AxisType.Percentage)
            {
                v.y *= _maxPosition;
                v.yType = RaceTrackVector.AxisType.Distance;
            }
        }
    }
}