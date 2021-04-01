using UnityEngine;

namespace Essentials
{
    public static class QuaternionUtil
    {
        private const int precision = 100;

        /**
         * Wandelt einen Quaternion in einen string mit dem Format x,y,z,w um.
         */
        public static string Serialize(Quaternion q)
        {
            return Mathf.RoundToInt(q.x * precision) + "," + Mathf.RoundToInt(q.y * 100) + "," +
                   Mathf.RoundToInt(q.z * 100) + "," + Mathf.RoundToInt(q.w * 100);
        }

        /**
         * Wandelt einen string mit dem Format x,y,z,w in einen Quaternion um.
         */
        public static Quaternion Deserialize(string s)
        {
            if (string.IsNullOrEmpty(s)) return Quaternion.identity;
            var parts = s.Split(',');
            var parsed = new float[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                int value;
                if (!int.TryParse(parts[i], out value)) continue;
                parsed[i] = value / (float) precision;
            }

            return parts.Length < 4 ? Quaternion.identity : new Quaternion(parsed[0], parsed[1], parsed[2], parsed[3]);
        }

        public static bool IsEmpty(this Quaternion q)
        {
            return Mathf.Abs(q.x) + Mathf.Abs(q.y) + Mathf.Abs(q.z) + Mathf.Abs(q.w) <= 0;
        }
    }
}