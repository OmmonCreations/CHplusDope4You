using System;
using Localizator;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Forms.Types
{
    public class BoolEntryController : ValueEntry<BoolEntry>
    {
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private LocalizedText _labelText = null;

        private LocalizationKey _trueLabel;
        private LocalizationKey _falseLabel;
        private Action<bool> _changed;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void ApplyEntry(BoolEntry entry)
        {
            _trueLabel = entry.TrueLabel;
            _falseLabel = entry.FalseLabel;
            _changed = entry.Changed;
        }

        protected override void ApplyValue(JToken value)
        {
            if (value == null)
            {
                value = DefaultValue != null && DefaultValue.Type == JTokenType.Boolean && (bool) DefaultValue;
            }

            base.ApplyValue(value);
            _toggle.isOn = value != null && value.Type == JTokenType.Boolean && (bool) value;
        }

        private void OnValueChanged(bool value)
        {
            if (_labelText)
            {
                var label = (value ? _trueLabel : _falseLabel);
                _labelText.key = label;
                _labelText.gameObject.SetActive(!string.IsNullOrWhiteSpace(_labelText.text));
            }
            base.ApplyValue(value);
            if(_changed!=null) _changed(value);
        }
    }
}