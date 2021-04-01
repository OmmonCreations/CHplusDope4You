using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Localizator
{
    public sealed class Localization : ILocalization
    {
        public delegate void LocalizationEditedEvent(string key, string value);

        public event LocalizationEditedEvent LocalizationEdited;

        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public int Count => _data.Count;

        public Localization()
        {
        }

        public Localization(IEnumerable<KeyValuePair<string, string>> data)
        {
            foreach (var entry in data)
            {
                _data[entry.Key] = entry.Value;
            }
        }

        public string GetString(LocalizationKey key)
        {
            return TryGetString(key.path, out var value) ? value : key.fallback != null ? key.fallback : key.path;
        }

        public bool TryGetString(LocalizationKey key, out string value)
        {
            return TryGetString(key.path, out value);
        }

        private bool TryGetString(string path, out string value)
        {
            if (path == null)
            {
                value = null;
                return false;
            }

            return _data.TryGetValue(path, out value);
        }

        public void SetString(LocalizationKey key, string value)
        {
            SetString(key.path, value);
        }

        public void SetString(string key, string value)
        {
            _data[key] = value;
            if (LocalizationEdited != null) LocalizationEdited(key, value);
        }

        public JObject Serialize()
        {
            var result = new JObject();
            foreach (var entry in _data)
            {
                result[entry.Key] = entry.Value;
            }

            return result;
        }

        public static Localization Deserialize(string data)
        {
            try
            {
                var json = JObject.Parse(data);
                return Deserialize(json);
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(data)) Debug.LogError("Invalid Json encountered:\n" + data);
                return new Localization();
            }
        }

        public static Localization Deserialize(JObject json)
        {
            var entries = new Dictionary<string, string>();
            foreach (var entry in json)
            {
                entries.Add(entry.Key, entry.Value.ToString());
            }

            return new Localization(entries);
        }

        public override string ToString()
        {
            return string.Join("\n", _data.Select(e => "- " + e.Key + ": " + e.Value));
        }
    }
}