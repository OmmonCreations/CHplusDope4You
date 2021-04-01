using System;

namespace Localizator
{
    public struct LocalizationKey : IComparable<LocalizationKey>
    {
        public readonly string path;
        public string fallback;
        public readonly string[] placeholders;

        public LocalizationKey(string path, params string[] placeholders)
        {
            this.path = path;
            fallback = "??????";
            this.placeholders = placeholders;
        }

        public override string ToString()
        {
            return path + (fallback != null ? " (" + fallback + ")" : "");
        }

        public static bool operator ==(LocalizationKey a, LocalizationKey b)
        {
            return a.path == b.path && a.fallback == b.fallback;
        }

        public static bool operator !=(LocalizationKey a, LocalizationKey b)
        {
            return !(a == b);
        }

        public int CompareTo(LocalizationKey other)
        {
            var pathComparison = string.Compare(path, other.path, StringComparison.Ordinal);
            if (pathComparison != 0) return pathComparison;
            return string.Compare(fallback, other.fallback, StringComparison.Ordinal);
        }

        public bool Equals(LocalizationKey other)
        {
            return path == other.path && fallback == other.fallback;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalizationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((path != null ? path.GetHashCode() : 0) * 397) ^ (fallback != null ? fallback.GetHashCode() : 0);
            }
        }
    }
}