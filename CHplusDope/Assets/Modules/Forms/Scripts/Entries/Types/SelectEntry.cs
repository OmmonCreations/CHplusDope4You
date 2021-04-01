using System;
using Newtonsoft.Json.Linq;

namespace Forms.Types
{
        public class SelectEntry : ValueEntry
        {
            public Action<JToken> Changed { get; set; }
            
            public SelectOption[] Options { get; }
            
            protected SelectEntry(string key, params SelectOption[] options) : base(key)
            {
                Options = options;
            }
        }
}