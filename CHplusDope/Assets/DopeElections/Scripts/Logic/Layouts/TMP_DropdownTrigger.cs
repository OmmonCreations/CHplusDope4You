using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class TMP_DropdownTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> _onValueChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent _onValueSelected = new UnityEvent();
        [SerializeField] private UnityEvent _onValueDeselected = new UnityEvent();

        private TMP_Dropdown _dropdown;

        public UnityEvent<string> onValueChanged => _onValueChanged;
        public UnityEvent onValueSelected => _onValueSelected;
        public UnityEvent onValueDeselected => _onValueDeselected;

        private void OnEnable()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            HookEvents();
            if (_dropdown) OnValueChanged(_dropdown.value);
        }

        private void OnDisable()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            if (_dropdown)
            {
                _dropdown.onValueChanged.AddListener(OnValueChanged);
            }
        }

        private void ReleaseHooks()
        {
            if (_dropdown) _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(int value)
        {
            var options = _dropdown.options;
            var text = value >= 0 && value < options.Count ? options[value].text : null;
            _onValueChanged.Invoke(text != null ? text : "");
            if (text != null) _onValueSelected.Invoke();
            else
            {
                _onValueDeselected.Invoke();
            }
        }

        public void TriggerUpdate()
        {
            if (_dropdown) OnValueChanged(_dropdown.value);
        }
    }
}