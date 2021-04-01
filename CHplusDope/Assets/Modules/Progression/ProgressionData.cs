using System.Collections.Generic;
using Essentials;
using Newtonsoft.Json.Linq;

namespace Progression
{
    public class ProgressionData : Dictionary<NamespacedKey, JToken>
    {
        public JToken Serialize()
        {
            var result = new JArray();
            foreach (var entry in this)
            {
                result.Add(new JObject
                {
                    ["id"] = entry.Key.Serialize(),
                    ["data"] = entry.Value
                });
            }

            return result;
        }

        public static bool TryParse(JToken json, out ProgressionData data)
        {
            return TryParse(json as JArray, out data);
        }

        public static bool TryParse(JArray json, out ProgressionData data)
        {
            if (json == null)
            {
                data = null;
                return false;
            }

            data = new ProgressionData();
            foreach (var entryJson in json)
            {
                if (!(entryJson is JObject entrySection)) continue;
                var entryId = entrySection["id"] != null && NamespacedKey.TryParse(entrySection["id"], out var id)
                    ? id
                    : default;
                var entryData = entrySection["data"];
                if (entryId == default || entryData == null) continue;
                data[entryId] = entryData;
            }

            return true;
        }
    }
}