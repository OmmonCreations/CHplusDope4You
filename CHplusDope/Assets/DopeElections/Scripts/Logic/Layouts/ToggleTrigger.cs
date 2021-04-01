using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onEnable = new UnityEvent();
        [SerializeField] private UnityEvent _onDisable = new UnityEvent();
        [SerializeField] private UnityEvent<bool> _onValueChanged = new UnityEvent<bool>();

        private Toggle _toggle;

        public UnityEvent onEnable => _onEnable;
        public UnityEvent onDisable => _onDisable;
        public UnityEvent<bool> onValueChanged => _onValueChanged;

        private void OnEnable()
        {
            _toggle = GetComponent<Toggle>();
            HookEvents();
            TriggerUpdate();
        }

        private void Start()
        {
            TriggerUpdate();
        }

        private void OnDisable()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            if (_toggle) _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void ReleaseHooks()
        {
            if (_toggle) _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            if (value) _onEnable.Invoke();
            else _onDisable.Invoke();
            _onValueChanged.Invoke(value);
        }

        public void TriggerUpdate()
        {
            if (_toggle) OnValueChanged(_toggle.isOn);
        }
    }
}