using System;
using Newtonsoft.Json.Linq;

namespace Essentials
{
    public readonly struct NamespacedKey
    {
        public string Namespace { get; }
        public string Key { get; }
        
        public NamespacedKey(string nameSpace, string key)
        {
            Namespace = nameSpace;
            Key = key;
        }

        public JToken Serialize()
        {
            return ToString();
        }
        
        public override string ToString()
        {
            return Namespace + ":" + Key;
        }

        public override bool Equals(object obj)
        {
            return obj is NamespacedKey k && Equals(k);
        }

        public bool Equals(NamespacedKey other)
        {
            return Namespace == other.Namespace && Key == other.Key;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Namespace != null ? Namespace.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
            }
        }

        public static NamespacedKey Builtin(string value)
        {
            return new NamespacedKey("builtin", value);
        }

        public static NamespacedKey? Of(JToken token)
        {
            return token != null ? Of((string) token) : new NamespacedKey?();
        }

        public static NamespacedKey? Of(string fullKey)
        {
            var parts = fullKey.Split(':');
            return parts.Length == 2 ? new NamespacedKey(parts[0], parts[1]) : new NamespacedKey?();
        }

        public static bool TryParse(JToken token, out NamespacedKey key)
        {
            var result = Of(token);
            key = result.HasValue ? result.Value : default;
            return result.HasValue;
        }
        
        public static bool operator ==(NamespacedKey a, NamespacedKey b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NamespacedKey a, NamespacedKey b)
        {
            return !(a == b);
        }
    }
}