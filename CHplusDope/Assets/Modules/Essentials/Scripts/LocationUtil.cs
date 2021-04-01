using Newtonsoft.Json.Linq;

namespace Essentials
{
    public static class LocationUtil
    {
        /**
         * Wandelt eine Location in Json um. Der Vector wird im Feld 'v' serialisiert, der Quaternion
         * im Feld 'r'.
         */
        public static JToken Serialize(Location location)
        {
            JToken result = new JObject();
            result["p"] = Vector3Util.Serialize(location.position);
            result["r"] = QuaternionUtil.Serialize(location.rotation);
            return result;
        }

        /**
         * List eine Location aus Json. Erwartet wird ein Feld 'v' mit einem serialisierten Vector3 und
         * ein Feld 'r' mit einem serialisierten Quaternion
         */
        public static Location Deserialize(JToken jsonData)
        {
            var v = Vector3Util.Deserialize(jsonData["p"] != null ? (string) jsonData["p"] : "");
            var r = QuaternionUtil.Deserialize(jsonData["r"] != null ? (string) jsonData["r"] : "");
            return new Location(v,r);
        }
    }
}