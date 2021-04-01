using System;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace Forms.Types
{
    public class StringEntryController : ValueEntry<StringEntry>
    {
        [SerializeField] private TMP_InputField _inputField = null;

        protected TMP_InputField InputField => _inputField;
        
        private Action<string> _changed;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (_inputField == null)
            {
                Debug.LogError("No InputField assigned to "+gameObject.name+"!");
                return;
            }
            _inputField.onSubmit.AddListener(OnValueChanged);
            _inputField.onDeselect.AddListener(OnValueChanged);
        }

        protected override void ApplyEntry(StringEntry entry)
        {
            _changed = entry.Changed;
        }

        protected override void ApplyValue(JToken value)
        {
            base.ApplyValue(value);
            var stringValue = (string) value;
            _inputField.SetTextWithoutNotify(stringValue);
        }

        private void OnValueChanged(string value)
        {
            base.ApplyValue(value);
            if (_changed != null) _changed(value);
        }
    }
}