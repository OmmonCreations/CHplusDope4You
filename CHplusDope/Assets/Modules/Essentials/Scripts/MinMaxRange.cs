using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Essentials
{
    public struct MinMaxRange
    {
        public float min;
        public float max;

        public MinMaxRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Evaluate(float t) => min + (max - min) * Mathf.Clamp01(t);

        public bool IsMatch(float f)
        {
            return f>=min && f <= max;
        }

        public float Clamp(float f)
        {
            return Mathf.Clamp(f, min, max);
        }

        public JToken Serialize()
        {
            return Math.Abs(min - max) > 0
                ? new JObject()
                {
                    ["min"] = min,
                    ["max"] = max
                }
                : (JToken) min;
        }
        
        public static bool TryParse(JToken json, out MinMaxRange range)
        {
            if (json == null)
            {
                range = default;
                return false;
            }

            if (json.Type == JTokenType.Object)
            {
                return TryParse((JObject) json, out range);
            }

            var value = (float) json;
            range.min = value;
            range.max = value;
            return true;
        }

        public static bool TryParse(JObject json, out MinMaxRange range)
        {
            if (json == null)
            {
                range = default;
                return false;
            }

            range.min = json["min"] != null ? (float) json["min"] : 0;
            range.max = json["max"] != null ? (float) json["max"] : 0;
            return true;
        }
        
        public bool Equals(MinMaxRange other)
        {
            return min.Equals(other.min) && max.Equals(other.max);
        }

        public override bool Equals(object obj)
        {
            return obj is MinMaxRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (min.GetHashCode() * 397) ^ max.GetHashCode();
            }
        }

        public static bool operator ==(MinMaxRange a, MinMaxRange b)
        {
            return Math.Abs(a.min - b.min) <= 0 && Math.Abs(a.max - b.max) <= 0;
        }

        public static bool operator !=(MinMaxRange a, MinMaxRange b)
        {
            return !(a == b);
        }
    }
}