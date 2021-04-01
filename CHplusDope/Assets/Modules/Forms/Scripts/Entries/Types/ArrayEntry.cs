using System;
using Newtonsoft.Json.Linq;

namespace Forms.Types
{
        public class ArrayEntry : ValueEntry
        {
            public Action<JToken[]> Changed { get; set; }
            
            public FormEntry Template { get; }
            
            protected ArrayEntry(string key, FormEntry template) : base(key)
            {
                Template = template;
            }
        }
}