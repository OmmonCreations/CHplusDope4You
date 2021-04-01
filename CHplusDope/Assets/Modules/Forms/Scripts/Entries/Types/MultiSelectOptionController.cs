using Localizator;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Forms.Types
{
    public class MultiSelectOptionController : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private LocalizedText _label = null;

        public bool isOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        }
        
        public JToken Value { get; private set; }

        public Toggle.ToggleEvent onValueChanged => _toggle.onValueChanged;

        public void Initialize(MultiSelectOption option)
        {
            Value = option.Value;
            _label.key = option.Label;
        }
        
        internal void Remove()
        {
            Destroy(gameObject);
        }
    }
}