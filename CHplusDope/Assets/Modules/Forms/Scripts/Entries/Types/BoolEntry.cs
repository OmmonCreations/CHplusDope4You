using System;
using Localizator;

namespace Forms.Types
{
    public class BoolEntry : ValueEntry
    {
        public Action<bool> Changed { get; set; }
        
        public LocalizationKey TrueLabel { get; set; }
        public LocalizationKey FalseLabel { get; set; }
        
        protected BoolEntry(string key) : base(key)
        {
        }
    }
}