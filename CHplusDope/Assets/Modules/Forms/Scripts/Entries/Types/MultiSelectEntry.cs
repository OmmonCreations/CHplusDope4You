using System;
using Newtonsoft.Json.Linq;

namespace Forms.Types
{
    public class MultiSelectEntry : ValueEntry
    {
        public Action<JArray> Changed { get; set; }
        
        public MultiSelectOption[] Options { get; }
        
        protected MultiSelectEntry(string key, params MultiSelectOption[] options) : base(key)
        {
            Options = options;
        }
    }
}