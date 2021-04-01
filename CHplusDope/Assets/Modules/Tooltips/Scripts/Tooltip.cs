using System.Collections.Generic;

namespace Tooltips
{
    public class Tooltip
    {
        public Dictionary<string,TooltipContent> Contents { get; }

        public Tooltip() => Contents = new Dictionary<string, TooltipContent>();
        
        public Tooltip(Dictionary<string,TooltipContent> contents)
        {
            Contents = contents;
        }

        public TooltipContent this[string key]
        {
            get => Contents[key];
            set => Contents[key] = value;
        }
    }
}