using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Slot coordinate within candidate group
    /// </summary>
    public readonly struct CandidateSlot
    {
        /// <summary>
        /// Slot x relative to track forward, where x=0 is on the right in movement direction
        /// </summary>
        public int x { get; }

        /// <summary>
        /// Slot y where y=0 is the front row in movement direction
        /// </summary>
        public int y { get; }

        public CandidateSlot(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(CandidateSlot a, CandidateSlot b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(CandidateSlot a, CandidateSlot b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(CandidateSlot a, CandidateSlot b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(CandidateSlot a, CandidateSlot b)
        {
            return !(a == b);
        }

        public bool Equals(CandidateSlot other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is CandidateSlot other && Equals(other);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
    }
}