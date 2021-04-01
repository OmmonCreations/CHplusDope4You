using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public struct RaceTrackVector
    {
        /// <summary>
        /// Position perpendicular to the movement direction, greater value means further left
        /// </summary>
        public float x;
        /// <summary>
        /// Position in line with the movement direction, greater value means further ahead
        /// </summary>
        public float y;

        public AxisType xType;
        public AxisType yType;

        public RaceTrackVector(RaceTrackVector template)
        {
            x = template.x;
            y = template.y;
            xType = template.xType;
            yType = template.yType;
        }
        
        /// <summary>
        /// Creates a new RaceTrackVector with both axis types set to provided type
        /// </summary>
        public RaceTrackVector(float x, float y, AxisType xyType)
        {
            this.x = x;
            this.y = y;
            xType = xyType;
            yType = xyType;
        }

        /// <summary>
        /// Creates a new RaceTrackVector with both axis types set to provided types
        /// </summary>
        public RaceTrackVector(float x, float y, AxisType xType, AxisType yType)
        {
            this.x = x;
            this.y = y;
            this.xType = xType;
            this.yType = yType;
        }

        public float GetPercentageX(float width)
        {
            return xType == AxisType.Percentage ? x : x / width + 0.5f;
        }

        public float GetPercentageY(float position, float length)
        {
            return yType == AxisType.Percentage ? y : (y - position) / length;
        }

        public float GetDistanceX(float width)
        {
            return xType == AxisType.Distance ? x : (x - 0.5f) * width;
        }

        public float GetDistanceY(float position, float length)
        {
            return yType == AxisType.Distance ? y : y * length + position;
        }

        public RaceTrackVector ToDistanceVector(float position, float width, float length)
        {
            return new RaceTrackVector(GetDistanceX(width), GetDistanceY(position, length),
                AxisType.Distance);
        }

        public override string ToString()
        {
            return $"({x},{y},x:{xType},y:{yType})";
        }

        /// <summary>
        /// Interpolates between two race track vectors and keeps the axis types of the first.
        /// </summary>
        public static RaceTrackVector Lerp(RaceTrackVector a, RaceTrackVector b, float t)
        {
            return new RaceTrackVector(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), a.xType, a.yType);
        }

        /// <summary>
        /// Interpolates between two race track vectors and keeps the axis types of the first.
        /// </summary>
        public static RaceTrackVector MoveTowards(RaceTrackVector a, RaceTrackVector b, float maximumDistance)
        {
            if (a.xType == AxisType.Percentage || b.xType == AxisType.Percentage)
                throw new InvalidOperationException("MoveTowards only works with distance vectors!");
            var c = Vector2.MoveTowards(new Vector2(a.x, a.y), new Vector2(b.x, b.y), maximumDistance);
            return new RaceTrackVector(c.x, c.y, AxisType.Distance);
        }
        
        public static RaceTrackVector operator +(RaceTrackVector a, RaceTrackVector b)
        {
            return new RaceTrackVector(a.x + b.x, a.y + b.y, a.xType, a.yType);
        }
        
        public static RaceTrackVector operator -(RaceTrackVector a, RaceTrackVector b)
        {
            return new RaceTrackVector(a.x - b.x, a.y - b.y, a.xType, a.yType);
        }

        public static bool operator ==(RaceTrackVector a, RaceTrackVector b)
        {
            return Math.Abs(a.x - b.x) <= 0 && Math.Abs(a.y - b.y) <= 0 && a.xType == b.xType && a.yType == b.yType;
        }

        public static bool operator !=(RaceTrackVector a, RaceTrackVector b)
        {
            return !(a == b);
        }

        public bool Equals(RaceTrackVector other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && xType == other.xType && yType == other.yType;
        }

        public override bool Equals(object obj)
        {
            return obj is RaceTrackVector other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) xType;
                hashCode = (hashCode * 397) ^ (int) yType;
                return hashCode;
            }
        }
        
        /// <summary>
        /// Indicates how a value is to be interpreted in the race track coordinate system
        /// </summary>
        public enum AxisType
        {
            /// <summary>
            /// Indicates a value is to be interpreted as a metric distance on the race track coordinate system.
            /// Examples include distance in meters from the start of the track or distance in meters from the center
            /// when looking at the sideways position.
            /// </summary>
            Distance,
            /// <summary>
            /// Indicates a value is to be interpreted as a percentage of a length on the race track coordinate system.
            /// Examples include percentage forward position on a segment or percentage sideways position.
            /// </summary>
            Percentage
        }
    }
}