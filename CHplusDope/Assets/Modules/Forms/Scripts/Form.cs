using System;
using Localizator;
using Newtonsoft.Json.Linq;

namespace Forms
{
    public class Form
    {
        public FormEntry[] Entries { get; set; }
        public ILocalization Localization { get; set; }
        public Action<FormData> onValueChanged { get; set; }

        public Form(ILocalization localization, params FormEntry[] entries)
        {
            Entries = entries;
            Localization = localization;
        }
        
        public Form(params FormEntry[] entries) : this(null, entries)
        {
            
        }
    }
}