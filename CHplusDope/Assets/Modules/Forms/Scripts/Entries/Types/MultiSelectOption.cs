using Localizator;
using Newtonsoft.Json.Linq;

namespace Forms.Types
{
    public class MultiSelectOption
    {
        public JToken Value { get; }
        public LocalizationKey Label { get; }

        public MultiSelectOption(JToken value) : this(value, new LocalizationKey(null) {fallback = value.ToString()})
        {
        }

        public MultiSelectOption(JToken value, LocalizationKey label)
        {
            Value = value;
            Label = label;
        }
    }
}