using System;

namespace Forms.Types
{
    public class SliderEntry : ValueEntry
    {
        public Action<float> Changed { get; set; }
        
        public float Min { get; }
        public float Max { get; }
        public float Step { get; set; } = 0;
        public string DisplayFormat { get; set; } = null;
        public string NumberFormat { get; set; } = "N";
        
        public SliderEntry(string key, float min, float max) : base(key)
        {
            Min = min;
            Max = max;
        }
    }
}