using System;

namespace Forms.Types
{
    public class FloatEntry : ValueEntry
    {
        public Action<float> Changed { get; set; }
        
        public float Min { get; set; } = float.MinValue;
        public float Max { get; set; } = float.MaxValue;
        public float Step { get; set; } = 0;
        
        protected FloatEntry(string key) : base(key)
        {
            
        }
    }
}