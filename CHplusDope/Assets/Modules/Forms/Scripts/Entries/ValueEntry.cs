using Newtonsoft.Json.Linq;

namespace Forms
{
    public class ValueEntry : FormEntry
    {
        public string Key { get; }
        public JToken Value { get; set; }
        public JToken DefaultValue { get; set; } = null;
        public bool Required { get; set; } = false;

        protected ValueEntry(string key)
        {
            Key = key;
        }
    }
}