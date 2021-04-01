using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Switches
{
    [ExecuteAlways]
    public class Switch : Toggle
    {
        [SerializeField] private RectTransform _knob = null;
        [SerializeField] private ColorBlock _onColors = ColorBlock.defaultColorBlock;
        [SerializeField] private ColorBlock _offColors = ColorBlock.defaultColorBlock;

#if !UNITY_EDITOR
        private bool _transitioning = false;
        private float _t;
#endif
        private bool _state;

        private void Update()
        {
            var value = isOn;
#if UNITY_EDITOR
            if (_state == value) return;
            _state = value;
            ApplyValue(value);
            SetState(value ? 1 : 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_knob);
#else
            if (_state == value && !_transitioning) return;
            if (_transitioning && _state == value)
            {
                _t += Time.deltaTime * 0.2f * (value ? 1 : -1);
                SetState(_t);
                return;
            }

            _transitioning = true;
            _state = value;
            _t = value ? 0 : 1;
            ApplyValue(value);
            SetState(_t);
#endif
        }

        private void ApplyValue(bool value)
        {
            colors = value ? _onColors : _offColors;
        }
        
        private void SetState(float t)
        {
#if !UNITY_EDITOR
            _t = t;
#endif
            if (!_knob) return;
            var progress = Mathf.SmoothStep(0, 1, Mathf.Clamp01(t));
            _knob.anchorMin = new Vector2(progress, 0);
            _knob.anchorMax = new Vector2(progress, 1);
            _knob.anchoredPosition = Vector2.zero;
        }
    }
}
