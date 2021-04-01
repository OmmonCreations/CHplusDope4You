using UnityEngine;

namespace Essentials
{
    public static class Vector3Util
    {
        private const int precision = 100;
        
        /**
         * Wandelt einen Vector3 in einen string mit dem Format x,y,z um.
         */
        public static string Serialize(Vector3 v)
        {
            return Mathf.RoundToInt(v.x * precision) + "," + Mathf.RoundToInt(v.y * 100) + "," + Mathf.RoundToInt(v.z * 100);
        }

        /**
         * Wandelt einen string mit dem Format x,y,z in einen Vector3 um.
         */
        public static Vector3 Deserialize(string s)
        {
            if (string.IsNullOrEmpty(s)) return Vector3.zero;
            var parts = s.Split(',');
            var parsed = new float[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                int value;
                if (!int.TryParse(parts[i], out value)) continue;
                parsed[i] = value / (float)precision;
            }

            switch (parsed.Length)
            {
                case 1:
                    return Vector3.one * parsed[0];
                case 2:
                    return new Vector3(parsed[0],parsed[1]);
                default:
                    return new Vector3(parsed[0],parsed[1],parsed[2]);
            }
        }
    }
}