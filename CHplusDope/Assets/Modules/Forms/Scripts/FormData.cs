using System;
using Newtonsoft.Json.Linq;

namespace Forms
{
    public class FormData
    {
        private JObject Data { get; }

        public FormData(JObject data)
        {
            Data = data;
        }

        public JObject Serialize()
        {
            return new JObject(Data);
        }

        public JToken Get(string key)
        {
            return Data.SelectToken(key);
        }

        public string GetString(string key)
        {
            var value = Data.SelectToken(key);
            return value != null ? (string) value : null;
        }

        public float GetFloat(string key)
        {
            var value = Data.SelectToken(key);
            return value != null ? (float) value : 0;
        }

        public int GetInt(string key)
        {
            var value = Data.SelectToken(key);
            return value != null ? (int) value : 0;
        }

        public bool GetBool(string key)
        {
            var value = Data.SelectToken(key);
            return value != null && (bool) value;
        }

        public Guid GetGuid(string key)
        {
            var value = Data.SelectToken(key);
            return value != null && Guid.TryParse((string) value, out var u) ? u : default;
        }
    }
}