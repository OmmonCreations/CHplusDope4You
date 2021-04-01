using Localizator;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace Forms.Types
{
    public class SelectOption
    {
        public JToken Value { get; }
        public LocalizationKey Label { get; }
        public Sprite Sprite { get; }

        private SelectOption(JObject data)
        {
            Value = data["value"];
            Label = new LocalizationKey(data["label"]!=null ? (string) data["label"] : (string) data["value"]);
            Sprite = null;
        }

        private SelectOption(JToken value, LocalizationKey label, Sprite sprite)
        {
            Value = value;
            Label = label;
            Sprite = sprite;
        }

        public static SelectOption Get(JToken value, LocalizationKey label, Sprite sprite = null)
        {
            return new SelectOption(value, label, sprite);
        }

        public static SelectOption Get(JToken data)
        {
            return new SelectOption(data is JObject o
                ? o
                : new JObject()
                {
                    ["value"] = data
                });
        }
    }
}