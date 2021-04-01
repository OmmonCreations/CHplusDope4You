using System;

namespace Forms.Types
{
    public class StringEntry : ValueEntry
    {
        public Action<string> Changed { get; set; }
        
        public StringEntry(string key) : base(key)
        {
        }
    }
}