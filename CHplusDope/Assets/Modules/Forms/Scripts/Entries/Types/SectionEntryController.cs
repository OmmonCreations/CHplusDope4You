using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Forms.Types
{
    public class SectionEntryController : FormEntryController<SectionEntry>
    {
        [SerializeField] private RectTransform _entriesArea = null;

        private FormEntryController[] _entries = null;
        
        protected override void ApplyEntry(SectionEntry entry)
        {
            var entries = entry.Entries;
            CreateEntries(entries);
        }

        private void ClearEntries()
        {
            if (_entries == null) return;
            foreach(var entry in _entries)
            {
                entry.Remove();
            }
        }

        private void CreateEntries(FormEntry[] entries)
        {
            ClearEntries();
            if (entries == null) return;
            _entries = Form.CreateEntries(_entriesArea, entries);
        }

        public override void SaveValues(JObject data)
        {
            base.SaveValues(data);
            if (_entries == null) return;
            
            foreach (var entry in _entries)
            {
                entry.SaveValues(data);
            }
        }

        public override void ApplyDefaults()
        {
            base.ApplyDefaults();
            if (_entries == null) return;
            
            foreach (var entry in _entries)
            {
                entry.ApplyDefaults();
            }
        }

    }
}