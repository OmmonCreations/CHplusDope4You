using System;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Forms.Types
{
    public class FloatEntryController : ValueEntry<FloatEntry>
    {
        [SerializeField] private TMP_InputField _inputField = null;
        
        [Header("ColorBlock (Normal)")]
        [SerializeField] private ColorBlock _okColorBlock = ColorBlock.defaultColorBlock;
        [Header("ColorBlock (Invalid Input)")]
        [SerializeField] private ColorBlock _invalidColorBlock = ColorBlock.defaultColorBlock;

        private float _value;
        private bool _valid = false;

        private float _min;
        private float _max;
        
        private Action<float> _changed;
        
        public new float Value
        {
            get => _value;
            set
            {
                base.Value = value;
                _value = value;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _inputField.onSubmit.AddListener(OnValueChanged);
            _inputField.onDeselect.AddListener(OnValueChanged);
        }

        protected override void ApplyEntry(FloatEntry entry)
        {
            var min = entry.Min;
            var max = entry.Max;
            var step = entry.Step;

            _min = min;
            _max = max;
            _changed = entry.Changed;

            _inputField.contentType = Mathf.Abs(step - Mathf.RoundToInt(step)) <= 0
                ? TMP_InputField.ContentType.IntegerNumber
                : TMP_InputField.ContentType.DecimalNumber;
        }

        protected override void ApplyValue(JToken value)
        {
            if (value == null) value = DefaultValue;
            if(value==null) value = new JValue(0);
            var f = (float) value;
            base.ApplyValue(f);
            _value = f;
            _inputField.SetTextWithoutNotify(((float)value).ToString(""));
            _valid = f >= _min && f <= _max;
            UpdateColors();
        }

        private void OnValueChanged(string valueString)
        {
            var valid = float.TryParse(valueString, out var f);
            if (valid)
            {
                OnValueChanged(f);
                return;
            }

            _valid = false;
            UpdateColors();
        }

        private void OnValueChanged(float value)
        {
            if (Math.Abs(value - _value) <=0) return;
            base.ApplyValue(value);
            _value = value;
            _valid = value >= _min && value <= _max;
            UpdateColors();
            if(_changed!=null) _changed(value);
        }

        private void UpdateColors()
        {
            _inputField.colors = _valid ? _okColorBlock : _invalidColorBlock;
        }

    }
}