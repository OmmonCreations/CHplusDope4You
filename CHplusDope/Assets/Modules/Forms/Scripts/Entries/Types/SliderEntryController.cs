using System;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Forms.Types
{
    public class SliderEntryController : ValueEntry<SliderEntry>
    {
        private const string NumberFormat = "N";

        [SerializeField] private Slider _slider = null;
        [SerializeField] private Image _fillImage = null;
        [SerializeField] private TMP_Text _displayText = null;

        private float _value;

        private float _min;
        private float _max;
        private float _step;

        private string _displayFormat;
        private string _numberFormat;

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
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        protected override void ApplyEntry(SliderEntry entry)
        {
            var min = entry.Min;
            var max = entry.Max;
            var step = entry.Step;
            var displayFormat = entry.DisplayFormat;
            var numberFormat = entry.NumberFormat;

            _displayFormat = displayFormat;
            _numberFormat = numberFormat;

            _min = min;
            _max = max;
            _step = step;
            _changed = entry.Changed;

            var useSteps = Mathf.Abs(step) > 0;
            var interval = useSteps ? step : 1;
            _slider.minValue = min / interval;
            _slider.maxValue = max / interval;
            _slider.wholeNumbers = useSteps;
            _slider.SetValueWithoutNotify(entry.Value != null ? (float) entry.Value :
                entry.DefaultValue != null ? (float) entry.DefaultValue :
                (_slider.maxValue - _slider.minValue) / 2 + _slider.minValue);
            _fillImage.fillAmount = _slider.normalizedValue;
        }

        protected override void ApplyValue(JToken value)
        {
            base.ApplyValue(value);
            _value = value != null ? (float) value : 0;
            ApplySliderValue((float) value);
            UpdateNumberDisplay();
        }

        private void OnSliderValueChanged(float sliderValue)
        {
            var normalizedValue = (sliderValue - _slider.minValue) / (_slider.maxValue - _slider.minValue);
            var value = _min + (_max - _min) * normalizedValue;
            _fillImage.fillAmount = normalizedValue;
            OnValueChanged(value);
        }

        private void OnValueChanged(float value)
        {
            if (Math.Abs(value - _value) <= 0) return;
            Value = value;
            if (_changed != null) _changed(value);
        }

        private void ApplySliderValue(float value)
        {
            var normalizedValue = Mathf.Clamp01((value - _min) / (_max - _min));
            _slider.SetValueWithoutNotify(_slider.minValue +
                                          (_slider.maxValue - _slider.minValue) * normalizedValue);
        }

        private void UpdateNumberDisplay()
        {
            var displayFormat = _displayFormat != null ? _displayFormat : "{amount}";
            var numberFormat = _numberFormat != null ? _numberFormat : NumberFormat;
            _displayText.text = displayFormat.Replace("{amount}", _value.ToString(numberFormat));
        }
    }
}