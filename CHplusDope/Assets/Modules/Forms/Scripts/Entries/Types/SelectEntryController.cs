using System;
using System.Collections.Generic;
using System.Linq;
using Localizator;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace Forms.Types
{
    public class SelectEntryController : ValueEntry<SelectEntry>
    {
        [SerializeField] private TMP_Dropdown _dropdown = null;

        private SelectOption[] _options;
        
        private Action<JToken> _changed;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            _dropdown.onValueChanged.AddListener(ApplyDropdownValue);
        }

        protected override void ApplyValue(JToken value)
        {
            base.ApplyValue(value);
            SetDropdownValue(value);
        }

        protected override void ApplyEntry(SelectEntry entry)
        {
            var options = entry.Options;
            _options = options;
            ApplyOptions(options);
            SetDropdownValue(Value);
            _changed = entry.Changed;
        }

        public override void OnDependencyUpdated()
        {
            base.OnDependencyUpdated();
            if (ValueDependency == null) return;
            ApplyOptions(_options);
        }

        private void ApplyOptions(SelectOption[] options)
        {
            _dropdown.options = options
                .Select(o=>new TMP_Dropdown.OptionData(GetLocalizedString(o.Label), o.Sprite))
                .ToList();
            SetDropdownValue(Value);
            var selectedValue = _options != null && _options.Length > _dropdown.value
                ? _options[_dropdown.value].Value
                : DefaultValue;
            if(selectedValue!=Value) base.ApplyValue(selectedValue);
        }

        private string GetLocalizedString(LocalizationKey key)
        {
            return Form.Localization.GetString(key);
        }

        private void SetDropdownValue(JToken value)
        {
            if (_options == null) return;
            var option = _options.FirstOrDefault(o => o.Value.Equals(value));
            if (option == null) option = _options.FirstOrDefault(o => o.Value.Equals(DefaultValue));
            var valueIndex = option != null ? Array.IndexOf(_options, option) : 0;
            _dropdown.SetValueWithoutNotify(valueIndex);
        }
        
        private void ApplyDropdownValue(int valueIndex)
        {
            var option = valueIndex>=0 && valueIndex<_options.Length ? _options[valueIndex] : null;
            Value = option != null ? option.Value : null;
            if (_changed != null) _changed(option!=null ? option.Value : null);
        }

    }
}