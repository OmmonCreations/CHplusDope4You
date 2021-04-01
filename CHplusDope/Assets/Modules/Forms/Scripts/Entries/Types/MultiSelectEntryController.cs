using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Forms.Types
{
    public class MultiSelectEntryController : ValueEntry<MultiSelectEntry>
    {
        [SerializeField] private RectTransform _optionsArea = null;
        [SerializeField] private MultiSelectOptionController _optionTemplate = null;

        private JArray _valueArray = null;
        private MultiSelectOptionController[] _options = null;
        
        private Action<JArray> _changed;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _optionTemplate.gameObject.SetActive(false);
        }

        protected override void ApplyValue(JToken value)
        {
            base.ApplyValue(value);
            var valueArray = value as JArray;
            if (valueArray == null || _options==null) return;
            ApplyValue(valueArray);
        }

        private void ApplyValue(JArray value)
        {
            foreach (var option in _options)
            {
                option.isOn = value.Contains(option.Value);
            }
        }

        protected override void ApplyEntry(MultiSelectEntry entry)
        {
            var options = entry.Options;
            ApplyOptions(options);
            _changed = entry.Changed;
        }

        private void ApplyOptions(MultiSelectOption[] optionsArray)
        {
            ClearOptions();
            var options = new List<MultiSelectOptionController>();
            foreach (var element in optionsArray)
            {
                var option = CreateOption(element);
                if (option == null) continue;
                options.Add(option);
            }

            _options = options.ToArray();
            
            if(_valueArray!=null) ApplyValue(_valueArray);
        }

        private void ClearOptions()
        {
            if (_options == null) return;
            foreach (var option in _options)
            {
                option.Remove();
            }
        }

        private MultiSelectOptionController CreateOption(MultiSelectOption element)
        {
            var instanceObject = Instantiate(_optionTemplate.gameObject, _optionsArea, false);
            var instance = instanceObject.GetComponent<MultiSelectOptionController>();
            instance.Initialize(element);
            instance.onValueChanged.AddListener(OnValueChanged);
            instanceObject.SetActive(true);
            return instance;
        }

        private void OnValueChanged(bool b)
        {
            var value = new JArray();
            foreach (var option in _options)
            {
                if (!option.isOn) continue;
                value.Add(option.Value);
            }

            base.ApplyValue(value);
            _valueArray = value;
            
            if (_changed != null) _changed(value);
        }

    }
}