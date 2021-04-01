using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Forms.Types
{
    public class ArrayEntryController : ValueEntry<ArrayEntry>
    {
        [SerializeField] private RectTransform _entriesArea = null;

        private FormEntry Template { get; set; }
        private FormEntryController[] _entries = null;
        
        private Action<JToken[]> _changed;

        protected override void ApplyEntry(ArrayEntry entry)
        {
            Template = entry.Template;
            _changed = entry.Changed;
        }

        protected override void ApplyValue(JToken value)
        {
            base.ApplyValue(value);
            var valueArray = value as JArray;
            CreateEntries(valueArray);
        }

        private void CreateEntries(JArray valueArray)
        {
            if(_entries!=null) ClearEntries();
            if (valueArray == null) return;
            
            var template = Template;
            if (template == null)
            {
                Debug.LogWarning("No template configuration provided for ArrayEntry "+Key);
                return;
            }
            
            var entries = new List<FormEntryController>(valueArray.Count);
            foreach (var value in valueArray)
            {
                var entry = Form.CreateEntry(_entriesArea, template);
                if (entry == null) continue;
                if (entry is ValueEntryController valueEntry)
                {
                    valueEntry.Value = value;
                }
                entries.Add(entry);
            }

            _entries = entries.ToArray();
        }

        private void ClearEntries()
        {
            foreach (var entry in _entries)
            {
                entry.Remove();
            }

            _entries = null;
        }

    }
}